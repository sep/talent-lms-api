using System;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace TalentLMS.Api
{
    public class Api
    {
        private readonly string _serverUrl;
        private readonly RefitSettings _refitSettings;

        public Api(string serverUrl, string apiKey)
        {
            _serverUrl = serverUrl;
            _refitSettings = new RefitSettings {
                AuthorizationHeaderValueGetter = () => Task.FromResult(AuthHeader(apiKey))
            };
        }

        public ICourses Courses => RestService.For<ICourses>(_serverUrl, _refitSettings);
        public IUsers Users => RestService.For<IUsers>(_serverUrl, _refitSettings);

        static string AuthHeader(string apiKey) => Base64($"{apiKey}:");
        static string Base64(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
}