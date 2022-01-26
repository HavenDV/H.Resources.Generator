# [H.Resources.Generator](https://github.com/HavenDV/H.Resources.Generator/) 

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.Resources.Generator/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.Resources.Generator.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Standard%202.0-blue.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
[![Build Status](https://github.com/HavenDV/H.Resources.Generator/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.Resources.Generator/actions?query=workflow%3A%22.NET%22)

### Nuget

[![NuGet](https://img.shields.io/nuget/dt/H.Resources.Generator.svg?style=flat-square&label=H.Resources.Generator)](https://www.nuget.org/packages/H.Resources.Generator/)

```
Install-Package H.Resources.Generator
```

### Usage
Just install this package and add any resources to Resources subfolder. After you can use resources in the code:
```cs
var bytes = H.Resources.name_png.AsBytes();
// or
var text = H.Resources.name_txt.AsText();
```

### Advanced Usage

```xml
<PropertyGroup>
  <HResourcesGenerator_WithSystemDrawing>true</HResourcesGenerator_WithSystemDrawing>
</PropertyGroup>

<ItemGroup Label="Images">
  <EmbeddedResource Include="Images\*.png" />
  <AdditionalFiles Include="Images\*.png" />
</ItemGroup>
```

After it, use resource in code:
```cs
var image = H.Resources.image_name_png.AsImage();
// or
var bytes = H.Resources.image_name_png.AsBytes();
```

Available methods:
- System.Drawing.Image AsImage() (only if `HResourcesGenerator_WithSystemDrawing` is true)
- System.IO.Stream AsStream()
- string AsString()
- byte[] AsBytes()

Global options(Default values are provided and can be omitted):
```xml
<PropertyGroup>
  <HResourcesGenerator_Namespace>H</HResourcesGenerator_Namespace>
  <HResourcesGenerator_Modifier>internal</HResourcesGenerator_Modifier>
  <HResourcesGenerator_ClassName>Resources</HResourcesGenerator_ClassName>
  <HResourcesGenerator_AddResourcesFolder>true</HResourcesGenerator_AddResourcesFolder>
  <HResourcesGenerator_WithSystemDrawing>false</HResourcesGenerator_WithSystemDrawing>
</PropertyGroup>
```

By default, it includes this code:
```xml
<ItemGroup Condition="$(HResourcesGenerator_AddResourcesFolder)">
  <EmbeddedResource Include="Resources\**\*.*" />
  <AdditionalFiles Include="Resources\**\*.*" />
</ItemGroup>
```
You can disable this behavior with `<HResourcesGenerator_AddResourcesFolder>false</HResourcesGenerator_AddResourcesFolder>`

### Alternatives
- [Resourcer.Fody](https://github.com/Fody/Resourcer)

### Contacts
* [mail](mailto:havendv@gmail.com)
