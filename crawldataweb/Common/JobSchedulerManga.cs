using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crawldataweb.Common
{
    public class JobSchedulerManga
    {
        public static void Start()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();



            IJobDetail job = JobBuilder.Create<ScheduleManga>().Build();

           

            ITrigger trigger = TriggerBuilder.Create()


                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInHours(24)
                     
                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 49))

                  )

                .Build();



            scheduler.ScheduleJob(job, trigger);
           

        }
    }
}