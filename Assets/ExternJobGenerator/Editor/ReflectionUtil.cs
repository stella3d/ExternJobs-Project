using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;

namespace Stella3d.ExternJobGenerator
{
    public enum ReadWriteOptions : byte
    {
        ReadAndWrite = 0,
        ReadOnly = 1,
        WriteOnly = 2
    }
    
    public enum AccessModifier : byte
    {
        Public,
        Private,
        Protected,
        Internal
    }

    public static class ExtensionMethods
    {
        public static string Serialize(this IFieldDeclaration field)
        {
            var attributes = field.Attributes != null ? $"{field.Attributes} " : string.Empty;
            var access = field.Access.GetString();
            var readOnlyMod = field.IsReadOnly ? " readonly " : " "; 
            
            return $"{attributes}{access}{readOnlyMod}{field.TypeString} {field.Name};"; 
        }
        
        public static string GetDeclaration(this ReadWriteOptions options)
        {
            string attributes = null;
            switch (options)
            {
                case ReadWriteOptions.ReadOnly:
                    attributes = "[ReadOnly]";
                    break;
                case ReadWriteOptions.WriteOnly:
                    attributes = "[WriteOnly]";
                    break;
            }

            return attributes;
        }
        
        public static string GetString(this AccessModifier options)
        {
            return options.ToString().ToLowerInvariant();
        }
    }

    public class JobMethodData
    {
        public string TypeName;

        public string[] Constraints;
        
        public IFieldDeclaration[] Fields;

        public MethodInfo NativeMethod;
    }
    
    public interface IFieldDeclaration
    {
        string Name { get; }
        
        bool IsReadOnly { get; }
        
        string Attributes { get; }
        
        string TypeString { get; }
        Type Type { get; }

        ReadWriteOptions ReadWriteOptions { get; }
        
        AccessModifier Access { get; }
    }

    class FieldDeclaration : IFieldDeclaration
    {
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        
        public string TypeString { get; set; }
        public Type Type { get; set; }
        
        public string Attributes { get; set; }
        
        public ReadWriteOptions ReadWriteOptions { get; set; }
        public AccessModifier Access { get; set; }
    }
    
    public static unsafe class ReflectionUtil
    {
        public const string ReturnFieldLabel = "Returned";
        public const string ReturnValuePropertyLabel = "ReturnValue";
        
        static readonly HashSet<string> k_PreIncludedNamespaces = new HashSet<string>
        {
            "System", "UnityEngine", "Unity.Jobs", "Unity.Collections"
        };
        
        public static string ParameterToFieldName(string name, bool isReturn = false)
        {
            if (string.IsNullOrEmpty(name))
                return isReturn ? ReturnFieldLabel : name;
            
            // capitalize first letter to turn param names to field names
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        static ReadWriteOptions GetReadWriteOptions(this ParameterInfo parameter)
        {
            if (parameter.GetCustomAttribute<ReadOnlyAttribute>() != null)
                return ReadWriteOptions.ReadOnly;
            
            return parameter.GetCustomAttribute<WriteOnlyAttribute>() != null 
                ? ReadWriteOptions.WriteOnly : ReadWriteOptions.ReadAndWrite;
        }

        public static bool TryGetMethodJobData(MethodInfo method, out JobMethodData jobData)
        {
            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                if (!param.ParameterType.IsJobCompatible())
                {
                    // TODO - check that method is not generic
                    Debug.LogWarning($"param type {param.ParameterType} tested as not job compatible!");
                    jobData = null;
                    return false;
                }
            }

            var hasReturn = method.ReturnsValue();
            var returnsLength = hasReturn ? 1 : 0;
            
            jobData = new JobMethodData
            {
                TypeName = JobWriter.GetJobTypeName(method),
                Fields = new IFieldDeclaration[parameters.Length + returnsLength],
                NativeMethod = method
            };

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                var rwOptions = param.GetReadWriteOptions();
                jobData.Fields[i] = new FieldDeclaration()
                {
                    Access = AccessModifier.Public,
                    ReadWriteOptions = rwOptions,
                    // TODO (later) - make this an option in ui / script
                    IsReadOnly = true,
                    Name = ParameterToFieldName(param.Name),
                    Type = param.ParameterType,
                    TypeString = GetFieldType(param.ParameterType),
                    Attributes = rwOptions.GetDeclaration()
                };
            }

            if (hasReturn)
            {
                var param = method.ReturnParameter;

                jobData.Fields[jobData.Fields.Length - 1] = new FieldDeclaration()
                {
                    Access = AccessModifier.Public,
                    ReadWriteOptions = ReadWriteOptions.WriteOnly,
                    IsReadOnly = false,
                    Name = ParameterToFieldName(param.Name, true),
                    Type = param.ParameterType,
                    TypeString = GetReturnFieldType(param.ParameterType),
                    Attributes = ReadWriteOptions.WriteOnly.GetDeclaration()
                };
            }

            return true;
        }

        static string GetFieldType(Type parameterType)
        {
            if (parameterType.IsPointer)
            {
                var elementType = parameterType.GetElementType();
                var eTypeStr = elementType.IsPrimitive ? JobWriter.GetPrimitiveName(elementType) : elementType.GetTypeName();
                return $"NativeArray<{eTypeStr}>";
            }

            return parameterType.IsPrimitive ? JobWriter.GetPrimitiveName(parameterType) : parameterType.GetTypeName();
        }
        
        static string GetReturnFieldType(Type returnParameterType)
        {
            if (returnParameterType.IsValueType)
            {
                string elementType = returnParameterType.IsPrimitive 
                    ? JobWriter.GetPrimitiveName(returnParameterType) 
                    : returnParameterType.GetTypeName();
                
                return $"NativeArray<{elementType}>";
            }

            return "";
        }

        internal static string GetTypeName(this Type type)
        {
            if (type.IsPrimitive)
                return JobWriter.GetPrimitiveName(type);

            return IsInPreIncludedNamespace(type) ? type.Name : type.IsGenericParameter ? type.Name : type.FullName;;
        }
        
        public static bool ReturnsValue(this MethodInfo method)=> method.ReturnType != typeof(void);

        public static bool IsInPreIncludedNamespace(Type type)
        {
            return k_PreIncludedNamespaces.Contains(type.Namespace);
        }

        public static bool IsJobCompatible(this Type type, bool logWarning = true)
        {
            if (!type.IsPointer)
            {
                if (type.IsClass)
                {
                    if(logWarning)
                        Debug.LogWarning($"Type '{type.FullName}' is a class, but jobs cannot use classes!");
                
                    return false;
                }
            }

            return type.IsPointer || type.IsValueType;
        }
    }
}

