using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Services
{
    public class JobScheduler
    {
       
        public static void ScheduleJobs()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<FacebookJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("fb", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(40)
                    .RepeatForever())
                    .Build();

            IJobDetail twt = JobBuilder.Create<TwitterJob>().Build();

            ITrigger twtTrigger = TriggerBuilder.Create()
                    .WithIdentity("twt", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(40)
                    .RepeatForever())
                    .Build();

            IJobDetail eml = JobBuilder.Create<EmailJobs>().Build();

            ITrigger emlTrigger = TriggerBuilder.Create()
                    .WithIdentity("eml", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(40)
                    .RepeatForever())
                    .Build();

            IJobDetail sms = JobBuilder.Create<SMSJob>().Build();
            ITrigger smsTrigger = TriggerBuilder.Create()
                    .WithIdentity("sms", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(40)
                    .RepeatForever())
                    .Build();

            
            /*
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>s.WithIntervalInSeconds(20)
                     //s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();
                */
            scheduler.ScheduleJob(job, trigger);
            scheduler.ScheduleJob(sms, smsTrigger);
            scheduler.ScheduleJob(twt, twtTrigger);
            scheduler.ScheduleJob(eml, emlTrigger);
        }
    }
        
}