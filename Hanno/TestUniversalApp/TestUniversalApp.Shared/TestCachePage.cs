using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Hanno.Cache;
using Hanno.CqrsInfrastructure;
using Hanno.Diagnostics;
using Hanno.Extensions;
using Hanno.Http;
using Hanno.ViewModels;

namespace TestUniversalApp
{
	public class TestCachePageViewModel : ViewModelBase
	{
		private readonly IAsyncQueryBus _bus;
		private IAsyncQueryBus _busWithCache;
		private readonly InvalidateCacheService _invalidateCache;
		private TimeSpan _cacheAge;
		private readonly CacheService _cacheService;

		public TestCachePageViewModel(IViewModelServices services)
			: base(services)
		{
			_bus = new DefaultHttpRequestBus(
							   new DefaultHttpRequestResolver(
								   new TestSpecificationSelector(),
								   new HttpRequestBuilder(),
								   new HttpRequestSpecificationsTransformer(
									   new HttpRequestBuilderFactory(),
									   new TestBuildHttpRequestResolver())),
							   new TestHttpRequestResultReaderResolver());
			_cacheService = new CacheService(new MemoryCacheEntryRepository());
			_busWithCache = new CacheAsyncQueryBus(
				_bus,
				_cacheService,
				TimeSpan.FromMinutes(10));
			_invalidateCache = new InvalidateCacheService(_cacheService);
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			TestQueryCommand = GetTestQueryCommand();
			TestQueryCommandWithCache = GetTestQueryCommandWithCache();
			InvalidateCache = GetInvalidateCacheCommand();
		}

		private ICommand GetTestQueryCommand()
		{
			return this.CommandBuilder("TestQueryCommand")
					   .Execute<string>(async (query, ct) =>
					   {
						   var queries = Enumerable.Range(0, 50)
												   .Select(_ => Task.Run(() => _bus.ProcessQuery<BingQuery, string>(new BingQuery(ct, query))));
						   var results = await Task.WhenAll(queries);
						   foreach (var result in results)
						   {
							   result.DebugWriteline();
						   }
					   })
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		private ICommand GetTestQueryCommandWithCache()
		{
			return this.CommandBuilder("TestQueryCommandWithCache")
					   .Execute<string>(async (query, ct) =>
					   {
						   var queries = Enumerable.Range(0, 10)
												   .Select(_ => Task.Run(() => _busWithCache.ProcessQuery<BingQuery, string>(new BingQuery(ct, query))));
						   var results = await Task.WhenAll(queries);
						   foreach (var result in results)
						   {
							   result.DebugWriteline();
						   }
					   })
					   .Error((token, exception) => Task.FromResult(true))
					   .ToCommand();
		}

		private ICommand GetInvalidateCacheCommand()
		{
			return this.CommandBuilderProvider
			           .Get("InvalidateCacheCommand")
			           .Execute<string>(async (q, ct) =>
			           {
				           var query = new BingQuery(ct, q);
				           await _invalidateCache.InvalidateCache<string>(ct, query);
			           })
			           .Error((token, exception) => Task.FromResult(true))
			           .ToCommand();
		}

		public ICommand TestQueryCommand { get; private set; }
		public ICommand TestQueryCommandWithCache { get; private set; }
		public ICommand InvalidateCache { get; private set; }

		public string CacheAge
		{
			get { return _cacheAge.TotalSeconds.ToString(); }
			set
			{
				_cacheAge = TimeSpan.FromSeconds(int.Parse(value));
				_busWithCache = new CacheAsyncQueryBus(
					_bus,
					_cacheService,
					_cacheAge);
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

	public class BingQuery : CachedAsyncQueryBase<string>
	{
		public BingQuery(CancellationToken ct, string query)
			: base(ct, "BingQuery", new KeyValuePair<string, string>("Query", query))
		{
			Query = query;
		}

		public string Query { get; private set; }
	}
}
