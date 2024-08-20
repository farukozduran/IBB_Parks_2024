using IBB.Nesine.Services.Services;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

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
