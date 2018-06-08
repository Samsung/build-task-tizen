# Packaging tpk with`Tizen.NET.Sdk`

## Overview of the tpk packaging
The tpk packaging phase is performed after the build step.
First, calculate the list of files to package and then copy the files to the tpk root directory.
Finally, `Tizen.NET.Sdk` compresses the tpk root with the package name and signs it at the last step.

## Enable generating tpk file
Add `Tizen.NET.Sdk` to  PackageReference  and build project.

- In Visual Studio 2017 : https://github.com/Samsung/vs-tools-cps/blob/master/docs/packaging/how-to-build-vs.md
- In .NET Core command-line infterface(CLI) : https://github.com/Samsung/vs-tools-cps/blob/master/docs/packaging/how-to-build-cli.md

## Include Files to tpk
### Default include files
`Tizen.NET.Sdk` Include below contents from project directory and copy to tpkroot of outputpath.
(see [Default include files](https://github.sec.samsung.net/dotnet/build-task-tizen/blob/master/doc/tizen.net.sdk-reference.md#default-tpk-package-includes-in-tizennetsdk))
You can add resources to tpk just by copying the files to reserved directory.

```
Projectroot                  
 |- res/**/*;
 |- lib**/*;
 |- shared/**/*;
 |- tizen-manifest.xml
```
If you does not want to use default include files, then you can set the `EnableDefaultTpkItems` propery to `false`, like this:
```xml
<PropertyGroup>
  <EnableDefaultTpkItems>false</EnableDefaultTpkItems>
</PropertyGroup>
```

### Include User files to tpk
If you want to add additional directories or files to your tpk , you can use `TizenTpkUserIncludeFiles` item.(see the [reference](https://github.sec.samsung.net/dotnet/build-task-tizen/wiki/Tizen.NET.Sdk-Reference#tizentpkuserincludefiles))
```xml
<ItemGroup>
  <TizenTpkUserIncludeFiles Include="data\**\*" />
</ItemGroup>
```

```xml
<ItemGroup>
     <!-- copy 'data\abc.txt' to 'tpkroot\shared\lib\abc.txt' -->
     <TizenTpkUserIncludeFiles Include="data\abc.txt">
       <TizenTpkSubDir>shared\lib</TizenTpkSubDir>
     </TizenTpkUserIncludeFiles>

     <!-- copy 'data\def.txt' to 'tpkroot\sha'red\lib1\test.dll' -->
     <TizenTpkUserIncludeFiles Include="date\def.txt">
       <TizenTpkFileName>test.dll</TizenTpkFileName>
       <TizenTpkSubDir>shared\lib1</TizenTpkSubDir>
     </TizenTpkUserIncludeFiles>

     <!-- bulk change, prepend text ('TEST-') to filename -->
     <MyData Include="data\**\*" />
 
     <TizenTpkUserIncludeFiles Include="@(MyData)">
       <TizenTpkSubDir>data\%(RecursiveDir)</TizenTpkSubDir>
       <TizenTpkFileName>TEST-%(Filename)%(Extension)</TizenTpkFileName>
     </TizenTpkUserIncludeFiles>
</ItemGroup>

```
## Exclude Files from tpk
### Exclude NuGet reference dll from tpk
If you want to exclude Specific NugetPackage's dll from your tpk package, you should add attribute ExcludeAssets and set value Runtime. Otherwise, all reference files will be included to tpk package.
This feature is supported in the Properties view in Visual Studio 2017.

```xml
<ItemGroup>
  <PackageReference Include="Tizen.NET" Version="4.0.0-preview1-00100">
    <ExcludeAssets>Runtime</ExcludeAssets>
  </PackageReference>
</ItemGroup>
```
[More Information of PackageReference Specification](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files#controlling-dependency-assets)
### Exclude project resource files from tpk
TizenTpkUserExcludeFiles item will exclude files from tpk.

Example Code :
```xml
  <ItemGroup>
    <TizenTpkUserExcludeFiles Include="res\Bitmap3.bmp" />
  </ItemGroup>
```
### Exclude specific dll from tpk
If you want to exclude specific dll file, you should write fullname of dll 
>Do not use the '\*' wildcard. '\*' can be used only when the files exists.

Example Code :
```xml
  <ItemGroup>
    <TizenTpkUserExcludeFiles Include="Tizen.My.dll" />
  </ItemGroup>
```
### Exclude Multi Files using ExcludePattern from tpk
`TizenTpkExcludePattern` and` TizenTpkNotExcludePattern` will be useful for dynamically excluding files

```xml
 <PropertyGroup>
    <!-- Exclude file pattern from tpkroot -->
    <TizenTpkExcludePattern>
      Tizen*.dll;
    </TizenTpkExcludePattern>

    <!-- Include file using pattern to tpkroot, this pattern should be use with TizenTpkExcludePattern -->
    <TizenTpkNotExcludePattern>
      Tizen.Application*.dll;
    </TizenTpkNotExcludePattern>
  </PropertyGroup>
```
## Change assembly copy path to tpk

After the project has been built, the assembly files (.dll, .pdb) will copy to `tpkroot\$(TizenTpkAssemblyDirName)`.
Default `$(TizenTpkAssemblyDirName)` value is bin.
If you want to change assembly directory of tpk then you should set `TizenTpkAssemblyDirName` property value you want.

Example Code : 
```xml
  <PropertyGroup>
    <TizenTpkAssemblyDirName>lib</TizenTpkAssemblyDirName>
  </PropertyGroup>
```

## See Also
- [Tizen.NET.Sdk properties](https://github.com/Samsung/build-task-tizen/blob/master/doc/tizen.net.sdk-reference.md#tizennetsdk-properties)
- [Tizen.NET.Sdk Items](https://github.com/Samsung/build-task-tizen/blob/master/doc/tizen.net.sdk-reference.md#tizennetsdk-items)
- [Default include files](https://github.com/Samsung/build-task-tizen/blob/master/doc/tizen.net.sdk-reference.md#default-tpk-package-includes-in-tizennetsdk)
- [Tizen.NET.Sdk Reference](https://github.com/Samsung/build-task-tizen/blob/master/doc/tizen.net.sdk-reference.md)
