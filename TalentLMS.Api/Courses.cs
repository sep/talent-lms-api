using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;
using TalentLMS.Api.Courses;

namespace TalentLMS.Api
{
    [Headers("Authorization: Basic")]
    public interface ICourses
    {
        [Get("/getuserstatusincourse/user_id:{userId},course_id:{courseId}")]
        Task<UserCourseStatus> UserStatus(string userId, string courseId);

        [Get("/courses/id:{courseId}")]
        Task<Course> Course(string courseId);
    }

    namespace Courses
    {
        public record Course(string Id, string Name, string Description, List<CourseUser> Users, List<CourseUnit> Units);
        public record CourseUser(string Id, string Name, string Role, string Completion_Percentage);
        public record CourseUnit(string Id, string Type, string Name, string Url);

        public record UserCourseStatus(string Role, string Completion_Percentage, string Completion_Status, List<UnitStatus> Units);
        public record UnitStatus(string Id, string Name, string Type, [JsonProperty(PropertyName = "completion_status")] string CompletionStatus, string Completed_On, string Score, string Total_Time) { }
    }
}