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
    public static class TreeExtensions
    {
        public static void VisitAllNodes<T>(this T startingPoint, Func<T, IEnumerable<T>> childsSelector, Action<T> actionToApplyOnEachNode)
            where T : class
        {
            if (startingPoint == null)
                return;
            actionToApplyOnEachNode(startingPoint);
            foreach (T child in childsSelector(startingPoint))
                child.VisitAllNodes(childsSelector, actionToApplyOnEachNode);
        }

        public static void VisitAllNodes<T>(this T startingPoint, Func<T, IEnumerable<T>> childsSelector, Func<T, bool> predicate, Action<T> actionOnSuccess)
            where T : class
        {
            if (startingPoint == null)
                return;
            if (predicate(startingPoint))
                actionOnSuccess(startingPoint);
            foreach (T child in childsSelector(startingPoint))
                child.VisitAllNodes(childsSelector, predicate, actionOnSuccess);
        }

        public static bool VisitAndAbortOnSuccess<T>(this T startingPoint, Func<T, IEnumerable<T>> childsSelector, Func<T, bool> predicate, Action<T> actionOnSuccess)
            where T : class
        {
            if (startingPoint == null)
                return false;
            if (predicate(startingPoint))
            {
                actionOnSuccess(startingPoint);
                return true;
            }
            foreach (T child in childsSelector(startingPoint))
                if (child.VisitAndAbortOnSuccess(childsSelector, predicate, actionOnSuccess))
                    break;
            return false;
        }
    }
}
