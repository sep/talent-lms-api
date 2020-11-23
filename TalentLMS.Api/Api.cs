using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    DateFormatString = "dd/MM/yyyy, HH:mm:ss",
                }),
                AuthorizationHeaderValueGetter = () => Task.FromResult(AuthHeader(apiKey))
            };
        }

        public ICourses Courses => ApiFor<ICourses>();
        public IUsers Users => ApiFor<IUsers>();
        public IGroups Groups => ApiFor<IGroups>();
        public ISiteInfo SiteInfo => ApiFor<ISiteInfo>();

        TApiInterface ApiFor<TApiInterface>() => RestService.For<TApiInterface>(_serverUrl, _refitSettings);
        static string AuthHeader(string apiKey) => Base64($"{apiKey}:");
        static string Base64(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
}