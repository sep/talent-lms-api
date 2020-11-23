using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTables;
using CsvHelper;
using MoreLinq;
using TalentLMS.Api;
using TalentLMS.Api.Courses;

namespace TalentLMSReporting
{
    public class Runner
    {
        private readonly TextWriter _stdOut;
        private readonly ICourses _coursesApi;
        private readonly IUsers _usersApi;
        private readonly IGroups _groupsApi;
        private readonly ISiteInfo _siteInfo;

        public Runner(TextWriter stdOut, ICourses coursesApi, IUsers usersApi, IGroups groupsApi, ISiteInfo siteInfo)
        {
            _stdOut = stdOut;
            _coursesApi = coursesApi;
            _usersApi = usersApi;
            _groupsApi = groupsApi;
            _siteInfo = siteInfo;
        }

        public async Task<ExitCode> Users()
        {
            var users = await _usersApi.All();

            using (var csv = new CsvWriter(_stdOut, CultureInfo.CurrentUICulture))
            {
                csv.WriteHeader(users.GetType().GetGenericArguments().Single());
                await csv.WriteRecordsAsync(users);
            }
            return ExitCode.Success;
        }
        public async Task<ExitCode> CourseProgress(string courseId)
        {
            var course = await _coursesApi.Course(courseId);
            var courseUnits = course.Units
               .Where(u => !u.IsSection)
               .ToList();
            var courseLearners = course.Users
               .Where(u => u.IsLearner)
               .ToList();

            (await GetProgressByUser())
               .Transpose()
               .Then(WriteCsv);
            return ExitCode.Success;

            async Task<IEnumerable<IEnumerable<string>>> GetProgressByUser()
            {
                var rows = new List<IEnumerable<string>>();
                rows.Add(new[] { "name" }.Concat(courseUnits.Select(u => u.Name)));

                foreach (var courseUser in courseLearners)
                {
                    var row = new List<string>();
                    row.Add(courseUser.Name);

                    var status = await _coursesApi.UserStatus(courseUser.Id, course.Id);

                    foreach (var unit in courseUnits)
                    {
                        var unitStatus = status.Units.SingleOrDefault(u => u.Id == unit.Id);
                        row.Add(CompletionStatus(unitStatus));
                    }

                    rows.Add(row);
                }

                return rows;
            }

            static string CompletionStatus(UserCourseStatus.Unit status) => status.CompletionStatus switch
            {
                "Completed" => "+",
                _ => ""
            };

            void WriteCsv(IEnumerable<IEnumerable<string>> rows)
            {
                using (var csv = new CsvWriter(_stdOut, CultureInfo.CurrentUICulture, leaveOpen: true))
                {
                    rows
                       .Select(r => r.ToArray())
                       .ForEach(r =>
                        {
                            r.ForEach(csv.WriteField);
                            csv.NextRecord();
                        });
                }
            }
        }

        public async Task<ExitCode> Groups()
        {
            var groups = await _groupsApi.All();

            using (var csv = new CsvWriter(_stdOut, CultureInfo.CurrentUICulture))
            {
                csv.WriteHeader(groups.GetType().GetGenericArguments().Single());
                await csv.WriteRecordsAsync(groups);
            }
            return ExitCode.Success;
        }

        public async Task<ExitCode> DomainDetails()
        {
            var domainDetails = (await _siteInfo.DomainDetails()).ToNamePairs();
            var rateLimit = (await _siteInfo.RateLimit()).ToNamePairs();

            ConsoleTable
               .From(domainDetails.Concat(rateLimit))
               .Configure(o =>
                {
                    o.OutputTo = _stdOut;
                    o.EnableCount = false;
                })
               .Write();

            return ExitCode.Success;
        }
    }
}