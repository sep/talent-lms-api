using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Refit;
using TalentLMS.Api;

namespace TalentLMSReporting
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var exitCode = await Parser.Default.ParseArguments<CourseProgressOptions>(args)
               .MapResult(
                    (CourseProgressOptions opts) =>
                    {
                        var refitSettings = new RefitSettings {
                            AuthorizationHeaderValueGetter = () => Task.FromResult(AuthHeader(opts.ApiKey))
                        };

                        var coursesApi = RestService.For<ICourses>(opts.ServerUri, refitSettings);
                        var runner = new Runner(Console.Out, coursesApi);

                        return runner.CourseProgress(opts.CourseId);
                    },
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

            static string AuthHeader(string apiKey) => Base64($"{apiKey}:");
            static string Base64(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }
    }

    public class BaseOptions
    {
        [Option('k', "api-key", Required = true, HelpText = "API Key for TalentLMS API")]
        public string ApiKey { get; init; }

        [Option('s', "server-uri", Required = true, HelpText = "URI for TalentLMS API (e.g. example.talentlms.com/api/v1/)")]
        public string ServerUri { get; init; }
    }

    [Verb("course-progress", HelpText = "Course Status CSV.")]
    public class CourseProgressOptions : BaseOptions
    {
        [Option('c', "course-id", Required = true, HelpText = "Course ID for the course to display status for")]
        public string CourseId { get; init; }
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
