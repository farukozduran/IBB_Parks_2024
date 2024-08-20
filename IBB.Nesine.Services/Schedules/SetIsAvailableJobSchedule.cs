using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBB.Nesine.Services.Services;

namespace IBB.Nesine.Services.Schedules
{
    public class SetIsAvailableJobSchedule
    {
        public static async Task Start()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<SetIsAvailableJobService>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(2)
                .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
