using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Stella3d.ExternJobGenerator
{
    // all methods in here are fakes for "DLL Import" methods
    public static class JobWriter
    {
        const string k_TemplateFilePath = "Assets/ExternJobGenerator/Editor/Templates/IJobTemplate.txt";
        const string k_FieldIndent = "        ";

        static readonly StringBuilder k_Builder = new StringBuilder();
        static readonly StringBuilder k_FileBuilder = new StringBuilder();

        public static bool WhitespaceLineBetweenFields = true;
        
        public static string Write(JobMethodData data)
        {
            k_Builder.Clear();
            k_FileBuilder.Clear();
            
            var templateText = File.ReadAllText(k_TemplateFilePath);
            k_FileBuilder.Append(templateText);

            WriteNamespace(data.NativeMethod);
            WriteTypeName(data);
            WriteFields(data.Fields);
            WriteMethodInvoke(data);

            var content = k_FileBuilder.ToString();
            return content;
        }

        public static string GetTypeName(MethodInfo method)
        {
            var capitalizedName = char.ToUpperInvariant(method.Name[0]) + method.Name.Substring(1);
            return $"{capitalizedName}Job";
        }
        
        static void WriteTypeName(JobMethodData data)
        {
            const string toReplace = "{JOB_TYPE_NAME}";
            k_FileBuilder.Replace(toReplace, data.TypeName);
        }
 
        static void WriteNamespace(MethodInfo method)
        {
            if (method.DeclaringType?.Namespace == null)
            {
                Debug.LogWarning($"Couldn't find namespace for {method.Name}");
                return;
            }

            const string toReplace = "{NAMESPACE}";
            k_FileBuilder.Replace(toReplace, method.DeclaringType.Namespace);
        }

        static void WriteFields(IFieldDeclaration[] fields)
        {
            k_Builder.Clear();
            var lastWhitespaceBound = fields.Length - 1;
            for (var i = 0; i < fields.Length; i++)
            {
                WriteField(fields[i]);
                
                if (WhitespaceLineBetweenFields && i < lastWhitespaceBound)
                    k_Builder.AppendLine();
            }

            const string toReplace = "{JOB_FIELDS}";
            k_FileBuilder.Replace(toReplace, k_Builder.ToString());
        }
        
        static void WriteField(IFieldDeclaration field)
        {
            k_Builder.AppendLine($"{k_FieldIndent}{field.Serialize()}");
        }
        
        static bool UseArrayExtensions { get; set; } = true;
        
        static void WriteMethodInvoke(JobMethodData data)
        {
            k_Builder.Clear();
            if (data.NativeMethod.DeclaringType == null)
                return;
            
            var methodSymbol = $"{data.NativeMethod.DeclaringType.Name}.{data.NativeMethod.Name}";

            var parameters = data.NativeMethod.GetParameters();
            
            var lastCommaBound = parameters.Length - 1;
            // this relies on fields being ordered in the same way as method args
            for (var i = 0; i < parameters.Length; i++)
            {
                var field = data.Fields[i];
                var parameter = parameters[i];
                var pType = parameter.ParameterType;

                // pointer parameters need to get NativeArray arguments converted
                string argument;
                if (pType.IsPointer && field.TypeString.Contains("NativeArray"))
                {
                    var castPrefix = GetPointerCastPrefix(pType);
                    var ptrSuffix = GetPointerParameterSuffix(field.ReadWriteOptions);
                    
                    argument = $"{castPrefix}{field.Name}.{ptrSuffix}";
                }
                else
                {
                    argument = field.Name;
                }

                k_Builder.Append(argument);

                if (i < lastCommaBound)
                    k_Builder.Append(", ");
            }

            string returnStr = string.Empty;
            if (data.NativeMethod.ReturnType != typeof(void))
                returnStr = $"ReturnValue[0] = ";

            var args = k_Builder.ToString();
            var line = $"{returnStr}{methodSymbol}({args});";
            
            const string toReplace = "{METHOD_INVOKE}";
            k_FileBuilder.Replace(toReplace, line);
        }

        static string GetPointerCastPrefix(Type ptrType)
        {
            // array extension method does the cast internallt
            return UseArrayExtensions ? "" : $"(${ptrType}) ";
        }
        
        static string GetPointerParameterSuffix(ReadWriteOptions readWriteOptions)
        {
            switch (readWriteOptions)
            {
                // array extension methods allow much more to fit on a line , but introduce a dependency, so it's an option
                case ReadWriteOptions.ReadOnly:
                    return UseArrayExtensions ? "ReadPtr()" : "GetUnsafeReadOnlyPtr()";
                default:
                    return UseArrayExtensions ? "Ptr()" : "GetUnsafePtr()";
            }
        }

        public static string GetPrimitiveName(Type type)
        {
            switch (type.Name)
            {
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Single":
                    return "float";
                case "Double":
                    return "double";
                default:
                    return type.Name;
            }
        }
    }
}

