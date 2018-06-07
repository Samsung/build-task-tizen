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
    public class SignTest : SdkTest
    {
        public SignTest(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void SignWithDefaultCert()
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

            outputDirectory.Sub("tpkroot").Should().HaveFiles(new[] {
                "author-signature.xml",
                "signature1.xml",
            });
        }

        [Fact]
        public void SignWithCustomCert()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                        "/p:AuthorPath=author_test.p12",
                        "/p:AuthorPass=author_test",
                        "/p:DistributorPath=tizen-distributor-signer.p12",
                        "/p:DistributorPass=tizenpkcs12passfordsigner")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");

            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");

            outputDirectory.Sub("tpkroot").Should().HaveFiles(new[] {
                "author-signature.xml",
                "signature1.xml",
            });
        }

        [Fact]
        public void SignWithMultipleDist()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                    "/p:AuthorPath=author_test.p12",
                    "/p:AuthorPass=author_test",
                    "/p:DistributorPath=tizen-distributor-signer.p12",
                    "/p:DistributorPass=tizenpkcs12passfordsigner",
                    "/p:DistributorPath2=tizen-distributor-partner-manufacturer-signer.p12",
                    "/p:DistributorPass2=tizenpkcs12passfordsigner")
                .Should()
                .Pass();

            var outputDirectory = buildCommand.GetOutputDirectory(targetFramework: "netcoreapp2.0");
            outputDirectory.Should().HaveFile("TizenBasicNuiApp-1.0.0.tpk");

            outputDirectory.Sub("tpkroot").Should().HaveFiles(new[] {
                "author-signature.xml",
                "signature1.xml",
                "signature2.xml",
            });
        }

        [Fact]
        public void SignWithWrongPasswordAuthor()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                        "/p:AuthorPath=author_test.p12",
                        "/p:AuthorPass=wrongpassword",
                        "/p:DistributorPath=tizen-distributor-signer.p12",
                        "/p:DistributorPass=tizenpkcs12passfordsigner")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0005");
        }

        [Fact]
        public void SignWithWrongPasswordDistributor()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                        "/p:AuthorPath=author_test.p12",
                        "/p:AuthorPass=author_test",
                        "/p:DistributorPath=tizen-distributor-signer.p12",
                        "/p:DistributorPass=wrongpassword")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0005");
        }

        [Fact]
        public void SignWithWrongPathAuthor()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                        "/p:AuthorPath=wrongfile.p12",
                        "/p:AuthorPass=author_test",
                        "/p:DistributorPath=tizen-distributor-signer.p12",
                        "/p:DistributorPass=tizenpkcs12passfordsigner")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0003");
        }

        [Fact]
        public void SignWithWrongPathDistributor()
        {
            var testAsset = _testAssetsManager
                .CopyTestAsset("01-TizenBasicNuiApp")
                .WithSource()
                .Restore(Log, "TizenBasicNuiApp");

            var projectDirectory = Path.Combine(testAsset.TestRoot, "TizenBasicNuiApp");

            var buildCommand = new BuildCommand(Log, projectDirectory);
            buildCommand
                .Execute(
                        "/p:AuthorPath=author_test.p12",
                        "/p:AuthorPass=author_test",
                        "/p:DistributorPath=wrongfile.p12",
                        "/p:DistributorPass=tizenpkcs12passfordsigner")
                .Should()
                .Fail()
                .And
                .HaveStdOutContaining("TS0004");
        }
    }
}
