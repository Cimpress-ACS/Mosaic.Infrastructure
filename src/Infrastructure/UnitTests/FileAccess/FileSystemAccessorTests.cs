/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.FileAccess;

namespace VP.FF.PT.Common.Infrastructure.UnitTests.FileAccess
{
    [TestFixture]
    public class FileSystemAccessorTests
    {
        private string _destinationFile;
        private string _sourceFile;

        [SetUp]
        public void Setup()
        {
            var tmp = Path.GetTempPath();
            _sourceFile = Path.Combine(tmp, Guid.NewGuid().ToString());
            _destinationFile = Path.Combine(tmp, Guid.NewGuid().ToString());
        }

        [TearDown]
        public void Teardown()
        {
            DeleteFile(_destinationFile);
            DeleteFile(_sourceFile);
        }

        [Test]
        public void CopyFile_succeeds_if_destination_does_not_exist()
        {
            // setup
            const int size = 10;
            var wrapper = new FileSystemAccessor();
            CreateFile(_sourceFile, size);

            // execute
            var t = wrapper.CopyAsync(_sourceFile, _destinationFile);
            t.Wait();

            // verify
            var dest = new FileInfo(_destinationFile);
            dest.Exists.Should().BeTrue();
            dest.Length.Should().Be(size);
        }

        [Test]
        public void CopyFile_succeeds_if_destination_exists()
        {
            // setup
            const int size = 10;
            var wrapper = new FileSystemAccessor();
            CreateFile(_sourceFile, size);
            CreateFile(_destinationFile, size * 2);

            // execute
            var t = wrapper.CopyAsync(_sourceFile, _destinationFile);
            t.Wait();

            // verify
            var dest = new FileInfo(_destinationFile);
            dest.Exists.Should().BeTrue();
            dest.Length.Should().Be(size);
        }

        [Test]
        public void CopyFile_fails_if_source_does_not_exist()
        {
            // setup
            var wrapper = new FileSystemAccessor();

            // execute
            var t = wrapper.CopyAsync(_sourceFile, _destinationFile);
            var ex = Assert.Throws<AggregateException>(t.Wait);

            // verify
            ex.InnerExceptions.Single().Should().BeOfType<FileNotFoundException>();
        }

        private void CreateFile(string sourceFile, int sizeInBytes)
        {
            var file = File.Create(sourceFile);

            for (int i = 0; i < sizeInBytes; i++)
            {
                unchecked
                {
                    file.WriteByte((byte)i);
                }
            }

            file.Close();
        }

        private void DeleteFile(string f)
        {
            var fi = new FileInfo(f);
            if (fi.Exists)
            {
                fi.Delete();
            }
        }
    }
}
