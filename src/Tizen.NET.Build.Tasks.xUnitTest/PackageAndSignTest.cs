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

using Xunit;
using Microsoft.Build.Utilities;
using Tizen.NET.Build.Tasks;
using System;
using System.IO;
using Xunit.Abstractions;
using Microsoft.DotNet.PlatformAbstractions;

namespace Tizen.NET.Build.Tasks.XUnitTest
{
    public class PackageAndSignTest
    {
        private readonly ITestOutputHelper _output;

        public PackageAndSignTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void PackageTest()
        {
            Package myPackageTask = new Package();
            myPackageTask.BuildEngine = new MockBuild();

            myPackageTask.TpkSrcPath = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "tpkroot");
            myPackageTask.UnSignedTpkFile = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "test.tpk");

            Assert.True(myPackageTask.Execute());
            Assert.True(File.Exists("test.tpk"));
        }

        [Fact]
        public void SignTestWithTpkWithSignedTpk()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test.p12");
            myTask.AuthorPassword = "author_test";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigner";
            myTask.RootDir = projPath + "\\MediaHub.TizenMobile.tpk";
            myTask.OutputSignedTpk = ApplicationEnvironment.ApplicationBasePath + "\\MediaHub.TizenMobile-signed.tpk";

            Assert.True(myTask.Execute());
            Assert.True(File.Exists(myTask.OutputSignedTpk));
        }

        [Fact]
        public void SignTestWithTpkWithoutSignedTpk()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test.p12");
            myTask.AuthorPassword = "author_test";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigner";
            myTask.RootDir = projPath + "\\MediaHub.TizenMobile.tpk";

            Assert.True(myTask.Execute());
        }

        [Fact]
        public void SignTestWithDirWithoutSignedTpk()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test.p12");
            myTask.AuthorPassword = "author_test";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigner";
            myTask.RootDir = projPath + "\\tpkroot";

            Assert.True(myTask.Execute());
        }

        [Fact]
        public void SignTestWithDirWithSignedTpk()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"\author_test.p12");
            myTask.AuthorPassword = "author_test";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigner";
            myTask.RootDir = projPath + "\\tpkroot";
            myTask.OutputSignedTpk = ApplicationEnvironment.ApplicationBasePath + "\\MediaHub.TizenMobile-signed1.tpk";

            Assert.True(myTask.Execute());
            Assert.True(File.Exists(myTask.OutputSignedTpk));
        }

        [Fact]
        public void SignTestWithWrongPassword()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test.p12");
            myTask.AuthorPassword = "author_testaaa";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigneraaa";
            myTask.RootDir = projPath + "\\MediaHub.TizenMobile.tpk";
            myTask.OutputSignedTpk = ApplicationEnvironment.ApplicationBasePath + "\\MediaHub.TizenMobile-signed.tpk";

            bool ret = myTask.Execute();
            Assert.NotNull(((MockBuild)myTask.BuildEngine).ErrorLog);

            _output.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Assert.False(ret);
        }

        [Fact]
        public void SignTestWithWrongFilePath()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test11111.p12");
            myTask.AuthorPassword = "author_testaaa";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigneraaa";
            myTask.RootDir = projPath + "\\MediaHub.TizenMobile.tpk";
            myTask.OutputSignedTpk = ApplicationEnvironment.ApplicationBasePath + "\\MediaHub.TizenMobile-signed.tpk";

            bool ret = myTask.Execute();
            Assert.NotNull(((MockBuild)myTask.BuildEngine).ErrorLog);

            _output.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Assert.False(ret);
        }

        [Fact]
        public void SignTestWithMultiDistFile()
        {
            Sign myTask = new Sign();
            myTask.BuildEngine = new MockBuild();

            string projPath = TaskUtility.GetProjectPath();
            string solutionPath = TaskUtility.GetSolutionPath();

            myTask.AuthorFile = Path.GetFullPath(projPath + @"author_test.p12");
            myTask.AuthorPassword = "author_test";
            myTask.DistFile = Path.GetFullPath(projPath + @"tizen-distributor-signer.p12");
            myTask.DistPassword = "tizenpkcs12passfordsigner";
            myTask.DistFile2 = Path.GetFullPath(projPath + @"tizen-distributor-partner-manufacturer-signer.p12");
            myTask.DistPassword2 = "tizenpkcs12passfordsigner";
            myTask.RootDir = projPath + "\\MediaHub.TizenMobile.tpk";
            myTask.OutputSignedTpk = ApplicationEnvironment.ApplicationBasePath + "\\MediaHub.TizenMobile-signed.tpk";

            bool ret = myTask.Execute();
            Assert.NotNull(((MockBuild)myTask.BuildEngine).ErrorLog);

            _output.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Assert.True(ret);
        }
    }
}

