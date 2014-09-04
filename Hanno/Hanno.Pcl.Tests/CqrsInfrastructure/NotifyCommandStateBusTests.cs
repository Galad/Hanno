using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class NotifyCommandStateBusTests
	{
		[Theory, AutoData]
		public void Sut_IsAsyncCommandBus(
		  NotifyCommandStateBus sut)
		{
			//arrange

			//act
			sut.Sut();

			//assert
		} 
	}
}