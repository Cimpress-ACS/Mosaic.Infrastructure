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
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Database;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class DbChangesTests
    {
        [Test]
        public void CompareRealObjectExample_FilledDbChanges_ComparedTo_EmptyDbChanges_ShouldBeFalse()
        {
            DbChanges a = new DbChanges();
            a.Data.Add("State_StateId", 4);
            a.Data.Add("Message", "RUN");
            a.Data.Add("Date", DateTime.Now);
            DbChanges b = new DbChanges();

            a.Equals(b).Should().BeFalse();
        }

        [Test]
        public void CompareRealObjectExample_SimilarDbChanges_ShouldBeTrue()
        {
            var dt = DateTime.Now;
            DbChanges a = new DbChanges();
            a.Data.Add("State_StateId", 4);
            a.Data.Add("Message", "RUN");
            a.Data.Add("Date", dt);
            DbChanges b = new DbChanges();
            b.Data.Add("State_StateId", 4);
            b.Data.Add("Message", "RUN");
            b.Data.Add("Date", dt);

            a.Equals(b).Should().BeTrue();
        }

        [Test]
        public void CompareRealObjectExample_SlightlyDifferentDbChanges_ShouldBeTrue()
        {
            var dt = DateTime.Now;
            DbChanges a = new DbChanges();
            a.Data.Add("State_StateId", 4);
            a.Data.Add("Message", "RUN");
            a.Data.Add("Date", dt);
            DbChanges b = new DbChanges();
            b.Data.Add("State_StateId", 5);
            b.Data.Add("Message", "RUN");
            b.Data.Add("Date", dt);

            a.Equals(b).Should().BeFalse();
        }
    }
}
