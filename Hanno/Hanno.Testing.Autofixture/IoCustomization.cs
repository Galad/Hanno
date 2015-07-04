using System.IO;
using Ploeh.AutoFixture;

namespace Hanno.Testing.Autofixture
{
	public class IoCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<MemoryStream>(() => new MemoryStream());
			fixture.Customize<FileInfo>(c => c.OmitAutoProperties());
			fixture.Customize<DirectoryInfo>(c => c.OmitAutoProperties());
		}
	}
}