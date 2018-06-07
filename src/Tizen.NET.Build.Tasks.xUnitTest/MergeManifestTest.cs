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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Tizen.NET.Build.Tasks.XUnitTest
{
    public class MergeManifestTest
    {
        public ITestOutputHelper Log { get; }

        public MergeManifestTest(ITestOutputHelper log)
        {
            Log = log;
        }

        [Fact]
        public void MergeOneSub()
        {
            string subManfiestFile = "sub1.xml";
            string mainManfiestFile = "main.xml";
            string resultManfiestFile = "result.xml";

            XElement submanifest = XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.subpkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.subappid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                        <label>TizenBasicNuiApp</label>
                        <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <privileges>
                       <privilege>http://tizen.org/privilege/testsub</privilege>
                       <privilege>http://tizen.org/privilege/testcommon</privilege>
                    </privileges>
                    <feature name='http://tizen.org/feature/testsub'>true</feature>
                    <feature name='http://tizen.org/feature/testcommon'>true</feature>
                </manifest>");

            XElement mainmanifest = XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.pkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.mainappid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                        <label>TizenBasicNuiApp</label>
                        <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <privileges>
                       <privilege>http://tizen.org/privilege/testmain</privilege>
                       <privilege>http://tizen.org/privilege/testcommon</privilege>
                    </privileges>
                    <feature name='http://tizen.org/feature/testmain'>true</feature>
                    <feature name='http://tizen.org/feature/testcommon'>true</feature>
                </manifest>");

            File.Delete(subManfiestFile);
            File.Delete(mainManfiestFile);
            File.WriteAllText(subManfiestFile, submanifest.ToString(SaveOptions.DisableFormatting));
            File.WriteAllText(mainManfiestFile, mainmanifest.ToString(SaveOptions.DisableFormatting));

            ITaskItem[] subManifestitem = new TaskItem[]{ new TaskItem(subManfiestFile) };

            Tasks.MergeManifest myTask = new Tasks.MergeManifest();

            myTask.BuildEngine = new MockBuild();
            myTask.MainManifestFile = mainManfiestFile;
            myTask.SubManifestFileList = subManifestitem;
            myTask.ResultManifestFile = resultManfiestFile;

            Assert.True(myTask.Execute());

            Log.WriteLine("Error Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).ErrorLog);
            Log.WriteLine("Warning Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).WarningLog);
            Log.WriteLine("Message Log");
            Log.WriteLine(((MockBuild)myTask.BuildEngine).MessageLog);

            var result = XDocument.Load(resultManfiestFile);

            Assert.True(XNode.DeepEquals(result.Root, XElement.Parse(
                @"<manifest xmlns='http://tizen.org/ns/packages' api-version='3' package='org.test.pkgid' version='1.0.0'>
                    <profile name='common' />
                    <ui-application appid='org.test.pkgid.mainappid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                        <label>TizenBasicNuiApp</label>
                        <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <ui-application appid='org.test.pkgid.subappid' exec='TizenExec.dll' type='dotnet' multiple='false' taskmanage='true' nodisplay='false' launch_mode='single'>
                        <label>TizenBasicNuiApp</label>
                        <icon>TizenBasicNuiApp.png</icon>
                    </ui-application>
                    <privileges>
                       <privilege>http://tizen.org/privilege/testmain</privilege>
                       <privilege>http://tizen.org/privilege/testcommon</privilege>
                       <privilege>http://tizen.org/privilege/testsub</privilege>
                    </privileges>
                    <feature name='http://tizen.org/feature/testmain'>true</feature>
                    <feature name='http://tizen.org/feature/testcommon'>true</feature>
                    <feature name='http://tizen.org/feature/testsub'>true</feature>
                </manifest>")));
        }
    }
}
