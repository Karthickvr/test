using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using XOProject.Api.Controller;
using XOProject.Api.Model;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Domain;
using XOProject.Services.Exchange;

namespace XOProject.Api.Tests
{
    public class AnalyticalControllerTest
    {
        private readonly Mock<IAnalyticsService> _analyticServiceMock = new Mock<IAnalyticsService>();

        private readonly Controller.AnalyticsController _analyticsController;

        public AnalyticalControllerTest()
        {
            _analyticsController = new Controller.AnalyticsController(_analyticServiceMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _analyticServiceMock.Reset();
        }

        [Test]
        public void Get_DailySharePriceRange_InvalidDateFormat_ShouldThrowArgumentError()
        {
            string symbol = "CBI";
            int year = 2018, month = 228, day = 13;

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _analyticsController.Daily(symbol, year, month, day));

            year = 0; month = 0; day = 13;
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _analyticsController.Daily(symbol, year, month, day));
        }


        [Test]
        public async Task Get_DailySharePriceRange()
        {
            string symbol = "CBI";
            int year = 2018, month = 8, day = 13;
            //Arrage
            _analyticServiceMock.Setup(t => t.GetDailyAsync(symbol, new DateTime(year, month, day)))
                .Returns(Task.FromResult(new AnalyticsPrice()));

            // Act
            var result = await _analyticsController.Daily(symbol, year, month, day);

            // Assert
            Assert.NotNull(result);

            var okresult = result as OkObjectResult;
            Assert.NotNull(okresult);
            Assert.AreEqual(200, okresult.StatusCode); 
        }

        [Test]
        public async Task Get_WeeklySharePriceRange()
        {
            string symbol = "CBI";
            int year = 2018, weekofyear = 8;
            //Arrage
            _analyticServiceMock.Setup(t => t.GetWeeklyAsync(symbol, year, weekofyear))
                .Returns(Task.FromResult(new AnalyticsPrice()));

            // Act
            var result = await _analyticsController.Weekly(symbol, year, weekofyear);

            // Assert
            Assert.NotNull(result);

            var okresult = result as OkObjectResult;
            Assert.NotNull(okresult);
            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task Get_MonthlySharePriceRange()
        {
            string symbol = "CBI";
            int year = 2018, month = 8;
            //Arrage
            _analyticServiceMock.Setup(t => t.GetMonthlyAsync(symbol, year, month))
                .Returns(Task.FromResult(new AnalyticsPrice()));

            // Act
            var result = await _analyticsController.Monthly(symbol, year, month);

            // Assert
            Assert.NotNull(result);

            var okresult = result as OkObjectResult;
            Assert.NotNull(okresult);
            Assert.AreEqual(200, okresult.StatusCode);
        }
    }
}
