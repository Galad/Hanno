using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq.Language.Flow;
using Xunit;

namespace Hanno.Testing.Autofixture
{
	public static class MockExtensions
	{
		public static IReturnsResult<T> ReturnsDefaultTask<T>(this ISetup<T, Task> setup) where T : class
		{
			return setup.Returns(() => Task.FromResult(default(object)));
		}

		public static IReturnsResult<T> ReturnsDefaultTask<T, TResult>(this ISetup<T, Task<TResult>> setup) where T : class
		{
			return setup.Returns(() => Task.FromResult(default(TResult)));
		}

		public static IReturnsResult<T> ReturnsTask<T, TResult>(this ISetup<T, Task<TResult>> setup, Func<TResult> result) where T : class
		{
			return setup.Returns(() => Task.FromResult(result()));
		}

		public static IReturnsResult<T> ReturnsTask<T, TResult>(this ISetup<T, Task<TResult>> setup, TResult result) where T : class
		{
			return setup.Returns(() => Task.FromResult(result));
		}

		public static IReturnsResult<T> ReturnsTaskTrue<T>(this ISetup<T, Task<bool>> setup) where T : class
		{
			return setup.ReturnsTask(() => true);
		}

		public static IReturnsResult<T> ReturnsTaskFalse<T>(this ISetup<T, Task<bool>> setup) where T : class
		{
			return setup.ReturnsTask(() => false);
		}

		public static ITaskVerifies ReturnsDefaultTaskVerifiable<T>(this ISetup<T, Task> setup) where T : class
		{
			var verifiable = new TaskCompletedVerification();
			setup.Returns(() => Task.Run(() =>
			{
				verifiable.MarkAsCompleted();
				return true;
			}));
			return verifiable;
		}

		public static ITaskVerifies ReturnsDefaultTaskVerifiable<T, TResult>(this ISetup<T, Task<TResult>> setup) where T : class
		{
			return setup.ReturnsTaskVerifiable(() => default(TResult));
		}

		public static ITaskVerifies ReturnsTaskVerifiable<T, TResult>(this ISetup<T, Task<TResult>> setup, Func<TResult> result) where T : class
		{
			var verifiable = new TaskCompletedVerification();
			setup.Returns(() => Task.Run(() =>
			{
				verifiable.MarkAsCompleted();
				return result();
			}));
			return verifiable;
		}

		public static IReturnsResult<T> ThrowsTask<T>(this ISetup<T, Task> setup, Func<Exception> excepection) where T : class
		{
			return setup.Returns(() => Task.Run(() =>
			{
				throw excepection();
			}));
		}

		public static IReturnsResult<T> ThrowsTask<T, TResult>(this ISetup<T, Task<TResult>> setup, Func<Exception> excepection) where T : class
		{
			return setup.Returns(() => Task.Run(() =>
			{
				throw excepection();
				return default(TResult);
			}));
		}
	}

	internal class TaskCompletedVerification : ITaskVerifies
	{
		private bool _isCompleted;

		public void Verify()
		{
            if (!_isCompleted && !System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
			_isCompleted.Should().BeTrue("The was expected to be awaited but it was not.");
		}

		public void MarkAsCompleted()
		{
			_isCompleted = true;
		}
	}

	public interface ITaskVerifies
	{
		void Verify();
	}
}
