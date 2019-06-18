using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using XOProject.Api.Controller;
using XOProject.Api.Model;
using XOProject.Repository.Domain;
using XOProject.Services.Exchange;

namespace XOProject.Api.Tests
{
    public class TradeControllerTest
    {
        private readonly Mock<ITradeService> _tradeServiceMock = new Mock<ITradeService>();

        private readonly TradeController _tradeController;

        public TradeControllerTest()
        {
            _tradeController = new TradeController(_tradeServiceMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _tradeServiceMock.Reset();
        }

        [Test]
        public async Task GetAllTradings_NotFound()
        {
            // Arrange
            TradeModel value = new TradeModel() { NoOfShares = 2, PortfolioId = 1, Symbol = "WWW", Action = "SELL" };

            _tradeServiceMock.Setup(t => t.GetByPortfolioId(value.PortfolioId))
                .Returns(Task.FromResult((IList<Trade>)new List<Trade>() { }));

            // Act
            var result = await _tradeController.GetAllTradings(value.PortfolioId);

            // Assert
            Assert.NotNull(result);
            var badresult = result as NotFoundResult;
            Assert.AreEqual(404, badresult.StatusCode);
        }

        [Test]
        public async Task GetAllTradings_BadRequest()
        {
            // Arrange
            TradeModel value = new TradeModel() { NoOfShares = 2, PortfolioId = -1, Symbol = "WWW", Action = "SELL" };

            _tradeServiceMock.Setup(t => t.GetByPortfolioId(value.PortfolioId))
                .Returns(Task.FromResult((IList<Trade>)new List<Trade>() { }));

            // Act
            var result = await _tradeController.GetAllTradings(value.PortfolioId);

            // Assert
            Assert.NotNull(result);
            var badresult = result as BadRequestResult;
            Assert.AreEqual(400, badresult.StatusCode);
        }

        [Test]
        public async Task GetAllTradings_OK()
        {
            // Arrange
            TradeModel value = new TradeModel() { NoOfShares = 2, PortfolioId = 1, Symbol = "WWW", Action = "SELL" };

            _tradeServiceMock.Setup(t => t.GetByPortfolioId(value.PortfolioId))
                .Returns(Task.FromResult((IList<Trade>)new List<Trade>()
                {
                    new Trade()
                    {

                    }
                }));

            // Act
            var result = await _tradeController.GetAllTradings(value.PortfolioId);

            // Assert
            Assert.NotNull(result);
            var okresult = result as OkObjectResult;
            Assert.AreEqual(200, okresult.StatusCode);
        }

        [Test]
        public async Task PostTrade_Create()
        {
            // Arrange
            TradeModel value = new TradeModel() { NoOfShares = 2, PortfolioId = 1, Symbol = "WWW", Action = "SELL" };

            _tradeServiceMock.Setup(t => t.BuyOrSell(value.PortfolioId, value.Symbol, value.NoOfShares, value.Action))
                .Returns(Task.FromResult(new Trade() { }));

            // Act
            var result = await _tradeController.Post(value);

            // Assert
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public void PostTrade_ValidTradeModel_Success()
        {
            var sut = new TradeModel() { NoOfShares = 2, Symbol = "EEE", Action = "SELL", PortfolioId = 1 };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);
            Assert.IsTrue(isModelStateValid);

        }

        [Test]
        public void PostTrade_InvalidModel_ThrowsError()
        {
            var sut = new TradeModel() { NoOfShares = 0, Symbol = "EEER", Action = "SELL1", PortfolioId = 0 };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);
            //Console.WriteLine(string.Join(',', new object[] { results.ToArray() }));
            Assert.IsFalse(isModelStateValid);
        }
    }
}
