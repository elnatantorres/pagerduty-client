# PagerDuty Client

A library that allows you to easily use PagerDuty's Events API V2.

# Table of Contents

* [Installation](#installation)
* [Quick Start](#quick-start)

## Installation

### Setup Environment Variables

In order to use this library, the **Routing Key** of your PagerDuty's service
must be set at the environment variables, so it can be used to redirect the 
event to the correspondent service.

Setup Environment Variables using CMD:

* 1. Run CMD as administrator
* 2. setx ROUTING_KEY "YOUR_ROUTING_KEY"

An example of how the Routing Key can be set programatically:

```
Environment.SetEnvironmentVariable("ROUTING_KEY", "YOUR_ROUTING_KEY");
```

### Install Package

To use PagerDuty Client in your C# project, install it using the NuGet 
package manager:

```
PM> Install-Package PagerDutyClient
```

### Dependencies

* Please see the [.csproj](https://github.com/elnatantorres/pagerduty-client/blob/master/PagerDuty.csproj) file.

## Quick Start

### Trigger Event with summary and component

The following code shows how to trigger an event at PagerDuty's Events API V2, 
simulating that an error that occurred must be notified. It sends to 
PagerDuty a summary of the event (summary), the part or component of the affected
system (component) and some additional data (custom details):

```csharp
using PagerDuty.Events;

namespace Example
{
    class Example
    {
        static void Main(string[] args)
        {
            using(TriggerEvent triggerEvent = TriggerEvent.New())
            {
                try
                {
                    triggerEvent.CustomDetails["Url"] = "https://example.com.br";
                }
                catch (Exception)
                {
                    EventResponse eventResponse = triggerEvent.Trigger("The HTTP request failed.", "ExampleApplication");
                }
            }
        }
    }
}
```

After the execution of the above code, `eventResponse.StatusCode` should be 
`200`. However, if PagerDuty return any error, it can be accessed at 
`eventResponse.Errors`.

If you want to trigger an event without custom details, the following code 
should be more useful:

```csharp
using PagerDuty.Events;

namespace Example
{
    class Example
    {
        static void Main(string[] args)
        {
            Pager.Trigger("The HTTP request failed.", "ExampleApplication");
        }
    }
}
```




