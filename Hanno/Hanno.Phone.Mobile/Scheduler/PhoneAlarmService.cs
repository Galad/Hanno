using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Hanno.Scheduler;
using Microsoft.Phone.Scheduler;

namespace Hanno.Mobile.Scheduler
{
	public class PhoneAlarmService : IAlarmService
	{
		private readonly ISchedulerActionService _schedulerActionService;

		public PhoneAlarmService(ISchedulerActionService schedulerActionService)
		{
			if (schedulerActionService == null) throw new ArgumentNullException("schedulerActionService");
			_schedulerActionService = schedulerActionService;
		}

		public void Add(AlarmAction alarm)
		{
			var phoneAlarm = ToScheduledAlarm(alarm);
			_schedulerActionService.Add(phoneAlarm);
		}

		public void Remove(string name)
		{
			var existingAlarm = Find(name);
			if (existingAlarm == null)
			{
				return;
			}
			_schedulerActionService.Remove(name);
		}

		public AlarmAction Find(string name)
		{
			var scheduledAction = _schedulerActionService.Find(name);
			if (scheduledAction == null)
			{
				return null;
			}
			var alarm = scheduledAction as Alarm;
			if (alarm == null)
			{
				throw new InvalidOperationException("Scheduled action is not an alarm");
			}
			return ToAlarmAction(alarm);
		}

		public IEnumerable<AlarmAction> GetAlarms()
		{
			return _schedulerActionService.GetActions<Alarm>()
										  .Select(ToAlarmAction);
		}

		private static Alarm ToScheduledAlarm(AlarmAction alarm)
		{
			return new Alarm(alarm.Name)
			{
				BeginTime = alarm.BeginTime.LocalDateTime,
				ExpirationTime = alarm.ExpirationTime.LocalDateTime,
				Content = alarm.Content,
				RecurrenceType = (Microsoft.Phone.Scheduler.RecurrenceInterval)alarm.RecurrencyType,
				Sound = new Uri("/Assets/Titan Embark.wav", UriKind.Relative)
			};
		}

		private static AlarmAction ToAlarmAction(Alarm phoneAlarm)
		{
			return new AlarmAction()
			{
				BeginTime = new DateTimeOffset(phoneAlarm.BeginTime),
				ExpirationTime = new DateTimeOffset(phoneAlarm.ExpirationTime),
				IsScheduled = phoneAlarm.IsScheduled,
				IsEnabled = phoneAlarm.IsEnabled,
				Name = phoneAlarm.Name,
				Content = phoneAlarm.Content,
				RecurrencyType = (RecurrenceInterval)phoneAlarm.RecurrenceType,
				Title = phoneAlarm.Title
			};
		}
	}
}