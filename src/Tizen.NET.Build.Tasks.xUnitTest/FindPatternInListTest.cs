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
using Tizen.NET.TestFramework.Assertions;
using Xunit;
using System.Xml.Linq;
using System.Linq;
using FluentAssertions;
using Xunit.Abstractions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text.RegularExpressions;

namespace Tizen.NET.Build.Tasks.XUnitTest
{
    public class FindPatternInListTest
    {
        public ITestOutputHelper Log { get; }

        public FindPatternInListTest(ITestOutputHelper log)
        {
            Log = log;
        }

        [Fact]
        public void ExactlyMatch()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\def\Tizen.dll"),
                new TaskItem(@"Tizen\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.Application.dll"),
                new TaskItem(@"abc\def\Test.Application.dll"),
                new TaskItem(@"abc\def\MyTest.dll"),
                new TaskItem(@"Tizen\def\MyTest.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"Tizen.dll";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(2);

            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\def\Tizen.dll",
                    @"Tizen\def\Tizen.dll"
                });
        }

        [Fact]
        public void WildCard()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\def\Tizen.dll"),
                new TaskItem(@"Tizen\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.Application.dll"),
                new TaskItem(@"abc\def\Test.Application.dll"),
                new TaskItem(@"abc\def\MyTest.dll"),
                new TaskItem(@"Tizen\def\MyTest.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"Tizen*.dll";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(3);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\def\Tizen.dll",
                    @"Tizen\def\Tizen.dll",
                    @"abc\def\Tizen.Application.dll"
                });
        }

        [Fact]
        public void MultiPattern()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\def\Tizen.dll"),
                new TaskItem(@"Tizen\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.Application.dll"),
                new TaskItem(@"abc\def\Test.Application.dll"),
                new TaskItem(@"abc\def\MyTest.dll"),
                new TaskItem(@"Tizen\def\MyTest.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"Tizen*.dll;Test*.dll";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(4);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\def\Tizen.dll",
                    @"Tizen\def\Tizen.dll",
                    @"abc\def\Tizen.Application.dll",
                    @"abc\def\Test.Application.dll"
                });

            /// Second Test
            myTask = new Tasks.FindPatternInList();
            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"Tizen*.dll;
                                Test*.dll;
                                 ;";
            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(4);
        }

        [Fact]
        public void MultiWildCard()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.pdb"),
                new TaskItem(@"Tizen\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.Application.dll"),
                new TaskItem(@"abc\def\Test.Application.dll"),
                new TaskItem(@"abc\def\MyTest.dll"),
                new TaskItem(@"Tizen\def\MyTest.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"Tizen*.*";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(4);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\def\Tizen.dll",
                    @"abc\def\Tizen.pdb",
                    @"Tizen\def\Tizen.dll",
                    @"abc\def\Tizen.Application.dll"
                });
        }

        [Fact]
        public void MultiWildCard2()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.pdb"),
                new TaskItem(@"Tizen\def\Tizen.dll"),
                new TaskItem(@"abc\def\Tizen.Application.dll"),
                new TaskItem(@"abc\def\Test.Application.dll"),
                new TaskItem(@"abc\def\MyTest.dll"),
                new TaskItem(@"Tizen\abc\TizenTizen.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"**\def\*Tizen*.dll";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(3);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\def\Tizen.dll",
                    @"Tizen\def\Tizen.dll",
                    @"abc\def\Tizen.Application.dll"
                });
        }

        [Fact]
        public void WildCardWithDir()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"abc\bin\Tizen.Application.dll"),
                new TaskItem(@"abc\lib\Test.Application.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"bin\Tizen*.dll";


            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(1);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"abc\bin\Tizen.Application.dll"
                });
        }

        [Fact]
        public void LinuxType()
        {
            ITaskItem[] itemList = new ITaskItem[]
            {
                new TaskItem(@"/Tizen.dll"),
                new TaskItem(@"abc/def/Tizen.dll"),
                new TaskItem(@"abc/def/Tizen.pdb"),
                new TaskItem(@"Tizen/def/Tizen.dll"),
                new TaskItem(@"abc/def/Tizen.Application.dll"),
                new TaskItem(@"abc/def/Test.Application.dll"),
                new TaskItem(@"abc/def/MyTest.dll"),
                new TaskItem(@"Tizen/abc/TizenTizen.dll"),
            };

            Tasks.FindPatternInList myTask = new Tasks.FindPatternInList();

            myTask.BuildEngine = new MockBuild();
            myTask.List = itemList;
            myTask.Patterns = @"**/def/*Tizen*.dll;Tizen.dll";

            Assert.True(myTask.Execute());

            myTask.MatchList.Count().Should().Be(4);
            myTask.MatchList
                .Select(x => x.ItemSpec)
                .Should()
                .Contain(new string[]
                {
                    @"/Tizen.dll",
                    @"abc/def/Tizen.dll",
                    @"Tizen/def/Tizen.dll",
                    @"abc/def/Tizen.Application.dll"
                });
        }
    }
}

