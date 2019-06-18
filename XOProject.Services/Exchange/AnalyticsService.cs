using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;
using XOProject.Services.Domain;

namespace XOProject.Services.Exchange
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IShareRepository _shareRepository;

        public AnalyticsService(IShareRepository shareRepository)
        {
            _shareRepository = shareRepository;
        }

        DateTime SetEndOfDay(DateTime end)
        {
            TimeSpan ts = new TimeSpan(23, 59, 59);
            return end.Date + ts;
        }

        DateTime GetFirstWeekDay(int year, int weekNum)
        {
            Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);
            int firstMondayWeekNum = calendar.GetWeekOfYear(firstMonday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            DateTime firstWeekDay = firstMonday.AddDays((weekNum - firstMondayWeekNum) * 7);
            return firstWeekDay;
        }

        public async Task<AnalyticsPrice> GetSharePriceByDateRange(string symbol, DateTime start, DateTime end)
        {
            //TO DO [Immi]: certainly there should be a better way of optimizing this, 
            //now the profiler creates 4 seperate queries to get this data
            //Update 1: this is developed on top of EF COre: 2.1.1 - From 2.1.2 [Group by support is enabled in query - as per below link]
            //haven't validated yet:  Suported Link: https://stackoverflow.com/questions/52007446/linq-does-not-produce-count-query-on-group-using-ef-core/52009814#52009814
            //TO DO [Immi]:  But have to spend some time to make this perfomant..
            var q = _shareRepository.Query().AsNoTracking().Where(t => t.Symbol == symbol && t.TimeStamp >= start && t.TimeStamp <= end).OrderBy(t => t.TimeStamp);            try
            {
                return new AnalyticsPrice()
                {
                    Open = (await q.FirstAsync()).Rate,
                    Close = (await q.LastAsync()).Rate,
                    High = await q.MaxAsync(t => t.Rate),
                    Low = await q.MinAsync(t => t.Rate)
                };
            }
            catch (InvalidOperationException ex)
            {
                //TO DO [If Needed]: Filter this exception & manage in right exception block
                throw new ApplicationException($"NotFound: No data Found in the date range provided for {symbol}");
            }
        }

        public async Task<AnalyticsPrice> GetDailyAsync(string symbol, DateTime day)
        {
            return await GetSharePriceByDateRange(symbol, day, SetEndOfDay(day));
        }

        public async Task<AnalyticsPrice> GetWeeklyAsync(string symbol, int year, int week)
        {
            var start = GetFirstWeekDay(year, week);
            var end = SetEndOfDay(start.AddDays(7));
            return await GetSharePriceByDateRange(symbol, start, end);
        }

        public async Task<AnalyticsPrice> GetMonthlyAsync(string symbol, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = SetEndOfDay(new DateTime(year, month, DateTime.DaysInMonth(year, month)));
            return await GetSharePriceByDateRange(symbol, start, end);
        }
    }
}