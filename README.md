# [H.Resources.Generator](https://github.com/HavenDV/H.Resources.Generator/) 

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.Resources.Generator/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.Resources.Generator.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build Status](https://github.com/HavenDV/H.Resources.Generator/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.Resources.Generator/actions?query=workflow%3A%22.NET%22)

Description

### Nuget

[![NuGet](https://img.shields.io/nuget/dt/H.Resources.Generator.svg?style=flat-square&label=H.Resources.Generator)](https://www.nuget.org/packages/H.Resources.Generator/)

```
Install-Package H.Resources.Generator
```

### Usage

```xml
<ItemGroup Label="Images">
  <EmbeddedResource Include="Images\*.png" />
  <AdditionalFiles Include="Images\*.png" /> // It creates System.Drawing.Image properties
</ItemGroup>
```

You can set up type explicitly:
```xml
<AdditionalFiles Include="Images\*.png" Type="Bytes" /> // It creates byte[] properties
```

Supported types:
- Image(System.Drawing.Image)
- Bytes(byte[])

### Contacts
* [mail](mailto:havendv@gmail.com)
