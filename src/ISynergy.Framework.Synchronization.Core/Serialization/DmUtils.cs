using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Serialization
{
    internal static class DmUtils
    {

        public static List<MemberInfo> GetMembersOrderedByParametersForConstructor(List<MemberInfo> members, ParameterInfo[] parameters)
        {
            var membersParams = members.Where(mi => parameters.Any(pi => pi.Name == mi.Name)).OrderBy(m => m.Name + m.DeclaringType.Name);
            var notMembersParams = members.Where(mi => !parameters.Any(pi => pi.Name == mi.Name)).OrderBy(m => m.Name + m.DeclaringType.Name);

            return membersParams.Union(notMembersParams).ToList();
        }

        /// <summary>
        /// Get the Fields and properties from a current type
        /// </summary>
        public static List<MemberInfo> GetMembers(this Type type,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            var targetMembers = new List<MemberInfo>();

            var s = type.GetBaseType().GetFields(flags);

            targetMembers.AddRange(type.GetFields(flags));

            // Dont add Properties since they are methods
            //targetMembers.AddRange(type.GetProperties(flags));

            var distinctMembers = new List<MemberInfo>(targetMembers.Count);

            foreach (var groupedMember in targetMembers.GroupBy(m => m.Name))
            {
                var count = groupedMember.Count();
                var member = groupedMember.FirstOrDefault();

                if (member == null)
                    continue;

                distinctMembers.Add(member);
            }

            return distinctMembers.Where(t => !t.IsIndexedProperty()).OrderBy(m => m.Name + m.DeclaringType.Name).ToList();

        }

        /// <summary>
        /// Get a member value
        /// </summary>
        public static object GetValue(this MemberInfo member, object obj)
        {

            var fi = member as FieldInfo;
            if (fi != null)
                return fi.GetValue(obj);

            var pi = member as PropertyInfo;
            if (pi != null)
                return pi.GetValue(obj);

            // .net standard 1.6 only
            //if (member.MemberType == MemberTypes.Field)
            //{
            //    FieldInfo fi = (FieldInfo)member;
            //    return fi.GetValue(obj);
            //}
            //else if (member.MemberType == MemberTypes.Property)
            //{
            //    PropertyInfo pi = (PropertyInfo)member;
            //    return pi.GetValue(obj);
            //}

            return null;

        }

        /// <summary>
        /// Set an object value
        /// </summary>
        public static void SetValue(this MemberInfo member, object obj, object value)
        {

            var fi = member as FieldInfo;
            if (fi != null)
                fi.SetValue(obj, value);

            var pi = member as PropertyInfo;
            if (pi != null)
                pi.SetValue(obj, value);

            //if (member.MemberType == MemberTypes.Field)
            //{
            //    FieldInfo fi = (FieldInfo)member;
            //    fi.SetValue(obj, value);
            //}
            //else if (member.MemberType == MemberTypes.Property)
            //{
            //    PropertyInfo pi = (PropertyInfo)member;
            //    pi.SetValue(obj, value);
            //}

        }

        /// <summary>
        /// Look if a property is an indexer
        /// </summary>
        public static bool IsIndexedProperty(this MemberInfo member)
        {
            var propertyInfo = member as PropertyInfo;

            return propertyInfo != null ? propertyInfo.GetIndexParameters().Length > 0 : false;
        }

        /// <summary>
        /// Get the member Type
        /// </summary>
        public static Type GetMemberType(this MemberInfo member)
        {

            //return member.GetType();

            var fi = member as FieldInfo;
            if (fi != null)
                return fi.FieldType;

            var pi = member as PropertyInfo;
            if (pi != null)
                return pi.PropertyType;


            //if (member.MemberType == MemberTypes.Field)
            //{
            //    FieldInfo fi = (FieldInfo)member;
            //    return fi.FieldType;
            //}
            //else if (member.MemberType == MemberTypes.Property)
            //{
            //    PropertyInfo pi = (PropertyInfo)member;
            //    return pi.PropertyType;
            //}

            return null;

        }

        public static bool IsInstantiable(this Type t)
        {
            var ti = t.GetTypeInfo();

            return !(ti.IsInterface || ti.IsAbstract);
        }

        public static bool IsNullable(this Type t)
        {
            if (t.GetTypeInfo().IsValueType)
                return t.IsNullableType();

            return true;
        }

        public static bool IsNullableType(this Type t)
        {
            var ti = t.GetTypeInfo();
            return ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsPrimitive(this Type t)
        {
            var ti = t.GetTypeInfo();

            // If enum, we have to retrieve the underlying type
            if (t.IsEnum())
                return Enum.GetUnderlyingType(t).IsPrimitive();

            // if it's a nullable<T>
            if (t.IsNullableType())
            {
                var nonNullable = Nullable.GetUnderlyingType(t);

                // Nullable<Enum> ?
                if (nonNullable.GetTypeInfo().IsEnum)
                {
                    var nullableUnderlyingType = typeof(Nullable<>).MakeGenericType(Enum.GetUnderlyingType(nonNullable));
                    return nullableUnderlyingType.IsPrimitive();
                }

                return nonNullable.IsPrimitive();
            }

            var isPrimitive = ti.IsPrimitive;

            if (isPrimitive)
                return true;

            // for such types like DateTime whe check the IConvertible interface
            return t.IsIConvertible();
        }

        public static bool IsPrimitiveManagedType(this Type valueType)
        {
            if (valueType == typeof(bool))
                return true;
            else if (valueType == typeof(byte))
                return true;
            else if (valueType == typeof(char))
                return true;
            else if (valueType == typeof(double))
                return true;
            else if (valueType == typeof(float))
                return true;
            else if (valueType == typeof(int))
                return true;
            else if (valueType == typeof(long))
                return true;
            else if (valueType == typeof(short))
                return true;
            else if (valueType == typeof(uint))
                return true;
            else if (valueType == typeof(ulong))
                return true;
            else if (valueType == typeof(ushort))
                return true;
            else if (valueType == typeof(byte[]))
                return true;
            else if (valueType == typeof(DateTime))
                return true;
            else if (valueType == typeof(DateTimeOffset))
                return true;
            else if (valueType == typeof(decimal))
                return true;
            else if (valueType == typeof(Guid))
                return true;
            else if (valueType == typeof(string))
                return true;
            else if (valueType == typeof(sbyte))
                return true;
            else if (valueType == typeof(TimeSpan))
                return true;

            return false;
        }

        public static Type GetTypeFromAssemblyQualifiedName(string valueType)
        {
            if (valueType == "1")
                return typeof(bool);
            else if (valueType == "2")
                return typeof(byte);
            else if (valueType == "3")
                return typeof(char);
            else if (valueType == "4")
                return typeof(double);
            else if (valueType == "5")
                return typeof(float);
            else if (valueType == "6")
                return typeof(int);
            else if (valueType == "7")
                return typeof(long);
            else if (valueType == "8")
                return typeof(short);
            else if (valueType == "9")
                return typeof(uint);
            else if (valueType == "10")
                return typeof(ulong);
            else if (valueType == "11")
                return typeof(ushort);
            else if (valueType == "12")
                return typeof(byte[]);
            else if (valueType == "13")
                return typeof(DateTime);
            else if (valueType == "14")
                return typeof(DateTimeOffset);
            else if (valueType == "15")
                return typeof(decimal);
            else if (valueType == "16")
                return typeof(Guid);
            else if (valueType == "17")
                return typeof(string);
            else if (valueType == "18")
                return typeof(sbyte);
            else if (valueType == "19")
                return typeof(TimeSpan);

            return Type.GetType(valueType, true, true);

        }

        public static string GetAssemblyQualifiedName(this Type valueType)
        {
            if (valueType == typeof(bool))
                return "1";
            else if (valueType == typeof(byte))
                return "2";
            else if (valueType == typeof(char))
                return "3";
            else if (valueType == typeof(double))
                return "4";
            else if (valueType == typeof(float))
                return "5";
            else if (valueType == typeof(int))
                return "6";
            else if (valueType == typeof(long))
                return "7";
            else if (valueType == typeof(short))
                return "8";
            else if (valueType == typeof(uint))
                return "9";
            else if (valueType == typeof(ulong))
                return "10";
            else if (valueType == typeof(ushort))
                return "11";
            else if (valueType == typeof(byte[]))
                return "12";
            else if (valueType == typeof(DateTime))
                return "13";
            else if (valueType == typeof(DateTimeOffset))
                return "14";
            else if (valueType == typeof(decimal))
                return "15";
            else if (valueType == typeof(Guid))
                return "16";
            else if (valueType == typeof(string))
                return "17";
            else if (valueType == typeof(sbyte))
                return "18";
            else if (valueType == typeof(TimeSpan))
                return "19";

            return valueType.AssemblyQualifiedName;

        }


        public static Type GetBaseType(this Type t)
        {
            var objectTypeInfo = t.GetTypeInfo();

            if (t.IsEnum())
                return Enum.GetUnderlyingType(t).GetBaseType();

            if (!objectTypeInfo.IsGenericType)
                return t;

            if (t.IsDictionary())
                return t;

            if (t.IsEnumerable())
                return t;

            var gga = objectTypeInfo.GenericTypeArguments;  // GetGenericArguments();
            if (gga == null || gga.Length <= 0)
                throw new ArgumentException("type not valid");

            return gga[0];
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type.IsArray)
                return true;

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return true;

            return false;
        }

        public static bool IsEnum(this Type type)
        {

            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsISerializable(this Type t)
        {
            if (typeof(ISerializable).IsAssignableFrom(t))
                return true;

            var typeInfo = t.GetTypeInfo();

            if (typeInfo.ImplementedInterfaces == null || typeInfo.ImplementedInterfaces.Count() == 0)
                return false;

            foreach (var interf in typeInfo.ImplementedInterfaces)
                if (interf == typeof(ISerializable))
                    return true;

            return false;
        }

        public static bool IsIConvertible(this Type t)
        {
            var isAssignable = typeof(IConvertible).IsAssignableFrom(t);

            if (isAssignable)
                return true;

            if (t.IsNullableType())
            {
                var ut = Nullable.GetUnderlyingType(t);
                isAssignable = typeof(IConvertible).IsAssignableFrom(ut);

                return isAssignable;
            }

            return false;
        }

        public static bool IsDictionary(this Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
                return true;

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return true;

            return false;
        }

        public static TypeConverter GetConverter(this Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);

            // Every object could use a TypeConverter, so we exclude it
            if (converter != null && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
                return converter;

            return null;
        }

        public static object CreateInstance(this Type type, object[] parameters)
        {
            if (type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);

            var constructorInfo = type.GetParameterizedConstructor(true);

            if (constructorInfo == null)
                return null;

            return constructorInfo.Invoke(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(this Type type)
        {
            if (type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);

            object instance;

            var constructorInfo = type.GetDefaultConstructor(true);

            if (constructorInfo == null)
            {
                // try to create an uninitialized object when we have a parameters constructor
                // since we don't know what kind of parameters values we need to pass, we can try this method
                // Exception : Doesn't work if the base class is abstract
                var rh = typeof(RuntimeHelpers);
                var mi = rh.GetRuntimeMethod("GetUninitializedObject", new Type[] { typeof(Type) });
                instance = mi.Invoke(null, new object[] { type });
            }
            else
                instance = constructorInfo.Invoke(null);

            return instance;
        }

        public static ParameterInfo[] GetConstructorParameters(this Type type, bool nonPublic)
        {

            var ci = type.GetDefaultConstructor(nonPublic);

            if (ci != null)
                return null;


            ci = type.GetParameterizedConstructor(nonPublic);

            if (ci == null)
                return null;

            return ci.GetParameters();

        }


        public static ConstructorInfo GetDefaultConstructor(this Type t, bool nonPublic)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            if (nonPublic)
                bindingFlags = bindingFlags | BindingFlags.NonPublic;

            var gtcs = t.GetConstructors(bindingFlags);

            return gtcs.SingleOrDefault(c => !c.GetParameters().Any());
        }

        public static ConstructorInfo GetParameterizedConstructor(this Type objectType, bool nonPublic)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            if (nonPublic)
                bindingFlags = bindingFlags | BindingFlags.NonPublic;

            IList<ConstructorInfo> constructors = objectType.GetConstructors(bindingFlags).ToList();

            return constructors.Count == 1 ? constructors[0] : null;
        }

        public static ConstructorInfo GetISerializableConstructor(this Type type)
        {
            return type.GetConstructor(new[] { typeof(SerializationInfo), typeof(StreamingContext) });
        }


        public static void WriteLineIndent(this TextWriter writer, int indent)
        {
            writer.WriteLine();
            writer.WriteIndent(indent);

        }
        public static void WriteIndent(this TextWriter writer, int indent)
        {
            for (var i = 0; i < indent; i++)
                writer.Write("  ");

        }

        public static void WriteBytes(this TextWriter writer, byte[] bytes)
        {
            Console.Write("[");
            foreach (var b in bytes)
                Console.Write(b);
            Console.Write("]");
        }

        public static void WriteBytes(this TextWriter writer, byte b)
        {
            Console.Write("[");
            Console.Write(b);
            Console.Write("]");
        }

    }
}
