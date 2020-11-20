using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public Runner(TextWriter stdOut, ICourses coursesApi)
        {
            _stdOut = stdOut;
            _coursesApi = coursesApi;
        }

        public async Task<ExitCode> CourseProgress(string courseId)
        {
            var course = await _coursesApi.Course(courseId);
            var courseUnits = course.Units
               .Where(u => u.Type != "Section")
               .ToList();
            var courseLearners = course.Users
               .Where(u => u.Role == "learner")
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

            static string CompletionStatus(UnitStatus status) => status.CompletionStatus switch
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
    }
}