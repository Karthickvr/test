using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Exchange;
using XOProject.Services.Tests.Helpers;

namespace XOProject.Services.Tests
{
    public class AnalyticalServiceTest
    {
        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly AnalyticsService _analyticalService;

        public AnalyticalServiceTest()
        {
            _analyticalService = new AnalyticsService(_shareRepositoryMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _shareRepositoryMock.Reset();
        }


        [Test]
        public async Task GetAnalyticalAsync_ForOneMonth_GetSharePrice()
        {
            ArrangeRates();
            var result = await _analyticalService.GetMonthlyAsync("CBI", 2018, 08);
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400m, result.High);
        }

        [Test]
        public void GetAnalyticalAsync_ForOneWeek_WhenNoData_ThrowValidError()
        {
            ArrangeRates();
            Assert.ThrowsAsync<ApplicationException>(async () => await _analyticalService.GetWeeklyAsync("CBI", 2018, 4));
            //Assert.Fail("NotFound: No data Found in the date range provided for CBI");
        }

        [Test]
        public async Task GetAnalyticalAsync_ForOneWeek_GetWeeklyAsync()
        {
            ArrangeRates();
            var result = await _analyticalService.GetWeeklyAsync("CBI", 2018, 33);
            Assert.NotNull(result);
            Assert.AreEqual(400m, result.High);
        }

        [Test]
        public async Task GetAnalyticalAsync_ForOneWeek_GetSharePrice()
        {
            ArrangeRates();
            var result = await _analyticalService.GetWeeklyAsync("CBI", 2018, 33);
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400m, result.High);
        }

        [Test]
        public async Task GetAnalyticalAsync_ForOneDay_GetSharePrice()
        {
            ArrangeRates();
            var result = await _analyticalService.GetDailyAsync("CBI", new DateTime(2018, 08, 17, 0, 0, 0));
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400m, result.High);
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
