﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TalentLMS.Api
{
    [Headers("Authorization: Basic")]
    public interface IGroups
    {
        [Get("/groups")]
        Task<List<Groups.BasicGroup>> All();

        [Get("/groups/id:{groupId}")]
        Task<Groups.Group> Group(string groupId);
    }

    namespace Groups
    {
        public record BasicGroup(
            string Id,
            string Name,
            string Description,
            string Key,
            string Price,
            string OwnerId,
            string? BelongToBranch,
            string? MaxRedemptions,
            string? RedemptionsSoFar);

        public record Group(
            string Id,
            string Name,
            string Description,
            string Key,
            string Price,
            string OwnerId,
            string? BelongsToBranch,
            string? MaxRedemptions,
            string? RedemptionsSofar,
            List<Group.User> Users,
            List<Group.Course> Courses)
        {
            public record User(
                string Id,
                string Name);

            public record Course(
                string Id,
                string Name);
        }
    }
}