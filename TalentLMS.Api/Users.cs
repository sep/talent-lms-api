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
        public record User(string Id, string Login, string First_Name, string Last_Name, string Email, string User_Type, string Avatar, string Level, string Points) { }
    }
}