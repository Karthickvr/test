using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using XOProject.Api.Model.Analytics;
using XOProject.Services.Domain;
using XOProject.Services.Exchange;

using Microsoft.AspNetCore.Mvc;

namespace XOProject.Api.Controller
{
    [Route("api")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("daily/{symbol}/{year}/{month}/{day}")]
        public async Task<IActionResult> Daily([FromRoute] string symbol, [FromRoute] int year, [FromRoute] int month, [FromRoute] int day)
        {
            //try
            //{
            var prices = await _analyticsService.GetDailyAsync(symbol, new DateTime(year, month, day));
            var result = new DailyModel()
            {
                Symbol = symbol,
                Day = new DateTime(year, month, day),
                Price = Map(prices)
            };
            return Ok(result);
            //}
            //catch (ApplicationException exp)
            //{
            //    if (exp.Message.Contains("NotFound:"))
            //        return NotFound(exp.Message);
            //    return BadRequest(exp.Message);
            //}
            //catch (Exception exp) //TO DO [Done]: Move this exception moved to middleware extension
            //{
            //    return BadRequest(exp.Message);
            //}
        }

        [HttpGet("weekly/{symbol}/{year}/{week}")]
        public async Task<IActionResult> Weekly([FromRoute] string symbol, [FromRoute] int year, [FromRoute] int week)
        {
            try
            {
                var result = new WeeklyModel()
                {
                    Symbol = symbol,
                    Year = year,
                    Week = week,
                    Price = Map(await _analyticsService.GetWeeklyAsync(symbol, year, week))
                };
                return Ok(result);
            }
            //This can be removed [Managing in exception middleware] 
            //- Keeping this, to validate test case 
            catch (ApplicationException exp)
            {
                if (exp.Message.Contains("NotFound:"))
                    return NotFound(exp.Message);
                return BadRequest(exp.Message);
            }
        }

        [HttpGet("monthly/{symbol}/{year}/{month}")]
        public async Task<IActionResult> Monthly([FromRoute] string symbol, [FromRoute] int year, [FromRoute] int month)
        {
            var result = new MonthlyModel()
            {
                Symbol = symbol,
                Year = year,
                Month = month,
                Price = Map(await _analyticsService.GetMonthlyAsync(symbol, year, month))
            };
            return Ok(result);
        }

        private PriceModel Map(AnalyticsPrice price)
        {
            return new PriceModel()
            {
                Open = price.Open,
                Close = price.Close,
                High = price.High,
                Low = price.Low
            };
        }
    }
}