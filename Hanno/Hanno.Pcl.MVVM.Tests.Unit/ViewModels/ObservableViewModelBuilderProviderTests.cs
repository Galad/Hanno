using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	#region Customization
	public class ObservableViewModelBuilderProviderCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<Func<Action<IObservableViewModel>, IScheduler, IScheduler, IObservableViewModelBuilder>>(() =>
				((action, s1, s2) => new ObservableViewModelBuilder(action, s1, s2, fixture.Create<ISchedulers>())));
		}
	}

	public class ObservableViewModelBuilderProviderCompositeCustomization : CompositeCustomization
	{
		public ObservableViewModelBuilderProviderCompositeCustomization()
			: base(new AutoMoqCustomization(), new ObservableViewModelBuilderProviderCustomization())
		{
		}
	}

	public class ObservableViewModelBuilderProviderAutoDataAttribute : AutoDataAttribute
	{
		public ObservableViewModelBuilderProviderAutoDataAttribute()
			: base(new Fixture().Customize(new ObservableViewModelBuilderProviderCompositeCustomization()))
		{
		}
	}

	public class ObservableViewModelBuilderProviderInlineAutoDataAttribute : CompositeDataAttribute
	{
		public ObservableViewModelBuilderProviderInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new ObservableViewModelBuilderProviderAutoDataAttribute())
		{
		}
	}
	#endregion

	public class ObservableViewModelBuilderProviderTests
	{
		[Theory, ObservableViewModelBuilderProviderAutoData]
		public void Get_TestGuardClauses(
			GuardClauseAssertion assertion,
		  ObservableViewModelBuilderProvider sut)
		{
			assertion.Verify(() => sut.Get(""));
		}

		[Theory, ObservableViewModelBuilderProviderAutoData]
		public void Get_GettingASingleBuilder_ShouldReturnCorrectValue(
		  ObservableViewModelBuilderProvider sut,
			string name)
		{
			//arrange

			//act
			var actual = sut.Get(name);

			//assert
			actual.Should().BeOfType<ObservableViewModelBuilder>();
		}

		[Theory, AutoMoqData]
		public void Get_ShouldCallAccept(
			Mock<IObservableViewModelVisitor> visitor,
			IBindable parent,
			ISchedulers schedulers,
			Mock<IObservableViewModelBuilder> ovmBuilder,
			Mock<IObservableViewModel> ovm
			)
		{
			//arrange
			Action<IObservableViewModel> action = null;
			var sut = new ObservableViewModelBuilderProvider(() => schedulers, (a, scheduler, arg3) =>
			{
				action = a;
				return ovmBuilder.Object;
			});
			sut.AddVisitor(visitor.Object);

			//act
			sut.Get("test");
			action(ovm.Object);

			//assert
			ovm.Verify(o => o.Accept(visitor.Object));
		}

		[Theory, AutoMoqData]
		public void CopyVisitors_ShouldCallAddVisitor(
		  ObservableViewModelBuilderProvider sut,
		  IObservableViewModelVisitor[] visitors,
		  Mock<IObservableViewModelBuilderProvider> other)
		{
			//arrange
			foreach (var ovmVisitor in visitors)
			{
				var v = ovmVisitor;
				sut.AddVisitor(ovmVisitor);
				other.Setup(c => c.AddVisitor(v)).Verifiable();
			}

			//act
			sut.CopyVisitors(other.Object);

			//assert
			other.Verify();
		}
	}
}