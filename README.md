# Amp.NET
[![Travis](https://img.shields.io/travis/joncloud/amp-net.svg)](https://travis-ci.org/joncloud/amp-net/)
[![NuGet](https://img.shields.io/nuget/v/Amp.svg)](https://www.nuget.org/packages/Amp/)

<img src="https://raw.githubusercontent.com/joncloud/amp-net/master/nuget.png" alt="amp.net" />

## Description
Amp.NET provides warm up routines to ASP.NET Core applications.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package Amp
```

Or update `*.csproj` to include a dependency on

```xml
<ItemGroup>
  <PackageReference Include="Amp" Version="0.1.0-*" />
</ItemGroup>
```

## Usage
You can configure your application in order to specify Amp's behavior.
* Path - (`"/warmup"`) The path which invokes the asynchronous initialization
* Parallelism - (`WarmUpParallelism.Parallel`) Execution flow for each individual `IWarmUp`.

```csharp
public void ConfigureServices(IServiceCollection services) => 
  services.AddWarmUp(builder => {
    builder.Path = "/my-custom-path";
    builder.Parallelism = WarmUpParallelism.Sequential;
  });
```

You can warm up your application in one of two ways: asynchronous or synchronous.

### Asynchronous
Use asynchronous warm up when you want to initialize the system separately from application boot up.
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
  app.UseWarmUp().UseMvc();  
```

### Synchronous
Use synchronous warm up when you need to initialize the system before the application begins to take requests.
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
  app.WarmUpAsync().GetAwaiter().GetResult();
  app.UseMvc();
}
```

### Code Samples
Sample integration with Entity Framework Core:
```csharp
class Startup {
  ...
  
  public void ConfigureServices(IServiceCollection services) => 
    services.AddWarmUp().AddScoped<IWarmUp, ApplicationDbContextWarmUp>();
    
  public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
    app.UseWarmUp().UseMvc();
}

class ApplicationDbContextWarmUp : IWarmUp {
  readonly ApplicationDbContext _context;
  public ApplicationDbContextWarmUp(ApplicationDbContext context) => _context = context;
  
  public Task InvokeAsync() => _context.Users.AnyAsync();
}
```

For additional usage see [Tests][].

[Tests]: tests/Amp.Tests
