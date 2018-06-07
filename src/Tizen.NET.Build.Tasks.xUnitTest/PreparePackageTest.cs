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
using Microsoft.DotNet.PlatformAbstractions;

namespace Tizen.NET.Build.Tasks.XUnitTest
{
    public class PreparePackageTest
    {
        public string projRoot;
        public string projOut;
        public string target;

        void CreateTestProject()
        {

            projRoot = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "testprojroot");
            //projRoot = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "testprojroot");

            projOut = Path.Combine(projRoot, @"bin\Debug");

            target = Path.Combine(projOut, @"test.exe");

            Directory.CreateDirectory(projRoot);

            Directory.CreateDirectory(projOut);

            if (!File.Exists(Path.Combine(projRoot, "tizen-manifest.xml")))
            {
                System.IO.FileStream fs = File.Create(Path.Combine(projRoot, "tizen-manifest.xml"));
                fs.Dispose();
            }

            if (!File.Exists(target))
            {
                System.IO.FileStream fs = File.Create(target);
                fs.Dispose();
            }
        }

        void DeleteTestProject()
        {
            if (Directory.Exists(projRoot))
            {
                Directory.Delete(projRoot, true);
            }
        }
    }
}
