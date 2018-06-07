/*
 * Copyright 2017 (c) Samsung Electronics Co., Ltd  All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * 	http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.IO;
using System.Runtime.InteropServices;
using Tizen.NET.TestFramework;
using Tizen.NET.TestFramework.Assertions;
using Tizen.NET.TestFramework.Commands;
using Xunit;
using static Tizen.NET.TestFramework.Commands.MSBuildTest;
using System.Xml.Linq;
using System.Linq;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tizen.NET.Sdk.Tests
{
    public class PackageTests : SdkTest
    {
        public PackageTests(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void PackageWithRes()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("04-TizenBasicNuiAppWithRes")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().OnlyHaveFiles(new[] {
                "tpkroot/tizen-manifest.xml",
                "tpkroot/bin/TizenBasicNuiApp.dll",
                "tpkroot/bin/TizenBasicNuiApp.pdb",
                "tpkroot/shared/res/TizenBasicNuiApp.png",
                "tpkroot/author-signature.xml",
                "tpkroot/signature1.xml",
                "tpkroot/res/tizen.png",
                "TizenBasicNuiApp-1.0.0.tpk",
                "TizenBasicNuiApp.deps.json",
                "TizenBasicNuiApp.dll",
                "TizenBasicNuiApp.pdb",
                "TizenBasicNuiApp.runtimeconfig.dev.json",
                "TizenBasicNuiApp.runtimeconfig.json",
            });
        }

        [Fact]
        public void PackageWithReferenceProj()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("02-TizenBasicNuiAppWithRefProj")
                .WithSource()
                .Restore(Log, "NUITemplate");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "NUITemplate");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Sub("tpkroot").Should().OnlyHaveFiles(new[] {
                "tizen-manifest.xml",
                "bin/ClassLibraryVD.pdb",
                "bin/ClassLibraryVD.dll",
                "bin/ClassLibrary1.dll",
                "bin/NUITemplate.dll",
                "bin/NUITemplate.pdb",
                "bin/Newtonsoft.Json.dll",
                "bin/ko-KR/ClassLibrary1.resources.dll",
                "bin/en-GB/NUITemplate.resources.dll",
                "res/ResourceShouldBeExclude.bmp",
                "shared/res/NUITemplate10.png",
                "author-signature.xml",
                "signature1.xml"
            });
        }

        [Fact(Skip = "Feature Out")]
        public void AutoExcludeTizenXX()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("03-TizenBasicNuiAppWithRefTizen")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Sub("tpkroot").Should().NotHaveFile("bin/Tizen.NUI.dll");
        }

        [Fact]
        public void PackageFailDueToWrongExec()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("05-TizenBasicNuiAppMultiExec")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0002");
        }
    }
}
