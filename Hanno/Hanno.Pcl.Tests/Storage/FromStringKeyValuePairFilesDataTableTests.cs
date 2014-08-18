using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using Hanno.IO;
using Hanno.Serialization;
using Hanno.Storage;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;
using FileAccess = Hanno.IO.FileAccess;
using FileMode = Hanno.IO.FileMode;

namespace Hanno.Tests.Storage
{
	//#region Customization
	//public class FromStringKeyValuePairFilesDataTableCustomization : ICustomization
	//{
	//	public void Customize(IFixture fixture)
	//	{
	//		fixture.Customize<Mock<IDirectoryOperations>>(c => c.Do(directoryOperations =>
	//			directoryOperations.Setup(d => d.Exists(It.IsAny<string>())).Returns(true)));
	//		fixture.Customize<Mock<ISerializer>>(c => c.Do(serializer =>
	//			serializer.Setup(s => s.Serialize(It.IsAny<int>(), It.IsAny<Stream>()))
	//					  .Callback((int k, Stream s) =>
	//					  {
	//						  var keyBytes = Encoding.UTF8.GetBytes(k.ToString(CultureInfo.InvariantCulture));
	//						  s.Write(keyBytes, 0, keyBytes.Length);
	//						  s.Position = 0;
	//					  })));
	//		fixture.Customize<Mock<IFileOperations>>(c => c.Do(fileOperations =>
	//			fileOperations.Setup(f => f.OpenRead(It.IsAny<string>()))
	//						  .Returns(new MemoryStream())));
	//		fixture.Customize<Mock<IDeserializer>>(c => c.Do(deserializer =>
	//			deserializer.Setup(s => s.Deserialize<object>(It.IsAny<Stream>())).Returns(fixture.Create<object>())));
	//	}
	//}

	//public class FromStringKeyValuePairFilesDataTableCompositeCustomization : CompositeCustomization
	//{
	//	public FromStringKeyValuePairFilesDataTableCompositeCustomization()
	//		: base(new AutoMoqCustomization(), new FromStringKeyValuePairFilesDataTableCustomization())
	//	{
	//	}
	//}

	//public class FromStringKeyValuePairFilesDataTableAutoDataAttribute : AutoDataAttribute
	//{
	//	public FromStringKeyValuePairFilesDataTableAutoDataAttribute()
	//		: base(new Fixture().Customize(new FromStringKeyValuePairFilesDataTableCompositeCustomization()))
	//	{
	//	}
	//}

	//public class FromStringKeyValuePairFilesDataTableInlineAutoDataAttribute : CompositeDataAttribute
	//{
	//	public FromStringKeyValuePairFilesDataTableInlineAutoDataAttribute(params object[] values)
	//		: base(new InlineDataAttribute(values), new FromStringKeyValuePairFilesDataTableAutoDataAttribute())
	//	{
	//	}
	//}
	//#endregion

