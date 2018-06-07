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
using Tizen.NET.TestFramework;
using Tizen.NET.TestFramework.Assertions;
using Tizen.NET.TestFramework.Commands;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace Tizen.NET.Sdk.Tests
{
    public class MultiAppTpkTest : SdkTest
    {
        public MultiAppTpkTest(ITestOutputHelper log) : base(log)
        {

        }

        [Fact]
        public void BuildMultiAppProj()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("11-TizenMultiApp")
                .WithSource()
                .Restore(Log, "Main");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "Main");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute()
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().HaveFile("org.tizen.example.MainProject-1.0.0.tpk");
            outputDirectory.Sub("tpkroot").Should().OnlyHaveFiles(new[] {
                "tizen-manifest.xml",
                "bin/ClassLibrary1.pdb",
                "bin/ClassLibrary1.dll",
                "bin/ClassLibrary2.pdb",
                "bin/ClassLibrary2.dll",
                "bin/SubProject1.dll",
                "bin/SubProject1.pdb",
                "bin/SubProject2.dll",
                "bin/SubProject2.pdb",
                "bin/MainProject.dll",
                "bin/MainProject.pdb",
                "bin/Newtonsoft.Json.dll",
                "bin/ko-KR/ClassLibrary1.resources.dll",
                "bin/en-GB/SubProject1.resources.dll",
                "res/ResourceShouldBeInclude.bmp",
                "shared/res/icon_main.png",
                "shared/res/icon_sub1.png",
                "shared/res/icon_sub2.png",
                "author-signature.xml",
                "signature1.xml"
            });
        }
    }
}
