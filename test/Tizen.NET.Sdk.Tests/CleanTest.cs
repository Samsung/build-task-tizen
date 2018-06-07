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
    public class CleanTest : SdkTest
    {
        public CleanTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void Clean()
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

            buildCommand
                .GetOutputDirectory(targetFramework: "netcoreapp2.0")
                .Should()
                .HaveFile("TizenBasicNuiApp-1.0.0.tpk");

            var cleanCommand = new MSBuildCommand(Log, "Clean", projectDirectory);
            cleanCommand
                .Execute()
                .Should()
                .Pass();

            cleanCommand
                .GetOutputDirectory(targetFramework: "netcoreapp2.0")
                .Should()
                .NotHaveFile("TizenBasicNuiApp-1.0.0.tpk")
                .And
                .NotHaveDirectory("tpkroot");
        }
    }

}
