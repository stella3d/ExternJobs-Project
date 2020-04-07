using System;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Stella3d.ExternJobGenerator.Tests
{
    public class JobWriterTests
    {
        [Test]
        public void ScalePositions()
        {
            LogJobData(typeof(TestMethods), nameof(TestMethods.ScalePositions));
        }
        
        [Test]
        public void SphereCullBatch()
        {
            LogJobData(typeof(TestMethods), nameof(TestMethods.SphereCullBatch));
        }
        
        [Test]
        public void SphereCullBatchFilter()
        {
            LogJobData(typeof(TestMethods), nameof(TestMethods.SphereCullBatchFilter));
        }

        [OneTimeTearDown] 
        public void AfterAll() => AssetDatabase.Refresh();

        static void LogJobData(Type type, string methodName, bool writeFile = true)
        {
            var jobData = GetJobData(type, methodName, out var jobText);
            if(jobData == null) 
                return;
            
            Debug.Log(jobText);

            if (writeFile)
            {
                const string folder = "Assets/ExternJobGenerator/Tests/Editor/Generated/";
                var path = $"{folder}{jobData.TypeName}.cs";
                File.WriteAllText(path, jobText);
            }
        }

        public static JobMethodData GetJobData(Type type, string methodName, out string jobText)
        {
            var method = type.GetMethod(methodName);
            if (method == null || !ReflectionUtil.TryGetMethodJobData(method, out var jobData))
            {
                jobText = null;
                return null;
            }

            jobText = JobWriter.Write(jobData);
            return jobData;
        }
    }
}