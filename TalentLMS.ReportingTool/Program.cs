using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using TalentLMS.Api;

namespace TalentLMSReporting
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var exitCode = await Parser.Default.ParseArguments<
                    SiteInfoOptions,
                    GroupsOptions,
                    UsersOptions,
                    CourseProgressOptions>(args)
               .MapResult(
                    (CourseProgressOptions opts) => GetRunner(opts, Console.Out).CourseProgress(opts.CourseId),
                    (UsersOptions opts) => GetRunner(opts, Console.Out).Users(),
                    (GroupsOptions opts) => GetRunner(opts, Console.Out).Groups(),
                    (SiteInfoOptions opts) => GetRunner(opts, Console.Out).DomainDetails(),
                    Err);

            if (exitCode != ExitCode.Success)
            {
                Console.Error.WriteLine($"{exitCode.Value} - {exitCode.Message}");
            }

            return exitCode.Value;

            static Task<ExitCode> Err(IEnumerable<Error> parseErrors)
            {
                return Task.FromResult(ExitCode.GeneralError);
            }

            static Runner GetRunner(BaseOptions opts, TextWriter stdout)
            {
                var api = new Api(opts.ServerUri, opts.ApiKey);
                return new Runner(api.Courses, api.Users, api.Groups, api.SiteInfo, opts.GetOutputMethod(Console.Out));
            }

        }
    }

    public class BaseOptions
    {
        [Option('k', "api-key", Required = true, HelpText = "API Key for TalentLMS API")]
        public string ApiKey { get; init; }

        [Option('s', "server-uri", Required = true, HelpText = "URI for TalentLMS API (e.g. example.talentlms.com/api/v1/)")]
        public string ServerUri { get; init; }

        [Option('f', "output-format", Required = false, Default = OutputFormat.Console, HelpText = "URI for TalentLMS API (e.g. example.talentlms.com/api/v1/)")]
        public OutputFormat OutputFormat { get; init; }

        public IOutput GetOutputMethod(TextWriter stdOut) => OutputFormat switch
        {
            OutputFormat.Csv => new CsvOutput(stdOut),
            OutputFormat.Console => new ConsoleTableOutput(o =>
            {
                o.OutputTo = Console.Out;
                o.EnableCount = false;
            }),
            _ => throw new InvalidOperationException($"{OutputFormat} is not a supported output format.")
        };
    }

    public enum OutputFormat
    {
        Csv,
        Console
    }

    [Verb("course-progress", HelpText = "Course Status CSV.")]
    public class CourseProgressOptions : BaseOptions
    {
        [Option('c', "course-id", Required = true, HelpText = "Course ID for the course to display status for")]
        public string CourseId { get; init; }
    }

    [Verb("users", HelpText = "Users CSV.")]
    public class UsersOptions : BaseOptions
    {
    }

    [Verb("groups", HelpText = "Groups CSV.")]
    public class GroupsOptions : BaseOptions
    {
    }

    [Verb("site-info", HelpText = "Site Info CSV.")]
    public class SiteInfoOptions : BaseOptions
    {
    }

    public class ExitCode
    {
        public static ExitCode Success = new ExitCode(0, "Success");
        public static ExitCode GeneralError = new ExitCode(-1, "unspecified error");

        private ExitCode(int value, string message)
        {
            Value = value;
            Message = message;
        }

        public int Value { get; }
        public string Message { get; }
    }

}
