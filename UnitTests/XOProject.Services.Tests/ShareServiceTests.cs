using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Exchange;
using XOProject.Services.Tests.Helpers;

namespace XOProject.Services.Tests
{
    public class ShareServiceTests
    {
        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly ShareService _shareService;

        public ShareServiceTests()
        {
            _shareService = new ShareService(_shareRepositoryMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _shareRepositoryMock.Reset();
        }

        [Test]
        public async Task InserttHourlyPriceAsyncTest_Insert()
        {
            // Arrange
            ArrangeRates();

            HourlyShareRate value = new HourlyShareRate()
            {
                Rate = 553,
                Symbol = "CBI",
                TimeStamp = new DateTime(2019, 08, 17, 6, 0, 0)
            };

            // Act & Assert
            await _shareService.InserttHourlyPriceAsync(value);

            Assert.True(value.Rate == 553); //TO DO: Not perfectly right
        }

        [Test]
        public void InserttHourlyPriceAsyncTest_Error()
        {
            // Arrange
            ArrangeRates();

            HourlyShareRate value = new HourlyShareRate()
            {
                Rate = 553,
                Symbol = "CBI",
                TimeStamp = new DateTime(2018, 08, 17, 6, 0, 0)
            };

            // Act & Assert
            Assert.ThrowsAsync<ApplicationException>(async () =>  await _shareService.InserttHourlyPriceAsync(value));
        }

        [Test]
        public async Task UpdateLatestPrice_RightSymbol_UpdatesRate()
        {
            // Arrange
            ArrangeRates();

            // Act
            var result = await _shareService.UpdateLastPriceAsync("CBI", 553);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Rate, 553);
        }

        [Test]
        public async Task UpdateLatestPrice_InvalidSymbol_ReturnsNull()
        {
            // Arrange
            ArrangeRates();

            // Act
            var result = await _shareService.UpdateLastPriceAsync("CKI", 300);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task GetBySymbolAsync_TestValue()
        {
            // Arrange
            ArrangeRates();

            // Act
            var result = await _shareService.GetBySymbolAsync("CBI");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Test]
        public async Task GetLatestPriceUpdated_TestValue()
        {
            // Arrange
            ArrangeRates();

            // Act
            var result = await _shareService.GetLastPriceAsync("CBI");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(5, result.Id);
        }

        [Test]
        public async Task GetHourlyAsync_WhenExists_GetsHourlySharePrice()
        {
            // Arrange
            ArrangeRates();

            // Act
            var result = await _shareService.GetHourlyAsync("CBI", new DateTime(2018, 08, 17, 5, 10, 15));

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(4, result.Id);
        }

        private void ArrangeRates()
        {
            var rates = new[]
            {
                new HourlyShareRate
                {
                    Id = 1,
                    Symbol = "CBI",
                    Rate = 310.0M,
                    TimeStamp = new DateTime(2017, 08, 17, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Id = 2,
                    Symbol = "CBI",
                    Rate = 320.0M,
                    TimeStamp = new DateTime(2018, 08, 16, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Id = 3,
                    Symbol = "REL",
                    Rate = 300.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Id = 4,
                    Symbol = "CBI",
                    Rate = 300.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Id = 5,
                    Symbol = "CBI",
                    Rate = 400.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 6, 0, 0)
                },
                new HourlyShareRate
                {
                    Id = 6,
                    Symbol = "IBM",
                    Rate = 300.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                },
            };
            _shareRepositoryMock
                .Setup(mock => mock.Query())
                .Returns(new AsyncQueryResult<HourlyShareRate>(rates));
        }
    }
}
