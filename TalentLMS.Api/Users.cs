using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using TalentLMS.Api.Users;

namespace TalentLMS.Api
{
    [Headers("Authorization: Basic")]
    public interface IUsers
    {
        [Get("/users")]
        Task<List<User>> All();

        [Get("/users/id:{userId}")]
        Task<User> User(string userId);
    }

    namespace Users
    {
        public record BasicUser(
            string Id,
            string Login,
            string FirstName,
            string LastName,
            string Email,
            string RestrictEmail,
            string UserType,
            string TimeZone,
            string Language,
            string Status,
            DateTime? DeactivationDate,
            string Level,
            int Points,
            DateTime CreatedOn,
            DateTime LastUpdated,
            string LastUpdatedTimestamp,
            string Avatar,
            string Bio,
            string LoginKey);

        public record User(
            string Id,
            string Login,
            string FirstName,
            string LastName,
            string Email,
            string RestrictEmail,
            string UserType,
            string TimeZone,
            string Language,
            string Status,
            DateTime? DeactivationDate,
            string Level,
            int Points,
            DateTime CreatedOn,
            DateTime LastUpdated,
            string LastUpdatedTimestamp,
            string Avatar,
            string Bio,
            string LoginKey,
            List<User.Course> Courses,
            List<User.Branch> Branches,
            List<User.Group> Groups,
            List<User.Certification> Certifications,
            List<User.Badge> Badges)
        {
            public record Branch;

            public record Course(
                string Id,
                string Name,
                string Role,
                DateTime EnrolledOn,
                string EnrolledOnTimestamp,
                DateTime? CompletedOn,
                string CompletedOnTimestamp,
                string CompletionStatus,
                string CompletionPercentage,
                DateTime? ExpiredOn,
                string ExpiredOnTimestamp,
                string TotalTime);

            public record Group(
                string Id,
                string Name);

            public record Certification(
                string CourseId,
                string CourseName,
                string UniqueId,
                DateTime IssuedDate,
                DateTime? ExpirationDate,
                string DownloadUrl,
                string PublicUrl);

            public record Badge(
                string Name,
                string Type,
                string Criteria,
                DateTime IssuedOn,
                string BadgeSetId);
        }
    }
}