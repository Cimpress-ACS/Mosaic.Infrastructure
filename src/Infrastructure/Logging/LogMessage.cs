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

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    public class LogMessage
    {
        public enum CategoryEnum
        {
            Debug, Info, Warning, Error,
        }

        private readonly string _emitter;
        private readonly CategoryEnum _category;
        private readonly string _text;
        private readonly Exception _exception;

        public LogMessage(string context, CategoryEnum category, string text, Exception exception)
        {
            _emitter = context;
            _category = category;
            _text = text;
            _exception = exception;
        }

        public string Emitter
        {
            get { return _emitter; }
        }

        public CategoryEnum Category
        {
            get { return _category; }
        }

        public string Text
        {
            get { return _text; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _category, _emitter, _text);
        }
    }
}
