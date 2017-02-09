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


using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{

    /// <summary>
    /// Bag of useful extension methods for objects like cloning.
    /// </summary>
    /// <remarks>
    /// See also http://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-an-object-in-net-c-specifically
    /// </remarks>
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof (Object).GetMethod("MemberwiseClone",
                                                                                   BindingFlags.NonPublic |
                                                                                   BindingFlags.Instance);

        public static Object CreateInstance(Type type)
        {
            object instance = FormatterServices.GetUninitializedObject(type);
            return instance;
        }

        /// <summary>
        /// Clones an object using binary serializer, so it is a deep copy and refactoring safe.
        /// Drawback is that it is not as fast as Copy().
        /// </summary>
        /// <typeparam name="T">Type of object to copy. Must be marked as Serializable!</typeparam>
        /// <param name="obj">Object to clone.</param>
        /// <returns>Cloned object.</returns>
        public static T Clone<T>(this T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, obj);
                memoryStream.Position = 0L;
                object copy = serializer.Deserialize(memoryStream);
                return (T) copy;
            }
        }

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof (String))
                return true;

            return (type.IsValueType & type.IsPrimitive);
        }

        /// <summary>
        /// Copies an object using reflection, so it is a deep copy and refactoring safe.
        /// Is 3 times faster than Clone() because the implementation does not use any serializer but reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T Copy<T>(this T original)
        {
            return (T) Copy((Object) original);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }

        public static void PartialCopy(this Object target, Object source)
        {
            foreach (PropertyInfo property in target.GetType().GetProperties())
            {
                // check whether source object has the the property
                PropertyInfo sp = source.GetType().GetProperty(property.Name);
                if (sp != null)
                {
                    // if yes, copy the value to the matching property
                    object value = sp.GetValue(source, null);
                    target.GetType().GetProperty(property.Name).SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// Compares two object of same type, number of methods, number of properties, number of fields and property/field values.
        /// It's like a deep compare using reflection.
        /// </summary>
        /// <remarks>
        /// Be careful as this operation can be very time consuming depending on the object structure.
        /// </remarks>
        public static bool CompareObjects(this object original, object comparedToObject)
        {
            if (original.GetType() != comparedToObject.GetType())
                return false;

            // Compare Number of Private and public Methods
            MethodInfo[] originalMethods = original
                .GetType()
                .GetMethods(BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public);

            MethodInfo[] comparedMethods = comparedToObject
                .GetType()
                .GetMethods(BindingFlags.Instance |
                            BindingFlags.NonPublic |
                            BindingFlags.Public);

            if (comparedMethods.Length != originalMethods.Length)
                return false;

            // Compare Number of Private and public Properties
            PropertyInfo[] originalProperties = original
                .GetType()
                .GetProperties(BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.Public);

            PropertyInfo[] comparedProperties = comparedToObject
                .GetType()
                .GetProperties(BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.Public);


            if (comparedProperties.Length != originalProperties.Length)
                return false;

            // Compare number of public and private fields
            FieldInfo[] originalFields = original
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            FieldInfo[] comparedToFields = comparedToObject
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (comparedToFields.Length != originalFields.Length)
                return false;

            // compare field values
            foreach (FieldInfo fi in originalFields)
            {
                // check to see if the object to contains the field          
                FieldInfo fiComparedObj = comparedToObject.GetType().GetField(fi.Name,
                                                                              BindingFlags.Instance |
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Public);

                if (fiComparedObj == null)
                    return false;

                if (fi.FieldType.IsArray)
                {
                    // compare Array values
                    var originalArray = fi.GetValue(original) as Array;
                    var comparedArray = fiComparedObj.GetValue(comparedToObject) as Array;

                    if (originalArray == null || comparedArray == null)
                        return false;

                    for (int i = 0; i < originalArray.Length; i++)
                    {
                        // recursive compare call for each value in array
                        if (!originalArray.GetValue(i).CompareObjectsOnlyFields(comparedArray.GetValue(i)))
                            return false;
                    }

                    return true;
                }

                // Get the value of the field from the original object        
                Object srcValue = original.GetType().InvokeMember(fi.Name,
                                                                  BindingFlags.GetField | BindingFlags.Instance |
                                                                  BindingFlags.NonPublic | BindingFlags.Public,
                                                                  null,
                                                                  original,
                                                                  null);

                // Get the value of the field
                object comparedObjFieldValue = comparedToObject
                    .GetType()
                    .InvokeMember(fiComparedObj.Name,
                                  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic |
                                  BindingFlags.Public,
                                  null,
                                  comparedToObject,
                                  null);


                // now compare the field values
                if (srcValue == null)
                {
                    if (comparedObjFieldValue != null)
                        return false;

                    return true;
                }

                if (srcValue.GetType() != comparedObjFieldValue.GetType())
                    return false;

                if (!srcValue.ToString().Equals(comparedObjFieldValue.ToString()))
                    return false;

                if (srcValue.GetType().Namespace == "System")
                {
                    if (srcValue.Equals(comparedObjFieldValue))
                        continue;
                }

                return srcValue.CompareObjects(comparedObjFieldValue);
            }

            // compare each Property values
            foreach (PropertyInfo pi in originalProperties)
            {
                // check to see if the object to contains the field          
                PropertyInfo piComparedObj = comparedToObject
                    .GetType()
                    .GetProperty(pi.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (piComparedObj == null)
                    return false;

                // Get the value of the property from the original object        
                Object srcValue = original
                    .GetType()
                    .InvokeMember(pi.Name,
                                  BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic |
                                  BindingFlags.Public,
                                  null,
                                  original,
                                  null);

                // Get the value of the property
                object comparedObjValue = comparedToObject
                    .GetType()
                    .InvokeMember(piComparedObj.Name,
                                  BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic |
                                  BindingFlags.Public,
                                  null,
                                  comparedToObject,
                                  null);

                // now compare the property values
                if (srcValue.GetType() != comparedObjValue.GetType())
                    return false;

                if (!srcValue.ToString().Equals(comparedObjValue.ToString()))
                    return false;

                if (srcValue.GetType().Namespace == "System")
                {
                    if (srcValue.Equals(comparedObjValue))
                        continue;
                }

                return srcValue.CompareObjects(comparedObjValue);
            }

            return true;
        }

        private const BindingFlags FieldComparisonVisibility = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly ConcurrentDictionary<Type, FieldInfo[]> _compareObjectsOnlyFields = new ConcurrentDictionary<Type, FieldInfo[]>();

        /// <summary>
        /// Similar to CompareObjects but compares only same fields (better performance).
        /// </summary>
        public static bool CompareObjectsOnlyFields(this object originalObject, object comparedObject)
        {
            if (originalObject == null)
                return comparedObject == null;

            if (comparedObject == null)
                return false;

            var originalType = originalObject.GetType();
            var comparedType = comparedObject.GetType();

            // check type
            if (originalType != comparedType)
                return false;

            // Compare number of public and private fields
            // Calculate those only once per type; if done, only get them from the dictionary
            FieldInfo[] originalFields = _compareObjectsOnlyFields.GetOrAdd(originalType, t => t.GetFields(FieldComparisonVisibility));

            // this will never really happen since nobody removes existing keys anymore, but it's here for clarity and ensuring
            // the originalFields null warning disappears
            if (originalFields == null)
                return false;
            
            // compare field values
            foreach (FieldInfo fi in originalFields)
            {
                // get values
                object originalValue = fi.GetValue(originalObject);
                object comparedValue = fi.GetValue(comparedObject);

                // check for null
                if (originalValue == null)
                {
                    return comparedValue == null;
                }

                if (fi.FieldType.IsArray)
                {
                    // compare Array values
                    var originalArray = originalValue as Array;
                    var comparedArray = comparedValue as Array;

                    if (originalArray == null && comparedArray == null)
                        continue;

                    if (originalArray == null || comparedArray == null)
                        return false;

                    for (var i = 0; i < originalArray.Length; i++)
                    {
                        // recursive compare call for each value in array
                        if (!originalArray.GetValue(i).CompareObjectsOnlyFields(comparedArray.GetValue(i)))
                            return false;
                    }

                    continue;
                }

                // if the to string value equals the full type name (complex type) we deep compare.
                if (string.Equals(originalValue.GetType().FullName, originalValue.ToString()))
                {
                    if (!originalValue.CompareObjectsOnlyFields(comparedValue))
                        return false;
                }
                else if (!originalValue.Equals(comparedValue))
                {
                    return false;
                }
            }

            return true;
        }

        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null)
                return null;

            Type typeToReflect = originalObject.GetType();

            if (IsPrimitive(typeToReflect))
                return originalObject;

            if (visited.ContainsKey(originalObject))
                return visited[originalObject];

            object cloneObject = CloneMethod.Invoke(originalObject, null);
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(
            object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
                           BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(
            object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect,
            BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                    continue;

                if (IsPrimitive(fieldInfo.FieldType))
                    continue;

                object originalFieldValue = fieldInfo.GetValue(originalObject);
                object clonedFieldValue = originalFieldValue == null ? null : InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);

                if (clonedFieldValue == null)
                    continue;

                if (fieldInfo.FieldType.IsArray)
                {
                    Type arrayType = fieldInfo.FieldType.GetElementType();
                    if (IsPrimitive(arrayType))
                        continue;

                    var clonedArray = (Array) clonedFieldValue;
                    for (long i = 0; i < clonedArray.LongLength; i++)
                    {
                        clonedArray.SetValue(InternalCopy(clonedArray.GetValue(i), visited), i);
                    }
                }
            }
        }


        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;
            return !enumerable.Any();
        }

        public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return null;
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;

            return obj.GetHashCode();
        }
    }
}
