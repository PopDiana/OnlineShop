using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.RecommenderSystem
{
    public class RecommenderJobScheduler
    {
        public async void Start()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<ComputeRecommenderMatrixJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                  .StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
