using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.CqrsInfrastructure;
using Hanno.Extensions;
using Hanno.Http;
using Hanno.Navigation;
using Hanno.ViewModels;

namespace TestWindowsPhone
{
	public class MainViewModel : ViewModelBase
	{
		public string Test
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public IObservable<string> TestObservableProperty
		{
			get { return this.GetObservableProperty<string>(); }
		}

		protected override async Task OnLoaded(CancellationToken ct)
		{
			await base.OnLoaded(ct);
			//ObservableExtensions.Subscribe(TestObservableProperty, s => Debug.WriteLine(s), e => Debug.WriteLine(e)).DisposeWith(LongDisposables);
			//Test = "Hello";
			//ObservableExtensions.Subscribe(Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(5)), _ => Test = Guid.NewGuid().ToString(), e => Debug.WriteLine(e))
			//		  .DisposeWith(LongDisposables);
			//ObservableExtensions.Subscribe(TestObservableProperty
			//		.SelectMany(async (s, ct) =>
			//		{
			//			var bus = new DefaultHttpRequestBus(
			//				new DefaultHttpRequestResolver(
			//					new TestSpecificationSelector(),
			//					new HttpRequestBuilder(),
			//					new HttpRequestSpecificationsTransformer(
			//						new HttpRequestBuilderFactory(),
			//						new TestBuildHttpRequestResolver())),
			//				new TestHttpRequestResultReaderResolver());

			//			await Task.WhenAll(
			//				Enumerable.Range(0, 20)
			//						  .Select(_ => Request(s, ct, bus))
			//				);


			//			return Unit.Default;
			//		}), _ => Test = Guid.NewGuid().ToString(), e => Debug.WriteLine(e))
			//	.DisposeWith(LongDisposables);

			//var observable = Observable.Create<int>(obs =>
			//{
			//	for (var i = 0; i <= 1000000; i++)
			//	{
			//		obs.OnNext(i);
			//	}
			//	obs.OnCompleted();
			//	return Disposable.Empty;
			//})
			//						   .Do(i => i.ToString());
			//var observables1 = CreatePooledObservables(observable).ToArray();
			//GC.Collect();
			//var memory = GC.GetTotalMemory(false);
			//var sw = Stopwatch.StartNew();
			//observables1.Merge().Subscribe(_ => { }, e => { }, () =>
			//{
			//	sw.Stop();
			//	Debug.WriteLine("POOLED : " + sw.ElapsedMilliseconds);
			//	Debug.WriteLine("MEMORY : " + (GC.GetTotalMemory(false) - memory));
			//	var observables2 = CreateObservables(observable);
			//	GC.Collect();
			//	memory = GC.GetTotalMemory(false);
			//	var sw2 = Stopwatch.StartNew();
			//	observables2.Merge().Subscribe(_ => { }, e => { }, () =>
			//	{
			//		sw2.Stop();
			//		Debug.WriteLine("NOT POOLED : " + sw2.ElapsedMilliseconds);
			//		Debug.WriteLine("MEMORY : " + (GC.GetTotalMemory(false) - memory));
			//	});
			//});
		}

		private IEnumerable<IObservable<int>> CreatePooledObservables(IObservable<int> source)
		{
			for (var i = 0; i <= 50; i++)
			{
				yield return this.ObservableFromPool(() => source, Scheduler.Default, "Test22");
			}
		}

		private IEnumerable<IObservable<int>> CreateObservables(IObservable<int> source)
		{
			for (var i = 0; i <= 50; i++)
			{
				yield return source;
			}
		}

		private async Task Request(string s, CancellationToken ct, IAsyncQueryBus bus)
		{
			await Services.Schedulers.ThreadPool.Run(async () =>
			{
				var query = new BingQuery(ct) { Query = s };
				var result = await bus.ProcessQuery<BingQuery, string>(query);
				Debug.WriteLine(result);
			});
		}

		public ICommand Navigate
		{
			get
			{
				return this.CommandBuilder()
						   .Execute(() => Observable.FromAsync(() => Task.Run(async () =>
				{
					//for (var i = 0; i < 10000; i++)
					//{
						await Services.RequestNavigation.Navigate(CancellationToken.None, ApplicationPages.Second);
						//await Services.NavigationService.NavigateBack();
					//}
				}
			)))
						   .ToCommand();
			}
		}
	}



	public class TestHttpRequestResultReaderResolver : IHttpRequestResultReaderResolver
	{
		public IHttpRequestResultReader ResolveHttpRequestResultReader<TQuery>(TQuery query)
		{
			return new StringRequestResultReader();
		}
	}

	public class StringRequestResultReader : IHttpRequestResultReader
	{
		public async Task<T> Read<T>(IHttpRequestResult result, CancellationToken ct)
		{
			using (var stream = await result.Content.ReadStream(ct))
			using (var sr = new StreamReader(stream))
			{
				var s = await sr.ReadToEndAsync();
				return (T)(object)s;
			}
		}
	}

	public class TestBuildHttpRequestResolver : IBuildHttpRequestResolver
	{
		public IBuildHttpRequest<T> ResolveBuildHttpRequest<T>(T parameter) where T : IAsyncParameter
		{
			if (typeof(BingQuery) == typeof(T))
			{
				return (IBuildHttpRequest<T>)new BingQuerySpecification();
			}
			return null;
		}
	}

	public class TestSpecificationSelector : IHttpRequestSpecificationResolver
	{
		public IHttpRequestSpecification ResolveHttpRequestSpecification<T>(T query) where T : IAsyncParameter
		{
			return new BingQuerySpecification();
		}
	}

	public class BingQuerySpecification : BuildSynchronousHttpRequest<BingQuery>, IHttpRequestSpecification
	{
		public IHttpRequestBuilderOptions CreateSpecifiation(IHttpRequestBuilderFactory httpRequestMethodBuilder)
		{
			return httpRequestMethodBuilder.CreateRequestBuilder(new Uri("http://www.bing.com"))
										   .Get()
										   .AppendPath("search");
		}

		protected override IHttpRequestBuilderOptions BuildSynchronously(IHttpRequestBuilderOptions options, BingQuery query)
		{
			return options.Parameter("q", query.Query);
		}
	}
}
