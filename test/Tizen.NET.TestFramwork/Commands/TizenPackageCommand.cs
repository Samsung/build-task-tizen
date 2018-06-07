using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Cli.Utils;
using Xunit.Abstractions;

namespace Tizen.NET.TestFramework.Commands
{
    public sealed class TizenPackageCommand : MSBuildCommand
    {
        public TizenPackageCommand(ITestOutputHelper log, string projectRootPath, string relativePathToProject = null, MSBuildTest msbuild = null)
            : base(log, "TizenPackage", projectRootPath, relativePathToProject, msbuild)
        {
        }
    }
}
