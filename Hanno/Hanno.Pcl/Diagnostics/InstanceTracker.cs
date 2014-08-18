using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Diagnostics
{
	public class InstanceMonitoringEventArgs : EventArgs
	{
		public InstanceMonitoringEventArgs(IList<object> instances)
		{
			if (instances == null) throw new ArgumentNullException("instances");
			Instances = instances;
		}

		public IList<object> Instances { get; private set; }
	}

	public interface IInstanceMonitor : IDisposable
	{
		event EventHandler<InstanceMonitoringEventArgs> Update;
	}

	internal class InstanceMonitor : IInstanceMonitor
	{
		private readonly Func<IList<object>> _getInstances;
		private readonly Timer _timer;

		private delegate void TimerCallback(object state);

		private sealed class Timer : CancellationTokenSource, IDisposable
		{
			internal Timer(TimerCallback callback, object state, int dueTime, int period)
			{
				CreateTimer(callback, state, dueTime, period);
			}

			private void CreateTimer(TimerCallback callback, object state, int dueTime, int period)
			{
				Task.Delay(dueTime, Token).ContinueWith((t, s) =>
				{
					var tuple = (Tuple<TimerCallback, object>)s;
					tuple.Item1(tuple.Item2);
					CreateTimer(callback, state, period, period);
				}, Tuple.Create(callback, state), CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default);
			}

			public new void Dispose()
			{
				base.Cancel();
			}
		}

		public event EventHandler<InstanceMonitoringEventArgs> Update;

		internal InstanceMonitor(Func<IList<object>> getInstances, TimeSpan time)
		{
			_getInstances = getInstances;
			_timer = new Timer(Callback, null, (int)time.TotalMilliseconds, (int)time.TotalMilliseconds);
		}

		private void Callback(object state)
		{
			if (Update != null)
			{
				//app crashes sometimes when the debugger is attached and we have a breakpoint
				if (!Debugger.IsAttached)
				{
					GC.Collect();
				}
				var instances = _getInstances();
				Update(this, new InstanceMonitoringEventArgs(instances));
			}
		}


		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				_timer.Dispose();
			}

			//Add disposition of unmanaged resources here

			_isDisposed = true;
		}

		~InstanceMonitor()
		{
			this.Dispose(false);
		}

		private bool _isDisposed;


	}

	public static class InstanceTracker
	{
		private class Instance
		{
			public WeakReference WeakReference;
			public string[] Tags;
		}

		private static List<Instance> _instances;

		static InstanceTracker()
		{
			_instances = new List<Instance>();
		}

		public static void Track(object instance, params string[] tags)
		{
			lock (_instances)
			{
				if (_instances.Any(i => i.WeakReference.Target == instance))
				{
					return;
				}
				_instances.Add(new Instance()
				{
					WeakReference = new WeakReference(instance),
					Tags = tags
				});
			}
		}

		public static IList<object> GetAllInstancesByTag(string tag)
		{
			Clean();
			return _instances.Where(i => i.WeakReference.IsAlive && i.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
							 .Select(i => i.WeakReference.Target)
							 .ToArray();
		}

		public static IList<object> GetAllInstancesByType(Type type)
		{
			Clean();
			return _instances.Where(i => i.WeakReference.IsAlive && type.GetTypeInfo().IsAssignableFrom(i.WeakReference.Target.GetType().GetTypeInfo()))
							 .Select(i => i.WeakReference.Target)
							 .ToArray();
		}

		public static IList<object> GetAllInstancesByType<T>()
		{
			Clean();
			return _instances.Where(i => i.WeakReference.IsAlive && typeof(T).GetTypeInfo().IsAssignableFrom(i.WeakReference.Target.GetType().GetTypeInfo()))
							 .Select(i => i.WeakReference.Target)
							 .ToArray();
		}

		public static int CountAllInstancesByTag(string tag)
		{
			Clean();
			return GetAllInstancesByTag(tag).Count;
		}

		public static int CountAllInstancesByType<T>()
		{
			Clean();
			return GetAllInstancesByType<T>().Count;
		}

		private static void Clean()
		{
			lock (_instances)
			{
				_instances.RemoveAll(i => !i.WeakReference.IsAlive);
			}
		}

		public static IInstanceMonitor MonitorByTag(string tag, TimeSpan time)
		{
			return new InstanceMonitor(() => InstanceTracker.GetAllInstancesByTag(tag), time);
		}

		public static IInstanceMonitor MonitorByType(Type type, TimeSpan time)
		{
			return new InstanceMonitor(() => InstanceTracker.GetAllInstancesByType(type), time);
		}

		public static IInstanceMonitor MonitorByType<T>(TimeSpan time)
		{
			return new InstanceMonitor(() => InstanceTracker.GetAllInstancesByType<T>(), time);
		}
	}
}