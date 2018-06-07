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
    public class EventTest : SdkTest
    {
        public EventTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void Event()
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
                .Pass()
                .And
                .HaveStdOutContaining("TestPrePackageEvent!!")
                .And
                .HaveStdOutContaining("TestPostPackageEvent!!");
        }
    }
}
