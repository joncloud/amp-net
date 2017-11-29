# Amp.NET
[![Travis](https://img.shields.io/travis/joncloud/amp-net.svg)](https://travis-ci.org/joncloud/amp-net/)

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
Sample integration with Entity Framework Core:
```csharp
class Startup {
  ...
  
  public void ConfigureServices(IServiceCollection services) => 
    services.AddWarmUp().AddScoped<IWarmUp, ApplicationDbContextWarmUp>()
}

class ApplicationDbContextWarmUp : IWarmUp {
  readonly ApplicationDbContext _context;
  public ApplicationDbContextWarmUp(ApplicationDbContext context) => _context = context;
  
  public Task InvokeAsync() => _context.Users.AnyAsync();
}
```

For additional usage see [Tests][].

[Tests]: tests/Amp.Tests