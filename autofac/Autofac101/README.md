# Autofac 101

## Prerequisites
- Visual Studio

## Introduction
Welcome to Autofac 101!  This 20-30 minute mini-hack will guide you through basic principles of Autofac.  To get started, clone this repository and load the Autofac101 solution.  If you stumble along the way, create an issue.

## Getting Started
Autofac is a simple, yet powerful dependency container.  Autofac assists us in implementing patters like [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection) and [IoC](https://en.wikipedia.org/wiki/Inversion_of_control).  The goal of this course is not to cover those topics, but to get your feet wet with what Autofac can do for you.

## Setup
After cloning the repository, you should find yourself looking at a simple console application that doesn't compile.  Let's fix that.

Install the latest stable version of [Autofac](https://www.nuget.org/packages/Autofac/).  Our application bootstrapper needs to set up our Autofac registrations before we can start the app.  With Autofac, everything generally starts with a ```ContainerBuilder```.  You can think of your Autofac registrations as a kind of configuration for your application.  You'd typically perform this configuration once, during application startup.  So, let's create a ```ContainerBuilder``` in our ```Bootstrapper```'s ```Bootstrap``` function.
```csharp
public Application Bootstrap()
{
	var builder = new ContainerBuilder();
}
```
We can now use this builder to create registrations.  Let's register our ```EnglishMannersProvider```.
```csharp
builder.RegisterType<EnglishMannersProvider>();
```
Now, whenever a ```EnglishMannersProvider``` is resolved from the container, Autofac will create one for us.

Next, build the container and resolve our ```Application``` to get things rolling.
```csharp
var container = builder.Build();
return container.Resolve<Application>();
```
Now, everything should compile and we can start the application.  Give it a shot.

Shit.  That didn't work.

What went wrong?  We never registered our ```Application``` with the container.  Ok, no problem.  Let's add that registration quick and try again.
```csharp
builder.RegisterType<Application>();
```

Nope!  Still no good.  When we resolve an ```Application``` from the container, Autofac will use reflection to determine what types are requied to create an instance.  Our Application has two constructor dependencies, ```IProvideWelcome``` and ```IProvideFarewell```.  But didn't we register a ```EnglishMannersProvider``` which implements both of those interfaces?  This is true, but Autofac never makes assumptions about your registrations.  We need to tell Autofac what specific interfaces our ```EnglishMannersProvider``` should be registered under.  In autofac tems, this is called applying a limiter.  Once a limiter is applied, Autofac will only resolve an instance for the limiters you defined during registration.  There are a number of ways to provide limiters.  Let's try out a couple common approaches.

### Aproach 1 - Sniper
Here, we explicity target the interfaces this type implements.  When either a ```IProvideWelcome``` or ```IProvideFarewell``` are resolved from the container, a new ```EnglishMannersProvider``` will be instantiated and returned.
```csharp
builder.RegisterType<EnglishMannersProvider>()
	.As<IProvideWelcome>()
	.As<IProvideFarewell>();
```
### Approach 2 - Shotgun
Not explicit, but effective at getting this type registered for each of its interfaces.  This is generally my preferred approach.
```csharp
builder.RegisterType<EnglishMannersProvider>()
	.AsImplementedInterfaces();
```
Run the application again.  You should see our expected greeting and farewell.  This isn't super impressive, is it.  Let's say we want our appliction to greet the user in English, but then say farewell in French because it sounds fancy.  How would we do that?  Try out these registrations and run the application again.
```csharp
builder.RegisterType<EnglishMannersProvider>()
	.As<IProvideWelcome>();
builder.RegisterType<FrenchMannersProvider>()
	.As<IProvideFarewell>();
```
Try experimenting with adding different registrations for these interfaces.  Can you get your application to speak Spanish?  What happens if multiple types are registered for the same limiter?  Can you figure out how to regiter these services as singletons?

## Modules
It is always the recommended approach to perform Autofac registrations within an Autofac ```Module```.  A ```Module``` is little more than a collection of registrations, but this provides us some advantages.  If this project were a class library instead of a console application, we could define all of our libraries required registrations in a reusable ```Module``` that some other application could then register with its own ```ContainerBuilder```.  Add a new folder to the project named "Autofac" and add a new class, ```MannersModule```, that extends ```Autofac.Module```.  We'll want to override ```Load``` and copy over all of our registrations from ```Bootstrap``` into our new module.  Our ```ContainerBuilder``` will need to register our new module.
```csharp
public class MannersModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{

		builder.RegisterType<Application>();

		builder.RegisterType<EnglishMannersProvider>()
			.As<IProvideWelcome>();
		builder.RegisterType<FrenchMannersProvider>()
			.As<IProvideFarewell>();
	}
}
```
```csharp
builder.RegisterModule<MannersModule>();
```
That's it!  We now have a reusable module that encapsulates our application behavior.  Let's try some neat tricks we can do with modules.  Add some "configuration" options to ```MannersModule```.
```csharp
public enum Language
{
	English,
	French,
	Spanish
}

private Language LanguageMode { get; }

public MannersModule(Language lang)
{
	LanguageMode = lang;
}

protected override void Load(ContainerBuilder builder)
{
	builder.RegisterType<Application>();

	switch (LanguageMode)
	{
		case Language.English:
		{
			builder.RegisterType<EnglishMannersProvider>()
				.AsImplementedInterfaces();
			break;
		}
		case Language.French:
		{
			builder.RegisterType<FrenchMannersProvider>()
				.AsImplementedInterfaces();
			break;
		}
		case Language.Spanish:
		{
			builder.RegisterType<SpanishMannersProvider>()
				.AsImplementedInterfaces();
			break;
		}
	}
}
```
```csharp
builder.RegisterModule(new MannersModule(MannersModule.Language.English));
```
Try running the application with different Language modes set.  It's not hard to see how powerful modules can be.  Our module now exposes a simple configuration option and changes its behavior based on the value of that option.  If someone wanted to use our manners services in their application, they wouldn't need to be concerned with which services to register or how.  They'd only need to configure our module with the clear language options we provided.
