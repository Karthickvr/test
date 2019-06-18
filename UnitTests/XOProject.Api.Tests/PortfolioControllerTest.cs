using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XOProject.Api.Controller;
using XOProject.Api.Model;
using XOProject.Repository.Domain;
using XOProject.Services.Exchange;

namespace XOProject.Api.Tests
{
    public class PortfolioControllerTest
    {
        private readonly Mock<IPortfolioService> _shareServiceMock = new Mock<IPortfolioService>();

        private readonly PortfolioController _portfolioController;

        public PortfolioControllerTest()
        {
            _portfolioController = new PortfolioController(_shareServiceMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _shareServiceMock.Reset();
        }

        [Test]
        public async Task PostPortfolio_Create()
        {
            // Arrange

            Portfolio value = new Portfolio() { Id = 2 };
            _shareServiceMock.Setup(t => t.InsertAsync(value))
                .Returns(Task.FromResult(new Portfolio() { }));

            // Act
            var result = await _portfolioController.Post(Map(value));

            // Assert
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.AreEqual(201, createdResult.StatusCode); 
        }

        [Test]
        public async Task GetPortfolio_NotFound()
        {
            // Arrange

            int portid = 1;
            Portfolio port = null;
            _shareServiceMock.Setup(t => t.GetByIdAsync(portid))
                .Returns(Task.FromResult(port));

            // Act
            var result = await _portfolioController.GetPortfolio(portid);

            // Assert
            Assert.NotNull(result);

            var notFoundResult = result as NotFoundResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetPortfolio_OK()
        {
            // Arrange

            int portid = 1;

            _shareServiceMock.Setup(t => t.GetByIdAsync(portid))
                .Returns(Task.FromResult(new Portfolio()
                    {
                    
                    }));

            // Act
            var result = await _portfolioController.GetPortfolio(portid);

            // Assert
            Assert.NotNull(result);

            var okresult = result as OkObjectResult;
            Assert.AreEqual(200, okresult.StatusCode);
        }

        private PortfolioModel Map(Portfolio portfolio)
        {
            return new PortfolioModel()
            {
                Id = portfolio.Id,
                Name = portfolio.Name
            };
        }

        private Portfolio Map(PortfolioModel portfolio)
        {
            return new Portfolio()
            {
                Id = portfolio.Id,
                Name = portfolio.Name
            };
        }
    }
}
