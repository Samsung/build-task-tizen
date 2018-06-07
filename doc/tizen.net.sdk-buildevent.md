# Build Events in `Tizen.NET` project

Since Tizen.NET.Sdk >= 1.0.1

## Overview of the tpk packaging sequence

`Tizen.NET.Sdk` works after build by `Microsoft.NET.Sdk`. The build order is as follows:

```xml
  <PropertyGroup>
    <TizenPackageDependsOn>
      Build;                        <!-- Create dlls by Microsoft.NET.Sdk -->
      TizenResolveTpkPackageFiles;  <!-- Computing Tpk Package Files -->
      TizenPrePackageEvent;         <!-- This event excecutes before the tpk package begins -->
      _TizenPrepareTpkPackage;      <!-- Copy Tpk resources to tpkroot -->
      _TizenTpkSign;                <!-- Sign & Tpk pakcage the tpkroot -->
      TizenPostPackageEvent;        <!-- This event excecutes after the tpk package begins -->
    </TizenPackageDependsOn>
  </PropertyGroup>
```

Developers can use the pre/post package events to define additional behavior.

## Examples

### Show message in Pre/Post Package Event

```xml
  <!-- Your .csproj --> 
...
  <Target Name="TestPrePackageEvent" BeforeTargets="TizenPrePackageEvent">
    <Message Importance="high" Text="TestPrePackageEvent!!" />
  </Target>

  <Target Name="TestPostPackageEvent" AfterTargets="TizenPostPackageEvent">
    <Message Importance="high" Text="TestPostPackageEvent!!" />
  </Target>
``` 

### Show Tpk Files in Pre/Post Package Event


```xml
  <!-- Your .csproj --> 
...
  <Target Name="YourTarget" BeforeTargets="TizenPrePackageEvent">
    <!-- Print Text --> 
    <Message Text="YourTarget Start" Importance="high" />
    <!-- Print Source path, Dest path --> 
    <Message Text="Source path : %(TizenResolvedFileToTpk.Identity)&#xA;Destination path : %(TizenResolvedFileToTpk.TizenTpkSubPath)" Importance="high" />
    <!-- Force Error in Pacakaging Sequence --> 
    <Error Condition="true" Text="Error Occured YourTarget!!" />
  </Target>
```

```
1>YourTarget Start
1>Source path : obj\Debug\netcoreapp2.0\NUITemplate30.dll
1>Destination path : bin\NUITemplate30.dll
1>Source path : obj\Debug\netcoreapp2.0\NUITemplate30.pdb
1>Destination path : bin\NUITemplate30.pdb
1>Source path : shared\res\NUITemplate30.png
1>Destination path : shared\res\NUITemplate30.png
1>Source path : tizen-manifest.xml
1>Destination path : tizen-manifest.xml
1>c:\users\samsung\Source\Repos\NUITemplate30\NUITemplate30\NUITemplate30.csproj(46,5): error : Error Occured YourTarget!!
1>Done building project "NUITemplate30.csproj" -- FAILED.
========== Build: 0 succeeded or up-to-date, 1 failed, 0 skipped ==========
```

## See Also
- [Tizen.NET.Sdk Reference](tizen.net.sdk-reference.md)
