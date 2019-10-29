## What is it?
Topshelf.Integrations is a collection of packages that extend the [Topshelf Project](http://topshelf-project.com). These packages handle the boiler plate code necessary for several use cases that we have found very useful for quickly developing small self contained services on Windows.

These use cases include the following:

*	HTTP/REST Communication from and to a Windows Service
*	Scheduled activities hosted within a Windows Service
*	Integration of your preferred IoC Container

These packages solve these problems by integrating the following technologies with Topshelf and providing extensions to quickly integrate them.

*	[Microsoft WebAPI](http://www.asp.net/web-api) - HTTP/REST Communication
*	[Quartz.NET](http://quartznet.sourceforge.net/) - Scheduling
*	[Ninject](http://www.ninject.org/) - IoC Container

## Getting Started

These packages are available on [Nuget](http://nuget.org/) and can be used in any combination desired. The Ninject package is directly integrated with Topshelf, and also has accompanying packages for the WebAPI and Quartz.Net projects to make it possible to instantiate ApiControllers via the container and initiate Quartz.NET IJob instances via the container.

### Topshelf.Ninject

To get the package: `Install-Package Topshelf.Ninject`

To use Ninject with your Topshelf service, all you need is three lines:

	using Topshelf.Ninject;

	...

    class Program
    {
        static void Main()
        {
            HostFactory.Run(c =>
            {
                c.UseNinject(new SampleModule()); //Initiates Ninject and consumes Modules

                c.Service<SampleService>(s =>
                {
                    //Specifies that Topshelf should delegate to Ninject for construction
                    s.ConstructUsingNinject(); 
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });
        }
    }

### Topshelf.Quartz & Topshelf.Quartz.Ninject

To get the package: `Install-Package Topshelf.Quartz`

To add Ninject Support: `Install-Package Topshelf.Quartz.Ninject`

There are two options for using Quartz.NET with Topshelf, you may schedule any number of Quartz jobs along with your service like this:

	using System;
	using Ninject.Modules;
	using Quartz;
	using Topshelf;
	using Topshelf.Ninject;
	using Topshelf.Quartz;
	using Topshelf.Quartz.Ninject;

	...

	class Program
    {
        static void Main()
        {
            HostFactory.Run(c =>
            {
            	// Topshelf.Ninject (Optional) - Initiates Ninject and consumes Modules
                c.UseNinject(new SampleModule());

                c.Service<SampleService>(s =>
                {
                    //Topshelf.Ninject (Optional) - Construct service using Ninject
                    s.ConstructUsingNinject();

                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());

                    // Topshelf.Quartz.Ninject (Optional) - Construct IJob instance with Ninject
                    s.UseQuartzNinject(); 

                    // Schedule a job to run in the background every 5 seconds.
                    // The full Quartz Builder framework is available here.
                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SampleJob>().Build())
                        .AddTrigger(() =>
                            TriggerBuilder.Create()
	                            .WithSimpleSchedule(builder => builder
		                            .WithIntervalInSeconds(5)
		                            .RepeatForever())
	                            .Build())
                        );
                });
            });
        }
    }

You can also schedule a job as your service if no continually running service implementation is needed

    using System;
    using Ninject.Modules;
    using Quartz;
    using Topshelf;
    using Topshelf.Ninject;
    using Topshelf.Quartz;
    using Topshelf.Quartz.Ninject;


    ...

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(c =>
            {

                    // Topshelf.Ninject (Optional) - Initiates Ninject and consumes Modules
                    c.UseNinject(new SampleModule());
                    // Topshelf.Quartz.Ninject (Optional) - Construct IJob instance with Ninject
                    c.UseQuartzNinject();

                c.ScheduleQuartzJobAsService(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SampleJob>().Build())
                        .AddTrigger(() =>
                            TriggerBuilder.Create()
                                .WithSimpleSchedule(builder => builder
                                    .WithIntervalInSeconds(5)
                                    .RepeatForever())
                                .Build())
                );
            });

        }
    }

### Topshelf.WebApi & Topshelf.WebApi.Ninject

To get the package: `Install-Package Topshelf.WebApi`

To add Ninject Support: `Install-Package Topshelf.WebApi.Ninject`

The WebAPI endpoint can be initialized alongside your service like this:

    using System;
    using System.Web.Http;
    using Ninject.Modules;
    using Topshelf;
    using Topshelf.Ninject;
    using Topshelf.WebApi;
    using Topshelf.WebApi.Ninject;

    ...

    static void Main()
    {
        HostFactory.Run(c =>
        {
            c.UseNinject(new SampleModule()); //Initiates Ninject and consumes Modules

            c.Service<SampleService>(s =>
            {
                //Specifies that Topshelf should delegate to Ninject for construction
                s.ConstructUsingNinject();

                s.WhenStarted((service, control) => service.Start());
                s.WhenStopped((service, control) => service.Stop());

                //Topshelf.WebApi - Begins configuration of an endpoint
                s.WebApiEndpoint(api => 
                    //Topshelf.WebApi - Uses localhost as the domain, defaults to port 8080.
                    //You may also use .OnHost() and specify an alternate port.
                    api.OnLocalhost()
                        //Topshelf.WebApi - Pass a delegate to configure your routes
                        .ConfigureRoutes(Configure)
                        //Topshelf.WebApi.Ninject (Optional) - You may delegate controller 
                        //instantiation to Ninject.
                        //Alternatively you can set the WebAPI Dependency Resolver with
                        //.UseDependencyResolver()
                        .UseNinjectDependencyResolver()
                        //Instantaties and starts the WebAPI Thread.
                        .Build());
            });
        });
    }

    private static void Configure(HttpRouteCollection routes)
    {
        routes.MapHttpRoute(
                "DefaultApiWithId", 
                "Api/{controller}/{id}", 
                new { id = RouteParameter.Optional }, 
                new { id = @"\d+" });
    }
