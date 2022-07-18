using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityCore.Data;
using Quartz;
using Quartz.Impl;


namespace OnlineShop.Auctions
{
    public class AuctionJobScheduler
    {

        public async void Start()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<CheckClosedAuctionsJob>().Build();

            /* StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
              */
            ITrigger trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
