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

namespace VP.FF.PT.Common.Infrastructure
{
    public static class Dictionary
    {
        public static TKey FindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary"); 

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value))
                    return pair.Key;

            throw new Exception("the value is not found in the dictionary");
        }

        public static bool TryFindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey key)
        {
            if (dictionary == null)
            {
                key = default(TKey);
                return false;
            }

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                if (value.Equals(pair.Value))
                {
                    key = pair.Key;
                    return true;
                }
            }

            key = default(TKey);
            return false;
        }
    }
}
