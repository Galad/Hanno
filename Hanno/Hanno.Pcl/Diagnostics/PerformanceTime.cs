using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Diagnostics
{
	public static class PerformanceTime
	{
		public static Tuple<TimeSpan, TimeSpan> Compare(Action firstAction, Action secondAction)
		{
			var firstSw = Stopwatch.StartNew();
			firstAction();
			firstSw.Stop();

			var secondSw = Stopwatch.StartNew();
			secondAction();
			secondSw.Stop();

			return new Tuple<TimeSpan, TimeSpan>(firstSw.Elapsed, secondSw.Elapsed);
		}

		public static async Task<Tuple<TimeSpan, TimeSpan>> Compare(Func<Task> firstAction, Func<Task> secondAction)
		{
			var firstSw = Stopwatch.StartNew();
			await firstAction();
			firstSw.Stop();

			var secondSw = Stopwatch.StartNew();
			await secondAction();
			secondSw.Stop();

			return new Tuple<TimeSpan, TimeSpan>(firstSw.Elapsed, secondSw.Elapsed);
		}

		public static TimeSpan Measure(Action action)
		{
			var sw = Stopwatch.StartNew();
			action();
			sw.Stop();
			return sw.Elapsed;
		}

		public static async Task<TimeSpan> Measure(Func<Task> action)
		{
			var sw = Stopwatch.StartNew();
			await action();
			sw.Stop();
			return sw.Elapsed;
		}

		public static async Task<T> Measure<T>(Func<Task<T>> action, Action<TimeSpan> timeCallback)
		{
			var sw = Stopwatch.StartNew();
			var result = await action();
			sw.Stop();
			timeCallback(sw.Elapsed);
			return result;
		}
	}
}