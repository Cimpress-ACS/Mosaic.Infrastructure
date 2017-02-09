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
using System.Collections.Generic;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    /// <summary>
    /// Container for a PLC variable with metadata.
    /// </summary>
    public class Tag
    {
        private readonly ICollection<Tag> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag()
        {
            _children = new List<Tag>();
        }

        /// <summary>
        /// Some PLC variables are complex types like arrays or structures.
        /// These variables might provide child tags. Tag trees are possible.
        /// </summary>
        public IEnumerable<Tag> Children
        {
            get { return _children.ToReadOnly(); }
            set
            {
                _children.Clear();
                foreach (Tag child in value)
                    _children.Add(child);
            }
        }

        /// <summary>
        /// Attached developer comment of the tag. It normally contains a short description.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Some PLC variables are readonly, e.g. motor speed.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Tag name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// .NET type equivalent.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Unit of the value, see Plc Base concept for all units.
        /// Usual units are m, mm, um, ms, ns or s.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Tag value. Cast is possible, it depends of the Type property.
        /// </summary>
        public object Value { get; set; }

        public List<EnumerationMember> EnumerationMembers { get; set; }

        public override string ToString()
        {
            return string.Format("Tag: Name = '{0}'", Name);
        }
    }

    public class EnumerationMember
    {
        public short Value { get; set; }
        public string Comment { get; set; }
    }
}
