using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crawldataweb.Common
{
    public class JobScheduleChap
    {
        public static void Start()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();



           

            IJobDetail jobChap = JobBuilder.Create<ScheduleChap>().Build();

            ITrigger trigger = TriggerBuilder.Create()


                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInHours(24)

                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(00, 00))

                  )

                .Build();


            scheduler.ScheduleJob(jobChap, trigger);

        }
    }
}