	//public class FromStringKeyValuePairFilesDataTableTests
	//{
	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Get_ShouldReturnCorrectValue(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IDeserializer> deserializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		object expected,
	//		int key,
	//		Mock<Stream> valueStreamMock
	//		)
	//	{
	//		//arrange
	//		var valueStream = valueStreamMock.Object;
	//		fileOperations.Setup(f => f.OpenRead(Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture))))
	//					  .Returns(valueStream);
	//		deserializer.Setup(s => s.Deserialize<object>(valueStream)).Returns(expected);

	//		//act
	//		var actual = sut.Get(key);

	//		//assert
	//		actual.Should().Be(expected);
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Get_FileDoesNotExists_ShouldReturnDefaultValue(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key
	//		)
	//	{
	//		//arrange
	//		fileOperations.Setup(f => f.OpenRead(Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture))))
	//					  .Throws(new FileNotFoundException());

	//		//act
	//		var actual = sut.Get(key);

	//		//assert
	//		actual.Should().BeNull();
	//	}
		
	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void AddOrUpdate__ShouldSerializeValueOnFileStream(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key,
	//		object value,
	//		Mock<Stream> valueStreamMock
	//		)
	//	{
	//		//arrange
	//		fileOperations.Setup(f => f.Open(Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture)), FileMode.Create, FileAccess.Write))
	//					  .Returns(valueStreamMock.Object);

	//		//act
	//		sut.AddOrUpdate(key, value);

	//		//assert
	//		serializer.Verify(s => s.Serialize(value, valueStreamMock.Object));
	//	}
		
	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void AddOrUpdate__WhenTableDirectoryDoesNotExists_ShouldCreateTableDirectory(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<IDirectoryOperations> directoryOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key,
	//		object value,
	//		Mock<Stream> valueStreamMock,
	//		byte[] expectedBytes
	//		)
	//	{
	//		//arrange
	//		var tablePath = Path.Combine(dbPath, "System.Object");
	//		directoryOperations.Setup(d => d.Exists(tablePath)).Returns(false);
			
	//		//act
	//		sut.AddOrUpdate(key, value);

	//		//assert
	//		directoryOperations.Verify(d => d.CreateDirectory(tablePath));
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void AddOrUpdate__WhenTableDirectoryExists_ShouldNotCreateTableDirectory(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<IDirectoryOperations> directoryOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key,
	//		object value,
	//		Mock<Stream> valueStreamMock,
	//		byte[] expectedBytes
	//		)
	//	{
	//		//arrange
	//		var tablePath = Path.Combine(dbPath, "System.Object");
	//		directoryOperations.Setup(d => d.Exists(tablePath)).Returns(true);

	//		//act
	//		sut.AddOrUpdate(key, value);

	//		//assert
	//		directoryOperations.Verify(d => d.CreateDirectory(tablePath), Times.Never());
	//	}
		
	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Delete__ShouldCallDelete(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IDeserializer> deserializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key,
	//		object value,
	//		Mock<Stream> valueStreamMock,
	//		byte[] expectedBytes
	//		)
	//	{
	//		//arrange
	//		var path = Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture));
			
	//		//act
	//		sut.Delete(key);

	//		//assert
	//		fileOperations.Verify(f => f.Delete(path));
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Query_ShouldReturnAllFiles(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IDeserializer> deserializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		[Frozen]Mock<IDirectoryOperations> directoryOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		IFixture fixture
	//		)
	//	{
	//		//arrange
	//		var keys = fixture.CreateMany<int>(10).ToArray();
	//		var files = keys.Select(k => Path.Combine(dbPath, k.ToString(CultureInfo.InvariantCulture))).ToArray();
	//		var streams = Enumerable.Range(0, 10).Select(_ => new MemoryStream()).ToArray();
	//		var expectedValues = fixture.CreateMany<object>(10).ToArray();
	//		var path = Path.Combine(dbPath, "System.Object");
	//		directoryOperations.Setup(d => d.EnumerateFiles(path)).Returns(files);
	//		for (var i = 0; i < 10; i++)
	//		{
	//			fileOperations.Setup(f => f.OpenRead(files[i])).Returns(streams[i]);
	//		}
	//		for (var i = 0; i < 10; i++)
	//		{
	//			deserializer.Setup(d => d.Deserialize<object>(streams[i])).Returns(expectedValues[i]);
	//		}
	//		for (var i = 0; i < 10; i++)
	//		{
	//			deserializer.Setup(d => d.Deserialize<int>(It.IsAny<Stream>()))
	//						.Returns((Stream stream) =>
	//						{
	//							var bytes = new byte[stream.Length];
	//							stream.Read(bytes, 0, bytes.Length);
	//							var s = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
	//							return int.Parse(s);
	//						});
	//		}

	//		var expected = Enumerable.Range(0, 10)
	//								 .Select(i => new KeyValuePair<int, object>(keys[i], expectedValues[i]))
	//								 .ToArray();

	//		//act
	//		var actual = sut.Query().ToArray();

	//		//assert
	//		actual.ShouldAllBeEquivalentTo(expected);
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Query_FolderDoesNotExists_ShouldReturnEmptyEnumerable(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IDeserializer> deserializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		[Frozen]Mock<IDirectoryOperations> directoryOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		IFixture fixture,
	//		string[] files
	//		)
	//	{
	//		//arrange
	//		var path = Path.Combine(dbPath, "System.Object");
	//		directoryOperations.Setup(d => d.Exists(path)).Returns(false);
	//		directoryOperations.Setup(d => d.EnumerateFiles(path)).Returns(files);
			
	//		//act
	//		var actual = sut.Query().ToArray();

	//		//assert
	//		actual.Should().BeEmpty();
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Contains_FileDoesNotExists_ShouldReturnCorrectValue(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key
	//		)
	//	{
	//		//arrange
	//		fileOperations.Setup(f => f.OpenRead(Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture))))
	//					  .Throws(new FileNotFoundException());

	//		//act
	//		var actual = sut.Contains(key);

	//		//assert
	//		actual.Should().BeFalse();
	//	}

	//	[Theory, FromStringKeyValuePairFilesDataTableAutoData]
	//	public void Contains_FileExists_ShouldReturnCorrectValue(
	//		[Frozen]string dbPath,
	//		[Frozen]Mock<ISerializer> serializer,
	//		[Frozen]Mock<IFileOperations> fileOperations,
	//		FromStringKeyValuePairFilesDataTable<int, object> sut,
	//		int key
	//		)
	//	{
	//		//arrange
	//		fileOperations.Setup(f => f.OpenRead(Path.Combine(dbPath, "System.Object", key.ToString(CultureInfo.InvariantCulture))))
	//					  .Returns(new MemoryStream());

	//		//act
	//		var actual = sut.Contains(key);

	//		//assert
	//		actual.Should().BeTrue();
	//	}
	//}
}