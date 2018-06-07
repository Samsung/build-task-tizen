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
    public class ExcludeTest : SdkTest
    {
        public ExcludeTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void DoNotExclude()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("02-TizenBasicNuiAppWithRefProj")
                .WithSource()
                .Restore(Log, "NUITemplate");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "NUITemplate");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute("/p:TizenTpkExcludePattern=''")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Sub("tpkroot").Should().HaveFiles(new[] {
                "tizen-manifest.xml",
                "bin/NUITemplate.dll",
                "bin/NUITemplate.pdb",
                "bin/ClassLibrary1.dll",
                "bin/ClassLibraryVD.dll",
            });
        }

        [Fact]
        public void ExcludeWildCard()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("02-TizenBasicNuiAppWithRefProj")
                .WithSource()
                .Restore(Log, "NUITemplate");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "NUITemplate");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute("/p:TizenTpkExcludePattern=ClassLibrary*.dll")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Sub("tpkroot").Should().NotHaveFiles(new[] {
                "bin/ClassLibrary1.dll",
                "bin/ClassLibraryVD.dll",
            });
        }

        [Fact]
        public void ExcludeAndInclude()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("02-TizenBasicNuiAppWithRefProj")
                .WithSource()
                .Restore(Log, "NUITemplate");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "NUITemplate");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute("/p:TizenTpkExcludePattern=ClassLibrary*.dll",
                         "/p:TizenTpkNotExcludePattern=ClassLibrary1.dll")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Sub("tpkroot").Should().NotHaveFiles(new[] {
                "bin/ClassLibraryVD.dll",
            });
            outputDirectory.Sub("tpkroot").Should().HaveFiles(new[] {
                "bin/ClassLibrary1.dll",
            });
        }
    }
}
