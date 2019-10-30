# FrankServer [![NuGet](https://img.shields.io/nuget/v/FrankServer.svg)](https://www.nuget.org/packages/FrankServer/) [![Build Status](https://github.com/craigjbass/Frank/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/craigjbass/Frank/actions?query=workflow%3A%22.NET+Core+CI%22)

FrankServer is a _library_ that lets you route incoming HTTP requests to specific code that deals with that request.

## Why?

Testing. Particularly, speed-of-tests. 

FrankServer is designed to facilitate fast, high-level integration tests.

### How fast? 

Well - the current Frank EndToEndTests suite finishes in less than 200ms.

Each single test spins up and tears down *the entire TCP server*, with a performance of between 10-100ms.

## Installing

Add `FrankServer` as a NuGet dependency to your project.

## Usage

Currently, you can create a project like this:

```C#
using System.Threading;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace SampleProject
{
    class Program
    {
        static void Main()
        {
            Frank.Server
                .Configure()
                .ListenOn(80)
                .WithRoutes(
                    router =>
                    {
                        router.Get("/test").To((request) => Ok());
                    }
                )
                .Build()
                .Start();
            
            SpinWait.SpinUntil(() => false);
        }
    }
}
```

This causes any GET request to `/test` to result in a 200 status code.

## FAQs

*Why not just use ASP.NET Core MVC?*

Maybe you should. However, your tests will probably be orders of magnitude slower than FrankServer.

*Does this support EntityFramework?*

Maybe you should use ASP.NET Core MVC. This is for people who want fast tests.

*Do you recommend I use this on X project?*

I dunno? If you are asking that, maybe you should use ASP.NET Core MVC.

*This looks incomplete.*

Yes, yes it is. it's v0.1. What do you want from me?

