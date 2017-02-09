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
using System.Linq;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Removes all non digit characters from input string.
    /// </summary>
    public static class StringExtensions
    {
        public static string ToDigitsOnly(this string input)
        {
            return new String(input.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Increases any number within the string even if it's mixed with non digit characters.
        /// </summary>
        /// <remarks>
        /// E.g. the input string "A105-X" will return "A106-X".
        /// </remarks>
        /// <returns></returns>
        public static string IncreaseAnyNumber(this string input)
        {
            bool alreadyIncreased = false;
            int i = input.Length - 1;
            string result = string.Empty;

            while (i >= 0)
            {
                char character = input[i];

                if (!alreadyIncreased && char.IsDigit(character))
                {
                    int digit = (int)Char.GetNumericValue(character);
                    if (digit == 9)
                    {
                        digit = 0;
                    }
                    else
                    {
                        alreadyIncreased = true;
                        ++digit;
                    }

                    result = digit + result;
                }
                else
                {
                    result = character + result;
                }

                --i;
            }

            return result;
        }
    }
}
