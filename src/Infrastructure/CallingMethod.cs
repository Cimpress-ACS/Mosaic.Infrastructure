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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Contains information about the calling method.
    /// </summary>
    public class CallingMethod
    {
        private readonly Type _ignoreType;
        private string _fileName;
        private string _filePath;
        private int _lineNumber;
        private string _methodNameFull;
        private MethodBase _method;
        private string _methodName;
        private string _methodSignature;
        private string _methodSignatureFull;
        private string _re;
        private string _text;
        private Type _type;
        private string _typeName;
        private string _typeNameFull;

        /// <summary>
        ///     Gets the calling method.
        /// </summary>
        public CallingMethod()
            : this(null)
        {
        }

        /// <summary>
        ///     Gets the calling method, ignoring calls from the specified type.
        /// </summary>
        /// <param name="ignoreType">
        ///     All calls made from this type will be ignored.
        ///     Use this when wrapping this class in another class. OK if null.
        /// </param>
        public CallingMethod(Type ignoreType)
        {
            _ignoreType = ignoreType;
            Initialize();
        }

        /// <summary>
        ///     Gets the name of the file that contained the method.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        ///     Gets the path of the file that contained the method.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        ///     Gets the type that will be ignored.
        /// </summary>
        public Type IgnoreType
        {
            get { return _ignoreType; }
        }

        /// <summary>
        ///     Gets the line number in the file that called the method.
        /// </summary>
        public int LineNumber
        {
            get { return _lineNumber; }
        }

        /// <summary>
        ///     Gets the full name of this method, with namespace.
        /// </summary>
        public string MethodNameFull
        {
            get { return _methodNameFull; }
        }

        /// <summary>
        ///     Gets the calling method.
        /// </summary>
        public MethodBase Method
        {
            get { return _method; }
        }

        /// <summary>
        ///     Gets the name of this method.
        /// </summary>
        public string MethodName
        {
            get { return _methodName; }
        }

        /// <summary>
        ///     Gets the complete method signature
        ///     with return type, full method name, and arguments.
        /// </summary>
        public string MethodSignatureFull
        {
            get { return _methodSignatureFull; }
        }

        /// <summary>
        ///     Gets the method name and arguments.
        /// </summary>
        public string MethodSignature
        {
            get { return _methodSignature; }
        }

        /// <summary>
        ///     Gets the namespace containing the object containing this method.
        /// </summary>
        public string Namespace
        {
            get
            {
                Type type = Type;
                return type == null
                           ? null
                           : type.Namespace;
            }
        }

        /// <summary>
        ///     Gets the name of the return type.
        /// </summary>
        public string ReturnName
        {
            get { return _re; }
        }

        /// <summary>
        ///     Gets the full method signature, file and line number.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        ///     Gets the full name of the type that contains this method,
        ///     including the namespace.
        /// </summary>
        public string TypeNameFull
        {
            get { return _typeNameFull; }
        }

        /// <summary>
        ///     Gets the name of the type that contains this method,
        ///     not including the namespace.
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
        }

        /// <summary>
        ///     Gets the type that contains this method.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        ///     Initializes the calling method information.
        /// </summary>
        private void Initialize()
        {
            MethodBase method;
            string ignoreName = _ignoreType == null
                                    ? null
                                    : _ignoreType.Name;

            StackFrame stackFrame = null;
            var stackTrace = new StackTrace(true);
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame sf = stackTrace.GetFrame(i);
                method = sf.GetMethod();
                string typeName = method.ReflectedType.Name;
                if (String.CompareOrdinal(typeName, "CallingMethod") != 0 &&
                    (ignoreName == null || String.CompareOrdinal(typeName, ignoreName) != 0))
                {
                    stackFrame = sf;
                    break;
                }
            }

            method = stackFrame.GetMethod();
            _method = method;
            string methodString = method.ToString();

            string returnName = null;
            string methodSignature = methodString;

            int splitIndex = methodString.IndexOf(' ');
            if (splitIndex > 0)
            {
                returnName = methodString.Substring(0, splitIndex);
                methodSignature = methodString.Substring(splitIndex + 1,
                                                         methodString.Length - splitIndex - 1);
            }
            _re = returnName;
            _methodSignature = methodSignature;

            _type = method.ReflectedType;
            _typeName = _type.Name;
            _typeNameFull = _type.FullName;

            _methodName = method.Name;
            _methodNameFull = String.Concat(
                _typeNameFull, ".", _methodName);

            _lineNumber = stackFrame.GetFileLineNumber();

            string fileLine = null;
            _filePath = stackFrame.GetFileName();
            if (!String.IsNullOrEmpty(_filePath))
            {
                _fileName = Path.GetFileName(_filePath);
                fileLine = String.Format("File={0}, Line={1}",
                                         _fileName, _lineNumber);
            }

            _methodSignatureFull = String.Format("{0} {1}.{2}",
                                                  returnName, _typeNameFull, _methodSignature);
            _text = String.Format("{0} [{1}]",
                                   _methodSignatureFull, fileLine);
        }

        /// <summary>
        ///     Gets the full method signature, file and line number.
        /// </summary>
        public override string ToString()
        {
            return Text;
        }
    }
}
