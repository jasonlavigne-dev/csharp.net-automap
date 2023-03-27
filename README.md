# Lightweight .NET Auto Mapper (C#)
This lightweight auto mapper is a simple and efficient object mapper for .NET applications. It provides a set of extension methods that enable you to map properties between objects without requiring any third-party packages, complex configurations, or code generation. The extension methods are designed to be dynamic, allowing any object to use them out-of-the-box.

The mapper is dynamic, allowing all objects to use it and achieving optimal performance without any complex usage. It has a small code footprint and can be used with various runtimes.

The implementation is based on regular .NET, and its small code footprint ensures optimal performance for an auto mapper. The following runtimes are supported: .NET Core 3.1, .NET 5, and .NET 6.

## Usage

Assuming the `Country` class has properties `Id`, `Name`, `Iso2`, `Iso3`, and `Tld`, and the `CountrySummary` class has properties `Name` and `Iso2`.

```csharp
Country sourceCountry = new Country { Id = 1, Name = "United States", Iso2 = "US", Iso3 = "USA", Tld = ".us" };
CountrySummary destCountrySummary;
```

## AutoMapNew

Maps properties from sourceCountry to a new instance of CountrySummary.

```csharp
destCountrySummary = sourceCountry.AutoMapNew<CountrySummary>();
```

## AutoMapTo

Maps properties from sourceCountry to an existing CountrySummary instance.

```csharp
sourceCountry.AutoMapTo(destCountrySummary);
```

## AutoMapFrom

Maps properties from sourceCountry to the destCountrySummary instance. The Id property is not included in the mapping (default behavior).

```csharp
destCountrySummary.AutoMapFrom(sourceCountry);
```

## AutoMapFrom with includeId = true

Maps properties from sourceCountry to the destCountrySummary instance. The Id property is included in the mapping.

```csharp
destCountrySummary.AutoMapFrom(sourceCountry, true);
```

## Customization
To exclude specific properties that should not be mapped, modify the excludeProperties HashSet:

```csharp
HashSet<string> excludeProperties = new() { "ETag" };
```

Simply add any additional property names to the HashSet to exclude them from mapping.
