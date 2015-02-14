using System;

namespace Hanno
{
	public interface INow
	{
		DateTimeOffset Now { get; }
	}

	public static class NowContext
	{
		private static INow _now;

		static NowContext()
		{
			ResetToDefault();
		}

		public static INow Current
		{
			get { return _now; }
		}

		public static void SetContext(INow now)
		{
			if (now == null) throw new ArgumentNullException("now");
			_now = now;
		}

		public static void ResetToDefault()
		{
			_now = new SystemUtcNow();
		}
	}

	public class SystemUtcNow : INow
	{
		public DateTimeOffset Now { get { return DateTimeOffset.UtcNow; } }
	}

	public class SystemNow : INow
	{
		public DateTimeOffset Now { get { return DateTimeOffset.Now; } }
	}

	public class FuncNow : INow
	{
		private readonly Func<DateTimeOffset> _now;

		public FuncNow(Func<DateTimeOffset> now)
		{
			if (now == null) throw new ArgumentNullException("now");
			_now = now;
		}

		public DateTimeOffset Now { get { return _now(); } }
	}
}