using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XOProject.Repository.Domain;
using XOProject.Repository.Exchange;

namespace XOProject.Services.Exchange
{
    public class ShareService : GenericService<HourlyShareRate>, IShareService
    {
        public ShareService(IShareRepository shareRepository) : base(shareRepository)
        {
        }

        public async Task<IList<HourlyShareRate>> GetBySymbolAsync(string symbol)
        {
            return await EntityRepository
                .Query()
                .Where(x => x.Symbol.Equals(symbol))
                .ToListAsync();
        }

        public async Task<HourlyShareRate> GetHourlyAsync(string symbol, DateTime dateAndHour)
        {
            var date = dateAndHour.Date;
            var hour = dateAndHour.Hour;

            return await EntityRepository
                .Query()
                .Where(x => x.Symbol.Equals(symbol)
                            && x.TimeStamp.Date == date
                            && x.TimeStamp.Hour == hour)
                .FirstOrDefaultAsync();
        }

        public async Task<HourlyShareRate> GetLastPriceAsync(string symbol)
        {
            var share = await EntityRepository
                .Query()
                .Where(x => x.Symbol.Equals(symbol))
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefaultAsync();
            return share;
        }

        public async Task<HourlyShareRate> UpdateLastPriceAsync(string symbol, decimal rate)
        {
            //BUG: The team identified shares with unexpected rates which do not match with data from other sources
            //Explanation: Based on the requirment 
            //-> The project registers share and allows admins to add the new price of the share on an hourly basis and update last price value at any time.
            //I assume the Fix is: desc by TimeStamp & Not rate
            var share = await EntityRepository
                .Query()
                .Where(x => x.Symbol.Equals(symbol))
                //.OrderByDescending(x => x.Rate)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefaultAsync();

            if (share == null)
            {
                return null;
            }

            share.Rate = rate;
            await EntityRepository.UpdateAsync(share);

            return share;
        }

        public async Task<HourlyShareRate> GetPriceAtHourAsync(HourlyShareRate hourlyShareRate)
        {
            return await EntityRepository
                       .Query()
                       .Where(x => x.Symbol.Equals(hourlyShareRate.Symbol)
                                   && x.TimeStamp.Date == hourlyShareRate.TimeStamp.Date
                                   && x.TimeStamp.Hour == hourlyShareRate.TimeStamp.Hour)
                       .FirstOrDefaultAsync();
        }

        public async Task InserttHourlyPriceAsync(HourlyShareRate hourlyShareRate)
        {
            var share = await EntityRepository
                       .Query()
                       .Where(x => x.Symbol.Equals(hourlyShareRate.Symbol)
                                   && x.TimeStamp.Date == hourlyShareRate.TimeStamp.Date
                                   && x.TimeStamp.Hour == hourlyShareRate.TimeStamp.Hour)
                       .FirstOrDefaultAsync();
            if (share == null)
            {
                await InsertAsync(hourlyShareRate);
            }
            else
            {
                throw new ApplicationException("Price already entered for the time specified.");
            }
        }

        //TO DO: timebeing returning httpcode, should be rightly wrapped to diff update & insert
        //enabling put with this upsert operation
        public async Task<int> UpsertHourlyPriceAsync(HourlyShareRate hourlyShareRate)
        {
            var share = await EntityRepository
                       .Query()
                       .Where(x => x.Symbol.Equals(hourlyShareRate.Symbol)
                                   && x.TimeStamp.Date == hourlyShareRate.TimeStamp.Date
                                   && x.TimeStamp.Hour == hourlyShareRate.TimeStamp.Hour)
                       .FirstOrDefaultAsync();
            if (share == null)
            {
                await InsertAsync(hourlyShareRate);
                return 201;
            }
            else
            {
                share.Rate = hourlyShareRate.Rate;
                hourlyShareRate.Id = share.Id;
                await UpdateAsync(share);
                return 200;
            }
        }
    }
}
