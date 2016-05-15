# Autofac 110
## Prerequisites
- Visual Studio

## Introduction
Greetings, web crawler!  This mini-hack is all about integrating Autofac into your web application stack of choice.  We'll be covering integration into ASP.NET Web API, MVC, and, my favorite, OWIN.  Each integration has its nuances, so it's good to be familiar with each.
## Getting Started
As always, your first step will be to clone this repository.  The solution you'll want to open is Autofac110.  Upon opening this solution, you should be looking at three web projects (Web API, MVC, and OWIN respectively) and a fourth shared library that contains our very simple shared service.  All three sites are configured to run in IIS Express.  Make sure you can run each one out of the box.  If you point your browser at each site's HelloControler, you should be presented with a cliche "Hello World!" message.
> API: [http://localhost:2238/api/Hello](http://localhost:2238/api/Hello)<br/>
MVC: [http://localhost:2242/Hello](http://localhost:2242/Hello)<br/>
OWIN: [http://localhost:2250/api/Hello](http://localhost:2250/api/Hello)

If you take a closer look at each of the three `HelloController`'s, you'll notice they're pretty boring.  Each has hardcoded "Hello World!" as its response message.  We can certainly do better than that.
## Web API
With an old-school Web API project, our best place to register our Autofac dependencies will be in Global.asax.  This is typically where application bootstrapping occurs.  Let's add a new `ContainerBuilder`.
```csharp
protected void Application_Start()
{
	var builder = new ContainerBuilder();

	GlobalConfiguration.Configure(WebApiConfig.Register);
}
```
Of course, to make our compiler happy we'll need to add the latest [Autofac nuget package](https://www.nuget.org/packages/Autofac/) to our project.  Go ahead and add this to all three sites while you're at it.

Next, let's register the most basic service I've ever created, ``SayHelloWorld```, from the shared library.
```csharp
builder.RegisterType<SayHelloWorld>()
	.AsImplementedInterfaces();
```
And then, of course, build the container.
```csharp
var container = builder.Build();
```
Now, in our API `HelloControler` (`autofac110.api.Controllers.HelloController`), add ISayHelloWorld as a dependency and use it to generate the response message instead of the magic string.  Our controller should now look like this:
```csharp
private ISayHelloWorld HelloWorld { get; }

public HelloController(ISayHelloWorld helloWorld)
{
	HelloWorld = helloWorld;
}

public string Get(ISayHelloWorld h)
{
	return HelloWorld.SayHello();
}
```
Try running the site now.  What do you think will happen?  If you guessed exceptions and failure, you'd be right.  We never registered our controller with Autofac.  We could try registering it as we've learned to do...
```csharp
builder.RegisterType<HelloController>();
```
but this won't work either.  There are several Web API framework classes at work here that locate and instantiate our `HelloController` when a request is received on the controller's corresponding path `api/Hello`.  We have to enable some integration between Web API and Autofac, so these Web API framework classes resolve our controller from the container instead of the default behavior.  For this, we'll need an additional nuget package.  Install [Autofac.WebAPI2](https://www.nuget.org/packages/Autofac.WebApi2/) and replace the following with the contents of `Application_Start` in `Global.asax'.
```csharp
var builder = new ContainerBuilder();

builder.RegisterType<SayHelloWorld>()
	.AsImplementedInterfaces();

//registeres anything that extends ApiController in this assembly
builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

var container = builder.Build();

//sets the http config's dependency resolver to use autofac
GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
GlobalConfiguration.Configure(WebApiConfig.Register);
```
You may also register the Autofac filter providers at this time.  I've never done so, but you should know it's an option.  Check out Autofac's [documentation](http://docs.autofac.org/en/latest/integration/webapi.html#id5) for more info on the subject.

That's all there is to it!  Run the site again and you should see a beautiful "Hello World!" message generated from our service and serialized into not-so-beautiful XML!
## MVC
MVC's Autofac integration is very similar to that of Web API, but there are differences, so strap in.

Again, check out our MVC site's HelloController.  Death to all magic strings!  Once again, pull in our SayHelloWorld service.
```csharp
private ISayHelloWorld HelloWorld { get; }

public HelloController(ISayHelloWorld helloWorld)
{
	HelloWorld = helloWorld;
}

// GET: Hello
public ActionResult Index()
{
    return View(new HelloModel
    {
	    Message = HelloWorld.SayHello()
    });
}
```
I'd say try running the site to see what happens, but I doubt you'll fall for that trick again.  Add the necessary registrations to `Global.asax`.  We'll need another Autofac integration nuget package, [Autofac.Mvc5](https://www.nuget.org/packages/Autofac.Mvc5/) first.
```csharp
AreaRegistration.RegisterAllAreas();
RouteConfig.RegisterRoutes(RouteTable.Routes);

var builder = new ContainerBuilder();

builder.RegisterType<SayHelloWorld>()
	.AsImplementedInterfaces();

// registers everything that extends Controller
builder.RegisterControllers(Assembly.GetExecutingAssembly());

var container = builder.Build();

// set the global dependency resolver
DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
```
> Important note!  Notice that with MVC, the dependency resolver is set on a global static class.  This is different from Web API, where the resolver is set on a HttpConfiguration.  We'll see in later courses that HttpContigurations can be newed up to create multiple sub-sites with specific Autofac containers, whereas MVC forces you to have one, global container.

If you run the MVC site again, you should again see the same "Hello World!" message now being generated from our dependency-injected service!  I think we can take this a bit farther...

### MVC Web Types
Autofac allows you to inject common MVC types anywhere that's running in an MVC context.  Let's grab some info out of the current HttpContext and display that along with our "Hello World!" message.
```csharp
builder.RegisterModule<AutofacWebTypesModule>();
```
And now, back in `HelloController`
```csharp
private ISayHelloWorld HelloWorld { get; }
private HttpContextBase Context { get; }

public HelloController(ISayHelloWorld helloWorld, HttpContextBase context)
{
	HelloWorld = helloWorld;
	Context = context;
}

// GET: Hello
public ActionResult Index()
{
	return View(new HelloModel
	{
		Message = $"{HelloWorld.SayHello()}  Your computer's name is {Context.Server.MachineName}"
	});
}
```
HttpContextBase is one of many MVC web types that Autofac can grab for us.  You can also hook up Autofac dependency-injection with your Models, Views, and Action Filters.  Autofac has great [documentation](http://autofac.readthedocs.io/en/latest/integration/mvc.html), so I'd suggest checking that out for further info.
## OWIN
There will be an entire set of mini-hacks on OWIN, so I don't want to go into too many details regarding best practices on this subject.  Just know that what we're about to do is beyond mini-hack - this is a supernasty-hack, but it's good for demonstrating how to get Autofac dependency injection working with OWIN middleware.

OWIN applications don't use a `Global.asax` for bootstrap/startup configuration, because OWIN is better.  Instead, we've got a `Startup` class with a bootstrap method, conventionally named `Configuration`.  The autofac110.owin project contains the simplest of simple middlewares, `HelloWorldMiddleware`.

To give at least a little background, let's take a look at `Startup`.  You'll notice this looks very much like a typical `Global.asax` for a Web API project.  Noticiable differences are we're creating our own `HttpConfiguration` and a couple `app.Use`-style method calls.  Each invokation of `app.Use` adds a step in a web request processing pipeline.  Order matters.  Our current configuration will process any incoming requests first with our `HelloWorldMiddleware` and, if our middleware chooses to process the rest of the pipeline, invoke ASP.NET WebApi.  Try placing debug breakpoints in our middleware and in our `HelloController` to see this processing order in action when running the site.

If you make several requests to `/api/Hello`, you should notice a few more things:
1. An instance of `HelloWorldMiddlware` is created per request.
2. We are not instantiating `HelloWorldMiddlware`, OWIN is.
3. OWIN is already injecting a depdendency into `HelloWorldMiddelware`, the next middleware in the pipeline.

Let's now pretend that, for some reason, we want all requests to respond with "Hello World!" regardless of the path we hit.  We could hard code this into our middleware, but we know it'd be better practice to reuse our `SayHelloWorld` service from earlier.  Let's try adding the service to our middleware as a dependency.
```csharp
private ISayHelloWorld HelloWorld { get; }

public HelloWorldMiddleware(OwinMiddleware next, ISayHelloWorld helloWorld) : base(next)
{
	HelloWorld = helloWorld;
}

public override Task Invoke(IOwinContext context)
{
	return context.Response.WriteAsync(HelloWorld.SayHello());
}
```
We know this won't work from past experience.  We haven't set up any Autofac registrations and we haven't told OWIN anything about this new service.  Let's run the application anyway to see what kind of error we get.  Did you try it?  OWIN throws an exception, of course.  It can't find a `HelloWorldMiddleware` constructor that matches the signature it's looking for.  Lucky for us, Autofac has us covered.  You'll need to install the [Autofac.WebApi2.Owin](https://www.nuget.org/packages/Autofac.WebApi2.Owin/) integration NuGet package since this is a Web API project.  For MVC, you'd need [Autofac.Mvc5.Owin](https://www.nuget.org/packages/Autofac.Mvc5.Owin/).

Now, we can add our container and run the application again.
```csharp
public void Configuration(IAppBuilder app)
{
	var config = new HttpConfiguration();

	WebApiConfig.Register(config);

	var builder = new ContainerBuilder();

	builder
		.RegisterType<SayHelloWorld>()
		.AsImplementedInterfaces()
		.InstancePerRequest();

	// register our middleware with Autofac
	builder.RegisterType<HelloWorldMiddleware>();
	
	var container = builder.Build();
	
	// registers all OwinMiddleware with the pipeline
	app.UseAutofacMiddleware(container)
		.UseAutofacWebApi(config)
		.UseWebApi(config);
}
```
Notice, we're no longer invoking `app.Use<HelloWorldMiddleware>()`.  Instead, we're registering our middleware with the container.  When `app.UseAutofacMiddleware` is invoked, all `Micorosft.Owin.OwinMiddleware` in the container are automatically added to the OWIN pipeline by Autofac.  Run the application again.  You should see the raw response (no longer xml) generated by our `SayHelloWorld` service!
# Congrats!
You are now an Autofac integration guru.  Feel free to experiment with more complex integration scenarios.  Can you make a site that is OWIN hosted with both WebApi and MVC, all using Autofac dependency injection?  As always, check out the full [Autofac documentation](http://docs.autofac.org/en/latest/integration/index.html) for more info and examples on this subject.
