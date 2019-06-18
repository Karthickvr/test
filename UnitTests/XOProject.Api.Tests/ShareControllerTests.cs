using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using XOProject.Api.Controller;
using XOProject.Api.Model;
using XOProject.Repository;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Exchange;

namespace XOProject.Api.Tests
{
    //Tried Intergtion with 2 Layer of Mocking - successfull with enabling virtual - but not right
    //TO DO: make a clean Interface implementation to make these integration work
    //Just leaving as it is..
    public class MockShareRepo : ShareRepository
    {
        public MockShareRepo() : base(new Repository.ExchangeContext(null))
        { }

        protected IGenericRepository<HourlyShareRate> EntityRepository { get; }
    }

    public class MockShareService : ShareService
    {
        public MockShareService(MockShareRepo repo) : base(repo)
        {

        }
    }

    public class ShareControllerTests
    {
        private readonly Mock<IShareService> _shareServiceMock = new Mock<IShareService>();

        private readonly ShareController _shareController;

        public ShareControllerTests()
        {
            _shareController = new ShareController(_shareServiceMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _shareServiceMock.Reset();
        }

        [Test]
        public async Task GetSharePrices_ShouldHaveData()
        {
            // Arrange

            string symbol = "CBI";

            _shareServiceMock.Setup(t => t.GetBySymbolAsync(symbol))
                .Returns(Task.FromResult((IList<HourlyShareRate>)new List<HourlyShareRate>()
                {
                    new HourlyShareRate()
                    {

                    }
                }));

            // Act
            var result = await _shareController.Get(symbol);

            // Assert
            Assert.NotNull(result);

            var okresult = result as OkObjectResult;
            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task GetSharePrices_NoDataFound()
        {
            // Arrange

            string symbol = "CBI";

            _shareServiceMock.Setup(t => t.GetBySymbolAsync(symbol))
                .Returns(Task.FromResult((IList<HourlyShareRate>)new List<HourlyShareRate>()
                {
                }));

            // Act
            var result = await _shareController.Get(symbol);

            // Assert
            Assert.NotNull(result);

            var notFoundResult = result as NotFoundResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Put_UpdateLastHourlySharePrice_NotExist_NotFoundResult()
        {
            // Arrange
            var hourRate = new HourlyShareRateModel
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2019, 04, 07, 5, 0, 0)
            };

            HourlyShareRate ret = null;

            _shareServiceMock.Setup(t => t.UpdateLastPriceAsync(hourRate.Symbol, hourRate.Rate))
                .Returns(Task.FromResult(ret));

            // Act
            var result = await _shareController.UpdateLastPrice(hourRate.Symbol, hourRate.Rate);

            // Assert
            Assert.NotNull(result);

            var notFoundResult = result as NotFoundResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Put_ShouldUpdateLastHourlySharePrice()
        {
            // Arrange
            var hourRate = new HourlyShareRateModel
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2019, 04, 07, 5, 0, 0)
            };

            _shareServiceMock.Setup(t => t.UpdateLastPriceAsync(hourRate.Symbol, hourRate.Rate))
                .Returns(Task.FromResult(Map(hourRate)));

            // Act
            var result = await _shareController.UpdateLastPrice(hourRate.Symbol, hourRate.Rate);

            // Assert
            Assert.NotNull(result);
            var okresult = result as OkObjectResult;
            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task Put_ShouldUpsertHourlySharePrice()
        {
            // Arrange
            var hourRate = new HourlyShareRateModel
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2019, 04, 07, 5, 0, 0)
            };

            _shareServiceMock.Setup(t => t.UpsertHourlyPriceAsync(Map(hourRate)))
                .Returns(Task.FromResult(new Random().Next(200, 201)));

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            var okresult = result as OkObjectResult;
            Assert.True(((createdResult != null && createdResult.StatusCode == 201) || (okresult != null && okresult.StatusCode == 200)), "Put Used to Insert or Update the Hourly Price");
        }

        [Test]
        public async Task Post_ShouldInsertHourlySharePrice()
        {
            // Arrange
            var hourRate = new HourlyShareRateModel
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2019, 04, 07, 5, 0, 0)
            };

            _shareServiceMock.Setup(t => t.InsertAsync(Map(hourRate)))
                .Returns(Task.FromResult(0));

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            // TODO: This unit test is broken, the result received from the Post method is correct.
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode); //This is the FIX
        }

        private HourlyShareRateModel Map(HourlyShareRate rate)
        {
            return new HourlyShareRateModel()
            {
                Id = rate.Id,
                TimeStamp = rate.TimeStamp,
                Rate = rate.Rate,
                Symbol = rate.Symbol
            };
        }

        private HourlyShareRate Map(HourlyShareRateModel rate)
        {
            return new HourlyShareRate()
            {
                Id = rate.Id,
                TimeStamp = rate.TimeStamp,
                Rate = rate.Rate,
                Symbol = rate.Symbol
            };
        }
    }
}
