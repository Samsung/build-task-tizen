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
using System.Reflection;

namespace Tizen.NET.Build.Tasks.XUnitTest
{
    public class GetManifestInfoTest
    {
        public ITestOutputHelper Log { get; }

        public GetManifestInfoTest(ITestOutputHelper log)
        {
            Log = log;
        }

        [Fact]
        public void GetSingleInfo()
        {
            string testManfiestFile = "test1.xml";

            XElement testmanifest = XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.pkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.appid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                </manifest>");

            File.WriteAllText(testManfiestFile, testmanifest.ToString(SaveOptions.DisableFormatting));

            Tasks.GetManifestInfo myTask = new Tasks.GetManifestInfo();

            myTask.BuildEngine = new MockBuild();

            myTask.ManifestFilePath = testManfiestFile;

            Assert.True(myTask.Execute());

            Log.WriteLine("Error Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Log.WriteLine("Warning Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).WarningLog);
            Log.WriteLine("Message Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).MessageLog);

            Assert.Equal("org.test.pkgid", myTask.TpkName);
            Assert.Equal("1.0.0", myTask.TpkVersion);

            myTask.TpkExecList.Count().Should().Be(1);
            myTask.TpkExecList.Where(x => x.ItemSpec.Equals("TizenExec.dll")).Count().Should().Be(1);
        }

        [Fact]
        public void GetMultiInfo()
        {
            string testManfiestFile = "test1.xml";

            XElement testmanifest = XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.pkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.appid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <ui-application appid='org.test.pkgid.appid2' exec='TizenExec2.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                </manifest>");

            File.WriteAllText(testManfiestFile, testmanifest.ToString(SaveOptions.DisableFormatting));

            Tasks.GetManifestInfo myTask = new Tasks.GetManifestInfo();

            myTask.BuildEngine = new MockBuild();

            myTask.ManifestFilePath = testManfiestFile;

            Assert.True(myTask.Execute());

            Log.WriteLine("Error Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Log.WriteLine("Warning Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).WarningLog);
            Log.WriteLine("Message Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).MessageLog);

            //Assert
            Assert.Equal("org.test.pkgid", myTask.TpkName);
            Assert.Equal("1.0.0", myTask.TpkVersion);

            myTask.TpkExecList.Count().Should().Be(2);
            myTask.TpkExecList.Where(x => x.ItemSpec.Equals("TizenExec.dll")).Count().Should().Be(1);
            myTask.TpkExecList.Where(x => x.ItemSpec.Equals("TizenExec2.dll")).Count().Should().Be(1);
        }

        [Fact]
        public void GetMultiInfoWrong()
        {
            string testManfiestFile = "test1.xml";

            XElement testmanifest = XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.pkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.appid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <ui-application appid='org.test.pkgid.appid2' exec='TizenExec2.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <ui-application appid='org.test.pkgid.appid3' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                    <label>TizenBasicNuiApp</label>
                    <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                </manifest>");

            File.WriteAllText(testManfiestFile, testmanifest.ToString(SaveOptions.DisableFormatting));

            Tasks.GetManifestInfo myTask = new Tasks.GetManifestInfo();

            myTask.BuildEngine = new MockBuild();

            myTask.ManifestFilePath = testManfiestFile;

            Assert.True(myTask.Execute());

            Log.WriteLine("Error Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Log.WriteLine("Warning Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).WarningLog);
            Log.WriteLine("Message Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).MessageLog);

            //Assert
            Assert.Equal("org.test.pkgid", myTask.TpkName);
            Assert.Equal("1.0.0", myTask.TpkVersion);

            myTask.TpkExecList.Count().Should().Be(2);
            myTask.TpkExecList.Where(x => x.ItemSpec.Equals("TizenExec.dll")).Count().Should().Be(1);
            myTask.TpkExecList.Where(x => x.ItemSpec.Equals("TizenExec2.dll")).Count().Should().Be(1);
        }
    }
}
