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


using System.Collections;
using Newtonsoft.Json;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// The Redis log DTO, provided by MSW Infrastructure squad, extended with additional fields
    /// </summary>
    [JsonObject]
    class RedisJsonLog
    {
        [JsonProperty]
        internal string Team { get; set; }

        [JsonProperty]
        internal string LogOwner { get; set; }

        [JsonProperty]
        internal string Environment { get; set; }

        [JsonProperty]
        internal string Host { get; set; }

        [JsonProperty("@timestamp")] //Elasticsearch special field
        internal string TimeStamp { get; set; }

        [JsonProperty("type")] //Elasticsearch special field
        internal string Type { get; set; }

        //[JsonProperty]
        //public object Data { get; set; }

        [JsonProperty]
        internal object Message { get; set; }

        [JsonProperty]
        public int ProcessId { get; set; }

        [JsonProperty]
        internal int MessageHash { get; set; }

        [JsonProperty]
        internal string RenderedMessage { get; set; }

        [JsonProperty]
        internal object ExceptionData { get; set; }

        [JsonProperty]
        internal string Level { get; set; }

        [JsonProperty]
        internal string Application { get; set; }

        [JsonProperty]
        internal string Executable { get; set; }

        [JsonProperty]
        internal IDictionary Properties { get; set; }

        #region Additional Properties
        [JsonProperty]
        internal string Organization { get; set; }

        [JsonProperty]
        internal string ApplicationSubpart { get; set; }

        [JsonProperty]
        internal string Exception { get; set; }

        [JsonProperty]
        internal string ExceptionMessage { get; set; }

        [JsonProperty]
        internal string ExceptionStackTrace { get; set; }

        [JsonProperty]
        internal int ManagedThreadId { get; set; }

        [JsonProperty]
        internal string ManagedThreadName { get; set; }

        [JsonProperty]
        internal string Instance { get; set; }
        #endregion
    }
}
