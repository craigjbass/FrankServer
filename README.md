# FrankServer [![NuGet](https://img.shields.io/nuget/v/FrankServer.svg)](https://www.nuget.org/packages/FrankServer/) [![Build Status](https://github.com/craigjbass/Frank/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/craigjbass/Frank/actions?query=workflow%3A%22.NET+Core+CI%22)

FrankServer is a _library_ that lets you route incoming HTTP requests to specific code that deals with that request.

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

## Contributing

