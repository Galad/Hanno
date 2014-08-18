using System;
using System.Collections.Generic;
using System.Text;
using Hanno;

namespace TestUniversalApp
{
	public class EntityConverter : IEntityConverter<ClassA, ClassB>
	{
		public ClassB ConvertEntity(ClassA entityFrom)
		{
			if (entityFrom == null)
			{
				return null;
			}
			return new ClassB()
			{
				Test = entityFrom.Test,
				Test2 = entityFrom.Test2,
				Test3 = entityFrom.Test3,
				Test4 = entityFrom.Test4,
				Test5 = entityFrom.Test5
			};
		}
	}

	public class ClassA
	{
		public string Test { get; set; }
		public string Test2 { get; set; }
		public string Test3 { get; set; }
		public string Test4 { get; set; }
		public string Test5 { get; set; }
	}

	public class ClassB
	{
		public string Test { get; set; }
		public string Test2 { get; set; }
		public string Test3 { get; set; }
		public string Test4 { get; set; }
		public string Test5 { get; set; }
	}
}
