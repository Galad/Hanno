using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace Hanno.CqrsInfrastructure
{
	public class ExecuteOnSchedulerBase
	{
		protected Dictionary<Type, IScheduler> Schedulers;
		protected IScheduler DefaultScheduler;

		protected ExecuteOnSchedulerBase()
		{
			Schedulers = new Dictionary<Type, IScheduler>();
		}

		public void MapScheduler(IScheduler scheduler, Type[] types)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			if (types == null) throw new ArgumentNullException("types");
			foreach (var type in types)
			{
				Schedulers.Add(type, scheduler);
			}
		}

		public void SetDefaultScheduler(IScheduler scheduler)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			DefaultScheduler = scheduler;
		}
	}
}