using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using TalentLMS.Api;
using TalentLMS.Api.Courses;
using static System.Linq.Enumerable;

namespace TalentLMSReporting
{
    public class Runner
    {
        private readonly ICourses _coursesApi;
        private readonly IUsers _usersApi;
        private readonly IGroups _groupsApi;
        private readonly ISiteInfo _siteInfo;
        private readonly IOutput _output;

        public Runner(ICourses coursesApi, IUsers usersApi, IGroups groupsApi, ISiteInfo siteInfo, IOutput output)
        {
            _coursesApi = coursesApi;
            _usersApi = usersApi;
            _groupsApi = groupsApi;
            _siteInfo = siteInfo;
            _output = output;
        }

        public async Task<ExitCode> Users()
        {
            var users = await _usersApi.All();

            _output.Write(users);

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

            Empty<IEnumerable<string>>()
               .Concat(new[] { "name"}.Concat(Enumerable.Select(courseUnits, u => u.Name)))
               .Concat(GetProgressByUser())
               .Transpose()
               .Then(progress =>
                {
                    _output.Write(
                        header: progress.First(),
                        rows: progress.Skip(1));
                });
            return ExitCode.Success;

            IEnumerable<IEnumerable<string>> GetProgressByUser()
            {
                return courseLearners
                   .Select(LearnerStatus)
                   .Select(_ => _.Result);
            }

            async Task<IEnumerable<string>> LearnerStatus(Course.User learner)
            {
                var status = await _coursesApi.UserStatus(learner.Id, course.Id);

                return new[] {learner.Name}
                   .Concat(courseUnits.Select(u => StatusForUnit(u, status)));
            }

            static string StatusForUnit(Course.Unit unit, UserCourseStatus status) => status
               .Units
               .SingleOrDefault(u => u.Id == unit.Id)
               .Then(CompletionStatus);

            static string CompletionStatus(UserCourseStatus.Unit status) => status.CompletionStatus switch
            {
                "Completed" => "+",
                _ => ""
            };
        }

        public async Task<ExitCode> Groups()
        {
            var groups = await _groupsApi.All();

            _output.Write(groups);

            return ExitCode.Success;
        }

        public async Task<ExitCode> DomainDetails()
        {
            var domainDetails = (await _siteInfo.DomainDetails()).ToNamePairs();
            var rateLimit = (await _siteInfo.RateLimit()).ToNamePairs();

            _output.Write(domainDetails.Concat(rateLimit));

            return ExitCode.Success;
        }
    }
}