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


namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// An implementer of <see cref="IAskUser"/> is capable of 
    /// asking the user a question.
    /// </summary>
    public interface IAskUser
    {
        /// <summary>
        /// Asks the user the specified <paramref name="question"/> which could get
        /// answered with yes or no.
        /// </summary>
        /// <param name="context">The context around this question.</param>
        /// <param name="question">The question.</param>
        /// <returns>true uf the user anwered with yes, false if he answered with no.</returns>
        bool AskYesOrNoQuestion(string context, string question);
    }
}
