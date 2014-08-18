using System;
using System.Linq;
using System.Reflection;
using FluentAssertions.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentAssertions
{
	public static class ObjectExtensions
	{
		public static EventRecorder ShouldNotRaise<TSut>(this TSut sut, string eventName)
		{
			var recorder = sut.GetRecorderForEvent(eventName);
			return recorder;
		}
	}

	public static class EventRecorderExtensions
	{
		public static void NotRaisedWithArgs<TEventArgs>(this EventRecorder recorder, Func<TEventArgs, bool> predicate)
		{
			var hasEvent = recorder.Any(r =>
			{
				var args = r.Parameters.FirstOrDefault(p => typeof(TEventArgs).GetTypeInfo().IsAssignableFrom(p.GetType()));
				if (args == null)
				{
					return false;
				}
				return predicate((TEventArgs)args);
			});
			if (hasEvent)
			{
				Assert.Fail("{0} should have not raised event {1} with predicate but it did", recorder.EventObject, recorder.EventName);
			}
		}
    }
}