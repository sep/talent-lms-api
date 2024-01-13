using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using TalentLMS.Api.Courses;
using Microsoft.Extensions.DependencyInjection;

namespace TalentLMS.Api
{

    public partial interface ITalentApi
    {
        HttpClient Client { get; }  // refit magic
    }

    public class TalentApi {

        private readonly string _talentLmsApiRoot;
        private readonly string _apiKey;

        public TalentApi(string talentLmsApiRoot, string apiKey)
        {
            _talentLmsApiRoot = talentLmsApiRoot;
            _apiKey = apiKey;
        }


        public ITalentApi TestClient()
        {
            //var httpClient = new HttpClient(new AuthHeaderHandler(_apiKey))
            //{
            //    BaseAddress = new Uri(_talentLmsApiRoot),
            //};

            var handler = new HttpClientHandler();

            var handler2 = new AuthHeaderHandler(_apiKey);

            var basicAuthHeader = new AuthenticationHeaderValue("Basic", _apiKey);

            var settings = new RefitSettings()
            {
                HttpMessageHandlerFactory = () => handler,
                AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(basicAuthHeader.ToString())
            };
            var client = RestService.For<ITalentApi>(_talentLmsApiRoot, settings);
           
            return client;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(serviceProvider => new AuthHeaderHandler(_apiKey));
            services.AddRefitClient<ITalentApi>().ConfigureHttpClient(services =>
            {
                services.BaseAddress = new Uri(_talentLmsApiRoot);
            }).AddHttpMessageHandler<AuthHeaderHandler>();

        }
    }

    class AuthHeaderHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        public AuthHeaderHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _apiKey);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}