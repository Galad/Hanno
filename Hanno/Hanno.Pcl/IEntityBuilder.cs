using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanno
{
	public interface IEntityBuilder
	{
		TTo BuildFrom<TFrom, TTo>(TFrom entityFrom);
	}

	public interface IEntityConverter<in TFrom, out TTo>
	{
		TTo ConvertEntity(TFrom entityFrom);
	}

	public interface IEntityConverterFactory
	{
		IEntityConverter<TFrom, TTo> GetConverter<TFrom, TTo>();
	}

	public sealed class EntityBuilder : IEntityBuilder
	{
		private readonly IEntityConverterFactory _converterFactory;

		public EntityBuilder(IEntityConverterFactory converterFactory)
		{
			if (converterFactory == null) throw new ArgumentNullException("converterFactory");
			_converterFactory = converterFactory;
		}

		private class Converter
		{
			public Type To;
			public Type From;
			public object Conv;
		}
		private readonly List<Converter> _existingConverters = new List<Converter>();
		public TTo BuildFrom<TFrom, TTo>(TFrom entityFrom)
		{
			var tto = typeof (TTo);
			var tfrom = typeof (TFrom);
			var converter = _existingConverters.FirstOrDefault(c => tfrom == c.From && tto == c.To);
			IEntityConverter<TFrom, TTo> conv;
			if (converter == null)
			{
				conv = _converterFactory.GetConverter<TFrom, TTo>();
				_existingConverters.Add(new Converter()
				{
					From = tfrom,
					To = tto,
					Conv = conv
				});
			}
			else
			{
				conv = (IEntityConverter<TFrom, TTo>) converter.Conv;
			}
			return conv.ConvertEntity(entityFrom);
		}
	}
}
