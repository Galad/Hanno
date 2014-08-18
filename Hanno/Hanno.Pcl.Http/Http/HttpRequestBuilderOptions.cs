using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Hanno.Http
{
	public sealed class HttpRequestBuilderOptions : IHttpRequestMethodBuilder, IHttpRequestBuilderOptions, IHttpRequestDefinition
	{
		private const char HttpPathSeparator = '/';
		private readonly List<string> _pathFragments = new List<string>();
		private readonly IDictionary<string, string> _parameters = new Dictionary<string, string>();
		private readonly IDictionary<string, string> _payloadParameters = new Dictionary<string, string>();
		private readonly IList<KeyValuePair<string, string>> _headers = new List<KeyValuePair<string, string>>();
		private readonly IList<Action<HttpHeaders>> _headersActions = new List<Action<HttpHeaders>>();
		private readonly IList<Tuple<Uri, Cookie>> _cookies = new List<Tuple<Uri, Cookie>>();
		private readonly Uri _baseUri;

		public Uri BaseUri
		{
			get { return _baseUri; }
		}

		public IEnumerable<string> PathFragments { get { return _pathFragments; } }

		public HttpRequestBuilderOptions(Uri uri)
		{
			if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
				!uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Uri must be http or https", "uri");
			}
			_baseUri = uri;
			HttpMethod = null;
		}

		public IHttpRequestBuilderOptions Method(HttpMethod method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			HttpMethod = method;
			return this;
		}


		public IHttpRequest ToRequest(IHttpRequestBuilder httpRequestBuilder)
		{
			return httpRequestBuilder.BuildRequest(this);
		}

		public IHttpRequestBuilderOptions AppendPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			var paths = path.Trim(HttpPathSeparator)
							.Split(new[] { HttpPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
			_pathFragments.AddRange(paths);
			return this;
		}

		public IHttpRequestBuilderOptions Parameter(string key, string value)
		{
			return ParameterInternal(key, value, _parameters);
		}

		public IHttpRequestBuilderOptions PayloadParameter(string key, string value)
		{
			return ParameterInternal(key, value, _payloadParameters);
		}

		private IHttpRequestBuilderOptions ParameterInternal(string key, string value, IDictionary<string, string> dictionary)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			dictionary.Add(key, value);
			return this;
		}

		public IHttpRequestBuilderOptions Header(string key, string value)
		{
			_headers.Add(new KeyValuePair<string, string>(key, value));
			return this;
		}

		public IHttpRequestBuilderOptions Header(Action<HttpHeaders> setHeaders)
		{
			if (setHeaders == null) throw new ArgumentNullException("setHeaders");
			_headersActions.Add(setHeaders);
			return this;
		}

		public IHttpRequestBuilderOptions Cookie(Uri uri, Cookie cookie)
		{
			_cookies.Add(new Tuple<Uri, Cookie>(uri, cookie));
			return this;
		}

		#region HttpRequestDefinition

		public Uri Uri
		{
			get
			{
				var pathsFragments = Enumerable.Concat(
					new[] { _baseUri.ToString().TrimEnd(HttpPathSeparator) },
					_pathFragments.Select(Uri.EscapeDataString));
				var parameters = _parameters.Select(p => string.Format("{0}={1}", Uri.EscapeDataString(p.Key), Uri.EscapeDataString(p.Value)));
				var separator = _parameters.Count == 0 ? "" : "?";
				var uriString = string.Concat(
					string.Join(HttpPathSeparator.ToString(), pathsFragments),
					separator,
					string.Join("&", parameters));
				return new Uri(uriString);
			}
		}

		public IEnumerable<Tuple<Uri, Cookie>> Cookies { get { return _cookies; } }

		public HttpMethod HttpMethod { get; private set; }

		public IEnumerable<KeyValuePair<string, string>> Parameters
		{
			get { return _parameters; }
		}

		public IEnumerable<KeyValuePair<string, string>> PayloadParameters
		{
			get { return _payloadParameters; }
		}

		public Stream StreamContent { get; private set; }
		public string StringContent { get; private set; }
		public IEnumerable<Action<HttpHeaders>> HeaderActions
		{
			get { return _headersActions; }
		}

		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get { return _headers; }
		}

		#endregion
	}
}