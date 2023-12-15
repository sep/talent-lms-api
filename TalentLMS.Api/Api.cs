using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace TalentLMS.Api
{
    public class Api
    {
        private readonly string _serverUrl;
        private readonly string _apiKey;
        private readonly RefitSettings _refitSettings;
        
        public Api(string serverUrl, string apiKey)
        {
            _serverUrl = serverUrl;
            _apiKey = apiKey;
            _refitSettings = new RefitSettings
            {

                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    // TODO: this format is part of the response from the SiteInfo API - we should use that value instead of hard-coding
                    // Converters = { new DateTimeConverter("dd/MM/yyyy, HH:mm:ss") }
                }),
                AuthorizationHeaderValueGetter = (rq,ct) => GetTokenAsync(apiKey)
            };
        }

        public ICourses Courses => ApiFor<ICourses>();
        public IUsers Users => ApiFor<IUsers>();
        public IGroups Groups => ApiFor<IGroups>();
        public IUnits Units => ApiFor<IUnits>();
        public ISiteInfo SiteInfo => ApiFor<ISiteInfo>();

        public async Task<string> GetTokenAsync(string apikey) => Base64($"{apikey}:");

        TApiInterface ApiFor<TApiInterface>() => RestService.For<TApiInterface>(_serverUrl, _refitSettings);
        static string Base64(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
}