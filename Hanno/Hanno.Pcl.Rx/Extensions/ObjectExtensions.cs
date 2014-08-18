using System;
using System.Linq;
using System.Reflection;

namespace Hanno.Rx.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert an object which is an IObservable of TSource where TSource is an uknown type into an IObservable of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<T> Convert<T>(this object source)
        {
            {
            if (source == null)
            {
                return null;
            }
            Type sourceType = source.GetType();
            //gets the IObservable interface
            Type observableInterface = sourceType.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(type => type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition().GetGenericTypeDefinition() == typeof(IObservable<>));
            if (observableInterface == null)
            {
                return null;
            }
            //gets the generic type of the observable
            Type genericType = observableInterface.GetTypeInfo().GenericTypeArguments.FirstOrDefault();
            if (genericType == null)
            {
                return null;
            }
            var select = typeof(System.Reactive.Linq.Observable).GetTypeInfo().DeclaredMethods
                         .SingleOrDefault(method =>
                         {
                             var parameters = method.GetParameters();
                             return method.Name == "Select"
                                 && parameters.Count() == 2
                                 && parameters[0].ParameterType.Name == "IObservable`1"
                                 && parameters[1].ParameterType.Name == "Func`2";
                         });
            
            var exp = System.Linq.Expressions.Expression.Parameter(genericType, "o");
            var exp2 = System.Linq.Expressions.Expression.Convert(exp, typeof(T));
            var lambda = System.Linq.Expressions.Expression.Lambda(exp2, exp);
            var del = lambda.Compile();
 
            var genericSelect = select.MakeGenericMethod(genericType, typeof(T));          
            var result = genericSelect.Invoke(null, new object[] { source, del });
            IObservable<T> observableResult = result as IObservable<T>;
            return observableResult;
        }
    }

    }
}