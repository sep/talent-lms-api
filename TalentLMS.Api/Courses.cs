﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public record Course(
            string Id,
            string Name,
            string Code,
            string CategoryId,
            string Description,
            string Price,
            string Status,
            DateTime CreationDate,
            DateTime LastUpdateOn,
            string CreatorId,
            string HideFromCatalog,
            string TimeLimit,
            string Level,
            string Shared,
            string SharedUrl,
            string Avatar,
            string BigAvatar,
            string Certification,
            string CertificationDuration,
            List<Course.User> Users,
            List<Course.Unit> Units)
        {
            public record User(
                string Id,
                string Name,
                string Role,
                DateTime EnrolledOn,
                string EnrolledOnTimestamp,
                DateTime? CompletedOn,
                string CompletedOnTimestamp,
                string CompletionPercentage,
                DateTime? ExpiredOn,
                string ExpiredOnTimestamp,
                string TotalTime)
            {
                public bool IsLearner => Role == "learner";
            }

            public record Unit(
                string Id,
                string Type,
                string Name,
                string DelayTime,
                string AggregatedDelayTime,
                string FormattedAggregatedDelayTime,
                string Url)
            {
                public bool IsSection => Type == "Section";
            }
        }

        public record UserCourseStatus(
            string Role,
            DateTime EnrolledOn,
            string EnrolledOnTimestamp,
            string CompletionStatus,
            string CompletionPercentage,
            DateTime? CompletedOn,
            string CompletedOnTimestamp,
            DateTime? ExpiredOn,
            string ExpiredOnTimestamp,
            string TotalTime,
            List<UserCourseStatus.Unit> Units)
        {
            public record Unit(
                string Id,
                string Name,
                string Type,
                string CompletionStatus,
                DateTime? CompletedOn,
                string CompletedOnTimestamp,
                string Score,
                string TotalTime);
        }
    }
}