# Signing tpk with `Tizen.NET.Sdk`
## Overview of the tpk signing
Before installing your application on a device or submitting it to the Tizen Store, it must be signed with a certificate profile. 

The certificate profile consists of an author certificate and 1 or 2 distributor certificates. To distribute your application, you must create a certificate profile and sign the application with it:

## Sign with Default Certificate

### For develop/debug
Basically Tizen.NET.Sdk signs the tpk with the default certificate files, if no certificate information is provided.
So you can create tpk for development without your certificates.
> However, if you submit to Tizen Store, you must sign with a certificate profile.
```
$ dotnet build
  testconsole -> /home/develop/testconsole/bin/Debug/netcoreapp2.0/testconsole.dll
  testconsole -> /home/develop/testconsole/bin/Debug/netcoreapp2.0/org.tizen.example.testconsole-1.0.0.tpk

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Change Privilege level
>TODO feature

## Sign with User Certificate
If you are developing a Tizen application on VS2017, you can manage your certificates through `Certificate Manager`. (see the [certificate manager](https://github.sec.samsung.net/dotnet/vs-tools-cps/blob/master/docs/tools/certificate-manager.md))
Then you can build with your cetificate in Visual Studio.

Otherwise, you can pass the certificate information to `Tizen.NET.Sdk` via the property.

```
$ dotnet build /p:"AuthorPath=YourAuthorCert.p12" \
               /p:"AuthorPass=YourAuthorPassword" \
               /p:"DistributorPath=YourDistributorCert.p12" \
               /p:"DistributorPass=YourDistributorPassword"
```

> INFO : you can also set certificate information at .csproj file
```
<PropertyGroup>
 <AuthorPath>author_test.p12</AuthorPath>
 <AuthorPass>author_test</AuthorPass>
 <DistributorPath>tizen-distributor-signer.p12</DistributorPath>
 <DistributorPass>tizenpkcs12passfordsigner</DistributorPass>
</PropertyGroup>
```

## See Also
- [How to Create User Certificate](https://github.sec.samsung.net/dotnet/vs-tools-cps/blob/master/docs/tools/certificate-manager.md)
- [How to Build a Tizen project in Visual Studio](https://github.sec.samsung.net/dotnet/vs-tools-cps/blob/master/docs/packaging/how-to-build-vs.md)
- [How to Build a Tizen project in dotnet cli](https://github.sec.samsung.net/dotnet/vs-tools-cps/blob/master/docs/packaging/how-to-build-cli.md)
