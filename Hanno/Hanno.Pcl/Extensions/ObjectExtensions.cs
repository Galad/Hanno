using System;
using System.Reflection;

namespace Hanno.Extensions
{
    public static class ObjectExtensions
    {
         public static bool IsAssignableTo<T1>(this object obj)
         {
             if (obj == null)
             {
                 return false;
             }
             return typeof (T1).GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
         }

	    public static T DoAction<T>(this T obj, Action<T> doAction)
	    {
		    doAction(obj);
		    return obj;
	    }
    }
}