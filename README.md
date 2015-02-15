Project description
===================

Hanno is a Framework designed to build maintainable, reliable and fast MVVM applications. It allows you to use the [Reactive Extensions](https://github.com/Reactive-Extensions/Rx.NET) in your view models and expose [IObservable\<T>](https://msdn.microsoft.com/en-us/library/dd990377%28v=vs.110%29.aspx) to the XAML view through an **IObservableProperty\<T>** instance.
It also provides ways to build ICommand, IObservableViewModel and http request with a fluent builder, a CQRS infrastructure, etc. among other features.

Supported Platforms
==================

Hanno supports
- Windows 8.1
- Windows Phone 8.1
- WPF (.NET 4.5)

Dependencies
============

Hanno is composed of several assemblies (ie assemblies). The core project, Hanno.Pcl has no dependencies, other than the PCL .NET subset. Other assemblies are here depending of the dependencies needded. For instance, you don't have to depend on the MVVM assembly if you just want to use the CQRS infrastructure. Hanno remains extensible.
Most of Hanno abstractions and classes are build for the Portable Class Library (For Windows 8.1, Windows Phone 8.1 and .NET 4.5), except when platform specific feature or APIs are used. In that Hanno provides a platform specific project.

Here is the complete list of Hanno PCL projects assemblies ('->' means 'depends on')
- Hanno.Pcl
- Hanno.Rx -> Hanno.Pcl
- Hanno.MVVM -> Hanno.Rx
- Hanno.Http -> Hanno.Pcl, [HttpClient](https://www.nuget.org/packages/Microsoft.Net.Http)
- Hanno.Json -> Hanno.Pcl, [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)

