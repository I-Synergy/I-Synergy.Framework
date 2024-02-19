using ISynergy.Framework.AspNetCore.Enumerations;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.AspNetCore.Tests.Fixture;
using ISynergy.Framework.AspNetCore.Tests.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace ISynergy.Framework.AspNetCore.Middleware.Tests;

/// <summary>
/// Class MaxConcurrentRequestsTests.
/// </summary>
[TestClass]
public class MaxConcurrentRequestsTests
{
    /// <summary>
    /// Struct HttpResponseInformation
    /// </summary>
    private struct HttpResponseInformation
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the timing.
        /// </summary>
        /// <value>The timing.</value>
        public TimeSpan Timing { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"StatusCode: {StatusCode} | Timing {Timing}";
        }
    }

    /// <summary>
    /// The default response
    /// </summary>
    private const string DEFAULT_RESPONSE = "-- MaxConcurrentConnections --";

    /// <summary>
    /// Some concurrent requests count
    /// </summary>
    private const int SOME_CONCURRENT_REQUESTS_COUNT = 30;
    /// <summary>
    /// Some maximum concurrent requests limit
    /// </summary>
    private const int SOME_MAX_CONCURRENT_REQUESTS_LIMIT = 10;
    /// <summary>
    /// Some maximum queue length
    /// </summary>
    private const int SOME_MAX_QUEUE_LENGTH = 10;
    /// <summary>
    /// The time shorter than processing
    /// </summary>
    private const int TIME_SHORTER_THAN_PROCESSING = 300;

    /// <summary>
    /// Prepares the test server.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>TestServer.</returns>
    private TestServer PrepareTestServer(IEnumerable<KeyValuePair<string, string>> configuration = null)
    {
        IWebHostBuilder webHostBuilder = new WebHostBuilder()
            .UseStartup<StartupFixture>();

        if (configuration is not null)
        {
            ConfigurationBuilder configurationBuilder = new();
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

    /// <summary>
    /// Defines the test method SingleRequest_ReturnsSuccessfulResponse.
    /// </summary>
    [TestMethod]
    public async Task SingleRequest_ReturnsSuccessfulResponse()
    {
        using TestServer server = PrepareTestServer();
        using HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    /// <summary>
    /// Defines the test method SingleRequest_ReturnsDefaultResponse.
    /// </summary>
    [TestMethod]
    public async Task SingleRequest_ReturnsDefaultResponse()
    {
        using TestServer server = PrepareTestServer();
        using HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/");
        string responseText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(DEFAULT_RESPONSE, responseText);
    }

    /// <summary>
    /// Defines the test method SomeMaxConcurrentRequestsLimit_Drop_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable.
    /// </summary>
    [TestMethod]
    public void SomeMaxConcurrentRequestsLimit_Drop_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable()
    {
        Dictionary<string, string> configuration = new()
        {
            {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
            {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.Drop.ToString() }
        };

        HttpResponseInformation[] responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

        Assert.AreEqual(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
    }

    /// <summary>
    /// Defines the test method SomeMaxConcurrentRequestsLimit_FifoQueueDropTail_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable.
    /// </summary>
    [TestMethod]
    public void SomeMaxConcurrentRequestsLimit_FifoQueueDropTail_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable()
    {
        Dictionary<string, string> configuration = new()
        {
            {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
            {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropTail.ToString() },
            {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() }
        };

        HttpResponseInformation[] responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

        Assert.AreEqual(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT - SOME_MAX_QUEUE_LENGTH, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
    }

    /// <summary>
    /// Defines the test method SomeMaxConcurrentRequestsLimit_FifoQueueDropHead_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable.
    /// </summary>
    [TestMethod]
    public void SomeMaxConcurrentRequestsLimit_FifoQueueDropHead_SomeMaxQueueLength_SomeConcurrentRequestsCount_CountMinusLimitRequestsAndMaxQueueLengthReturnServiceUnavailable()
    {
        Dictionary<string, string> configuration = new()
        {
            {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
            {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropHead.ToString() },
            {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() }
        };

        HttpResponseInformation[] responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

        Assert.AreEqual(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT - SOME_MAX_QUEUE_LENGTH, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
    }

    /// <summary>
    /// Defines the test method SomeMaxConcurrentRequestsLimit_Queue_SomeMaxQueueLength_MaxTimeInQueueShorterThanProcessing_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable.
    /// </summary>
    [TestMethod]
    public void SomeMaxConcurrentRequestsLimit_Queue_SomeMaxQueueLength_MaxTimeInQueueShorterThanProcessing_SomeConcurrentRequestsCount_CountMinusLimitRequestsReturnServiceUnavailable()
    {
        Dictionary<string, string> configuration = new()
        {
            {"MaxConcurrentRequestsOptions:Limit", SOME_MAX_CONCURRENT_REQUESTS_LIMIT.ToString() },
            {"MaxConcurrentRequestsOptions:LimitExceededPolicy", MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropTail.ToString() },
            {"MaxConcurrentRequestsOptions:MaxQueueLength", SOME_MAX_QUEUE_LENGTH.ToString() },
            {"MaxConcurrentRequestsOptions:MaxTimeInQueue", TIME_SHORTER_THAN_PROCESSING.ToString() }
        };

        HttpResponseInformation[] responseInformation = GetResponseInformation(configuration, SOME_CONCURRENT_REQUESTS_COUNT);

        Assert.AreEqual(SOME_CONCURRENT_REQUESTS_COUNT - SOME_MAX_CONCURRENT_REQUESTS_LIMIT, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
    }

    /// <summary>
    /// Gets the response information.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="concurrentRequestsCount">The concurrent requests count.</param>
    /// <returns>HttpResponseInformation[].</returns>
    private HttpResponseInformation[] GetResponseInformation(Dictionary<string, string> configuration, int concurrentRequestsCount)
    {
        HttpResponseInformation[] responseInformation;

        using (TestServer server = PrepareTestServer(configuration))
        {
            List<HttpClient> clients = [];
            for (int i = 0; i < concurrentRequestsCount; i++)
            {
                clients.Add(server.CreateClient());
            }

            List<Task<HttpResponseMessageWithTiming>> responsesWithTimingsTasks = [];

            foreach (HttpClient client in clients)
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
