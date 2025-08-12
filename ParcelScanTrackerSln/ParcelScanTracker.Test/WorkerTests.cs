using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using ParcelScanTracker.Common.Models;
using ParcelScanTracker.DAL;
using ParcelScanTracker.Worker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ParcelScanTracker.Test
{
    /// <summary>
    /// Tests for the Worker background service.
    /// Covers scan event processing and error handling.
    /// </summary>
    public class WorkerTests
    {
        // Mocks for Worker dependencies.
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<LogHelper> _mockLogHelper;
        private readonly Mock<IOptions<ScanEventApiOptions>> _mockApiOptions;
        private readonly ScanEventDbContext _dbContext;

        /// <summary>
        /// Sets up mocks and in-memory database for Worker tests.
        /// </summary>
        public WorkerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogHelper = new Mock<LogHelper>(Mock.Of<ILogger<LogHelper>>());
            _mockApiOptions = new Mock<IOptions<ScanEventApiOptions>>();

            // Mock API options
            _mockApiOptions.Setup(o => o.Value).Returns(new ScanEventApiOptions
            {
                BaseUrl = "http://localhost/v1/scans/scanevents",
                Limit = 2,
                PollingDelayMs = 10000
            });

            // Mock configuration for both indexer and GetValue
            //_mockConfiguration.Setup(c => c["Config:WorkerId"]).Returns("W_AKL");
            _mockConfiguration.Setup(c => "W_AKL");

            // Set up in-memory database
            var options = new DbContextOptionsBuilder<ScanEventDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ScanEventDbContext(options);

            // Mock service provider to return the in-memory DbContext
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(ScanEventDbContext))).Returns(_dbContext);
        }

        /// <summary>
        /// Verifies Worker processes scan events and logs info.
        /// </summary>
        [Fact]
        public async Task Worker_ProcessesScanEventsSuccessfully()
        {
            // Arrange
            _mockConfiguration.Setup(c => c.GetValue<string>("Config:WorkerId")).Returns("W_AKL");
            _mockConfiguration.Setup(c => c["Config:WorkerId"]).Returns("W_AKL");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"ScanEvents\":[{\"EventId\":1,\"ParcelId\":1001,\"Type\":\"PICKUP\",\"WorkerId\":\"W_AKL\"},{\"EventId\":2,\"ParcelId\":1002,\"Type\":\"DELIVERY\",\"WorkerId\":\"W_AKL\"}]}")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var worker = new ParcelScanTracker.Worker.Worker(
                _mockConfiguration.Object,
                _mockServiceProvider.Object,
                _mockApiOptions.Object,
                _mockHttpClientFactory.Object,
                _mockLogHelper.Object
            );

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await worker.StartAsync(cancellationToken);

            // Assert
            _mockLogHelper.Verify(log => log.WriteToLog(LogType.Info, It.IsAny<string>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies Worker logs error when API call fails.
        /// </summary>
        [Fact]
        public async Task Worker_LogsError_WhenApiCallFails()
        {
            // Arrange
            var worker = new ParcelScanTracker.Worker.Worker(
                _mockConfiguration.Object,
                _mockServiceProvider.Object,
                _mockApiOptions.Object,
                _mockHttpClientFactory.Object,
                _mockLogHelper.Object
            );

            var cancellationToken = new CancellationTokenSource().Token;

            // Simulate API failure
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Throws(new HttpRequestException("API call failed"));

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(() => worker.StartAsync(cancellationToken));

            // Assert
            _mockLogHelper.Verify(log => log.WriteToLog(LogType.Error, It.Is<string>(msg => msg.Contains("API call failed"))), Times.Once);
        }
    }
}
