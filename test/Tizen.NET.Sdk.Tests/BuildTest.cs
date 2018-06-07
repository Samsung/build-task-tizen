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
    public class BuildTest : SdkTest
    {
        public BuildTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void BuildTpk()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");
        }

        [Fact]
        public void DoNotBuildTpk()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute("/p:TizenCreateTpkOnBuild=false")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().NotHaveDirectory("tpkroot");
            outputDirectory.Should().NotHaveFile("TizenBasicNuiApp-1.0.0.tpk");
        }

        [Fact(Skip = "Feature Out")]
        public void BuildTpkWithTizenPackageAndSign()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute("/t:TizenPackageAndSign", "/p:TizenCreateTpkOnBuild=false")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");
        }

        [Fact]
        public void BuildTpkWithTizenPackage()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new TizenPackageCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");

            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");
        }

        [Fact]
        public void BuildTpkWithOutManifest()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("08-TizenNuiAppNoManfest")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0001");
        }

        [Fact]
        public void BuildLibProj()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("10-TizenLib")
                .WithSource()
                .Restore(Log, "TizenLib");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenLib");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();
        }

        [Fact(Skip ="ignore")]
        public void BuildTFM()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("09-TizenTFMApp")
                .WithSource()
                .Restore(Log, "TizenTFMApp", "/p:PackageTargetFallback=netcoreapp2.0");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenTFMApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "tizen40");

            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");
        }
    }
}
