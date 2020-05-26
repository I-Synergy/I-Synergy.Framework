using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.Enumerations;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.AspNetCore.Tests.Fixture;
using ISynergy.Framework.AspNetCore.Tests.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ISynergy.Framework.AspNetCore.Middleware.Tests
{
    public class MaxConcurrentRequestsTests
    {
        private struct HttpResponseInformation
        {
            public HttpStatusCode StatusCode { get; set; }

            public TimeSpan Timing { get; set; }

            public override string ToString()
            {
                return $"StatusCode: {StatusCode} | Timing {Timing}";
            }
        }

        private const string DEFAULT_RESPONSE = "-- MaxConcurrentConnections --";

        private const int SOME_CONCURRENT_REQUESTS_COUNT = 30;
        private const int SOME_MAX_CONCURRENT_REQUESTS_LIMIT = 10;
        private const int SOME_MAX_QUEUE_LENGTH = 10;
        private const int TIME_SHORTER_THAN_PROCESSING = 300;

        private TestServer PrepareTestServer(IEnumerable<KeyValuePair<string, string>> configuration = null)
        {
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseStartup<StartupFixture>();

            if (configuration != null)
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(configuration);
                IConfiguration buildedConfiguration = configurationBuilder.Build();

                webHostBuilder.UseConfiguration(buildedConfiguration);
                webHostBuilder.ConfigureServices((services) =>
                {
                    services.Configure<MaxConcurrentRequestsOptions>(options => buildedConfiguration.GetSection(nameof(MaxConcurrentRequestsOptions)).Bind(options));
                });
            }

            return new TestServer(webHostBuilder);
        }

        [Fact]
        public async Task SingleRequest_ReturnsSuccessfulResponse()
        {
            using (var server = PrepareTestServer())
            {
                using (var client = server.CreateClient())
                {
                    var response = await client.GetAsync("/");

                    Assert.True(response.IsSuccessStatusCode);
                }
            }
        }

        [Fact]
        public async Task SingleRequest_ReturnsDefaultResponse()
        {
            using (var server = PrepareTestServer())
            {
                using (var client = server.CreateClient())
                {
                    var response = await client.GetAsync("/");
                    var responseText = await response.Content.ReadAsStringAsync();

                    Assert.Equal(DEFAULT_RESPONSE, responseText);
                }
            }
        }

        [Fact]
        public void SomeMaxConcurrentRequestsLimit_Drop_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable()
        {
            var configuration = new Dictionary<string, string>
            {
                {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
                {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.Drop.ToString() }
            };

            var responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

            Assert.Equal(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
        }

        [Fact]
        public void SomeMaxConcurrentRequestsLimit_FifoQueueDropTail_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable()
        {
            var configuration = new Dictionary<string, string>
            {
                {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
                {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropTail.ToString() },
                {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() }
            };

            var responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

            Assert.Equal(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT - SOME_MAX_QUEUE_LENGTH, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
        }

        [Fact]
        public void SomeMaxConcurrentRequestsLimit_FifoQueueDropHead_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable()
        {
            var configuration = new Dictionary<string, string>
            {
                {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
                {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropHead.ToString() },
                {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() }
            };

            var responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

            Assert.Equal(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT - SOME_MAX_QUEUE_LENGTH, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
        }

        [Fact]
        public void SomeMaxConcurrentRequestsLimit_Queue_SomeMaxQueueLength_MaxTimeInQueueShorterThanProcessing_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable()
        {
            var configuration = new Dictionary<string, string>
            {
                {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
                {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropTail.ToString() },
                {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() },
                {"MaxConcurrentRequestsOptions:MaxTimeInQueue", TIME_SHORTER_THAN_PROCESSING.ToString() }
            };

            var responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

            Assert.Equal(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
        }

        private HttpResponseInformation[] GetResponseInformation(Dictionary<string, string> configuration, int concurrentRequestsCount)
        {
            HttpResponseInformation[] responseInformation;

            using (var server = PrepareTestServer(configuration))
            {
                var clients = new List<HttpClient>();
                for (var i = 0; i < concurrentRequestsCount; i++)
                {
                    clients.Add(server.CreateClient());
                }

                var responsesWithTimingsTasks = new List<Task<HttpResponseMessageWithTiming>>();

                foreach (var client in clients)
                {
                    responsesWithTimingsTasks.Add(Task.Run(async () => { return await client.GetWithTimingAsync("/"); }));
                }

                Task.WaitAll(responsesWithTimingsTasks.ToArray());

                clients.ForEach(client => client.Dispose());

                responseInformation = responsesWithTimingsTasks.Select(task => new HttpResponseInformation
                {
                    StatusCode = task.Result.Response.StatusCode,
                    Timing = task.Result.Timing
                }).ToArray();
            }

            return responseInformation;
        }
    }
}
