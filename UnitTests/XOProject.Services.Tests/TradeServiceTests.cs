using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Exchange;

namespace XOProject.Services.Tests
{
    public class TradeServiceTests
    {

        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly ShareService _shareService;

        private readonly Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();

        private readonly TradeService _tradeService;

        public TradeServiceTests()
        {
            _shareService = new ShareService(_shareRepositoryMock.Object);
            _tradeService = new TradeService(_tradeRepositoryMock.Object, _shareService);
        }

        [TearDown]
        public void Cleanup()
        {
            _tradeRepositoryMock.Reset();
            _shareRepositoryMock.Reset();
        }

        [Test]
        public async Task Trade_BuyOrSell_TestValue()
        {
            // Arrange
            ArrangeShareRates(); // Share Mock

            _tradeRepositoryMock //Trade Mock
                .Setup(mock => mock.Query())
                .Returns(new Helpers.AsyncQueryResult<Trade>(new List<Trade>()
                {
                    new Trade() { PortfolioId = 1 }
                }));

            // Act
            var result = await _tradeService.BuyOrSell(1, "CBI", 2, "BUY");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.PortfolioId);
        }

        [Test]
        public async Task GetByPortfolioId_TestValue()
        {
            // Arrange
            _tradeRepositoryMock
                .Setup(mock => mock.Query())
                .Returns(new Helpers.AsyncQueryResult<Trade>(new List<Trade>()
                {
                    new Trade() { PortfolioId = 1 }
                }));

            // Act
            var result = await _tradeService.GetByPortfolioId(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        private void ArrangeShareRates()
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
                .Returns(new Helpers.AsyncQueryResult<HourlyShareRate>(rates));
        }
    }
}
