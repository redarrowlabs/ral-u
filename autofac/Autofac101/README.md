# Autofac 101

## Prerequisites
- Visual Studio

## Introduction
Welcome to Autofac 101!  This 20-30 minute mini-hack will guide you through basic principles of Autofac.  To get started, clone this repository and load the Autofac101 solution.  If you stumble along the way, create an issue.

## Getting Started
Autofac is a simple, yet powerful dependency container.  Autofac assists us in implementing patterns like [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection) and [IoC](https://en.wikipedia.org/wiki/Inversion_of_control).  The goal of this course is not to cover those topics, but to get your feet wet with what Autofac can do for you.

## Setup
After cloning the repository, you should find yourself looking at a simple console application that doesn't compile.  Let's fix that.

Install the latest stable [Autofac nuget package](https://www.nuget.org/packages/Autofac/).  Our application bootstrapper needs to set up our Autofac registrations and build the container when the application starts.  With Autofac, everything generally starts with a `ContainerBuilder`.  You can think of your Autofac registrations as a kind of application behavior configuration.  You'd typically perform this configuration once, during application startup.  So, let's create a `ContainerBuilder` in our `Bootstrapper`'s `Bootstrap` function.
```csharp
public Application Bootstrap()
{
	var builder = new ContainerBuilder();
}
```
We can now use this builder to create registrations.  Let's register our `EnglishMannersProvider`.
```csharp
builder.RegisterType<EnglishMannersProvider>();
```
Now, whenever a `EnglishMannersProvider` is resolved from the container, Autofac will create one for us.

Next, build the container and resolve our `Application` to get things rolling.
```csharp
var container = builder.Build();
return container.Resolve<Application>();
```
Now, everything should compile and we can start the application.  Give it a shot.

Shit.  That didn't work.

What went wrong?  We never registered our `Application` with the container.  We can't resolve things from our container that we haven't registered.  Ok, no problem.  Let's add that registration quick and try running again.
```csharp
builder.RegisterType<Application>();
```
Nope!  Still no good.  Take a close look at the exception being thrown.  Autofac is telling you that it doesn't have anything registered for the types required by `Application`'s constructor.  When we resolve an `Application` from the container, Autofac will use reflection to determine what types are requied to create an instance, locate registrations with matching types, resolve them, and then use them as arguments for our requested resource's constructor.  Our Application has two constructor dependencies, `IProvideWelcome` and `IProvideFarewell`.  But didn't we register a `EnglishMannersProvider` which implements both of those interfaces?  This is true, but Autofac never makes assumptions about your registrations.  We need to tell Autofac what specific interfaces our `EnglishMannersProvider` should be registered as.  In autofac terms, this is called applying a limiter.  Once a limiter is applied, Autofac will only resolve an instance for the limiters you defined during registration.  There are a number of ways to provide limiters.  Let's try out a couple common approaches.

### Aproach 1 - Sniper
Here, we explicity target the interfaces this type implements.  When either a `IProvideWelcome` or `IProvideFarewell` are resolved from the container, a new `EnglishMannersProvider` will be instantiated and returned.
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
Try experimenting with adding different registrations for these interfaces.  Can you get your application to speak Spanish?  What happens if multiple types are registered for the same limiter?  Can you figure out how to register these services as singletons?

> To see further examples of registrations and limiters, check out this [fiddle](https://dotnetfiddle.net/dVqaip).

## Modules
It is always the recommended approach to perform Autofac registrations within an Autofac `Module`.  A `Module` is little more than a collection of registrations, but this provides us some advantages.  If this project were a class library instead of a console application, we could define all of our required registrations in a reusable `Module` that some other application could register with its own `ContainerBuilder`.

Add a new folder to the project named "Autofac" and add a new class, `MannersModule`, that extends `Autofac.Module`.  We'll want to override `Load` and move all of our registrations from `Bootstrap` into our new module.  Our `ContainerBuilder` will need to register our new module as well.
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
That's it!  We now have a reusable module that encapsulates our application behavior configuration.  Let's try out some tricks we can do with modules.  Add some "configuration" options to `MannersModule`.
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
Try running the application with different Language modes set.  It's not hard to imagine how powerful modules can be.  Our module now exposes a simple configuration option and changes its internal behavior based on the value of that option.  If someone wanted to use our manners services in their application, they wouldn't need to be concerned with which services to register or how.  They'd only need to configure our module with the clear language options we provided.
## Factories
You may, from time to time, come across a situation where you need a factory.  If your factory is creating a type that has dependencies, we don't want to be newing up services when we can be leveraging dependency injection.  Lucky for us, Autofac can generate factories for us.  Let's play around with the Fibonacci services.  Add registrations to our module and add a dependency to `Application` before running the application again.
```csharp
builder.RegisterType<FibonacciSequenceComputer>()
	.AsImplementedInterfaces();
builder.RegisterType<FibonacciNumberComputer>()
	.AsImplementedInterfaces();
```
```csharp
private IComputeFibonacciSequence Fibonacci { get; }

public Application(
	IProvideWelcome welcomeProvider,
	IProvideFarewell farewellProvider,
	IComputeFibonacciSequence fibonacci)
{
	WelcomeProvider = welcomeProvider;
	FarewellProvider = farewellProvider;
	Fibonacci = fibonacci;
}

public void Run()
{
	WelcomeProvider.Greet();
	Fibonacci.Compute();
	FarewellProvider.Farewell();
}
```
This isn't super interesting yet.  We probably want our `FibonacciSequenceComputer` to compute as much of the Fibonacci sequence as the user requests.  We would normally provide this count to `Compute` as an argument, but that wouldn't teach us much about Autofac factories.  Instead, let's make the count a constructor argument and use this count when computing the Fibonacci sequence.
```csharp
private int Count { get; }

public FibonacciSequenceComputer(int count, IComputeFibonacciNumber fibonacciNumber)
{
	Count = count;
	FibonacciNumber = fibonacciNumber;
}

public void Compute()
{
	var sequence = Enumerable.Range(0, Count)
		.Select(FibonacciNumber.Compute)
		.ToArray();

	Console.WriteLine($"The first {Count} numbers in the Fibonacci sequence are:");
	Console.WriteLine(string.Join(" ", sequence));
}
```
So, what is Autofac going to do with this constructor dependency of type `int`?  Turns out, there are ways of telling Autofac how to resolve parameters like this, but we won't get into that until a later course.  For now, let's focus on factories.  Change `Application`'s dependency.
```csharp
private Func<int, IComputeFibonacciSequence> Fibonacci { get; }

public Application(
			IProvideWelcome welcomeProvider,
			IProvideFarewell farewellProvider,
			Func<int, IComputeFibonacciSequence> fibonacci)
		{
			WelcomeProvider = welcomeProvider;
			FarewellProvider = farewellProvider;
			Fibonacci = fibonacci;
		}
```
What is this madness, you ask?  Do we need to also register a `Func<int, IComputeFibonacciSequence>`?  While we could do that, we don't have to.  Autofac will generate a `Func` for us that, when invoked, given some `int` value, resolves an `IComputeFibonacciSequence` using our provided `int` value for the matching constructor argument type.  But our constructor also has an `IComputeFibonacciNumber` parameter and our factory doesn't provide an argument of this type!  How can this work!?  Autofac is smart enough to check the container for any registrations that match any constructor parameters not provided by the factory.  Pretty neat, huh?  Let's change our `Run` function to be a little more interactive.
```csharp
public void Run()
{
	WelcomeProvider.Greet();
	Console.Write("How many Fibonacci numbers would you like to compute? ");
	Fibonacci(Convert.ToInt32(Console.ReadLine())).Compute();
	FarewellProvider.Farewell();
}
```
This is obviously a very contrived example, but hopefully you can see the value here.  It's not an uncommon need to create something based on some current application state.  This allows us to have more control over when instances are created, how they are created, provide some stateful information to that instance, and still leverage Autofac dependency injection.

Continue to experiment with your factory to see how different registrations will affect the factory.  Try adding some more constructor parameters.  Maybe register `IComputeFibonacciSequence` as a singleton and see what that does to your factory.
## You did it!
That wasn't so bad, was it?  You now know the basics of Autofac.  If you want to continue to play around with this sample application, check out the very helpful [Autofac documentation guide](http://autofac.readthedocs.io/en/latest/index.html) to get some ideas or check out the next Autofac course, Autofac110 (coming soon).
