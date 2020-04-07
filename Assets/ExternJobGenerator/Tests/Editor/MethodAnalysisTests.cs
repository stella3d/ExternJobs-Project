using System;
using NUnit.Framework;
using UnityEngine;

namespace Stella3d.ExternJobGenerator.Tests
{
    public class MethodAnalysisTests
    {
        [Test]
        public void ScalePositions()
        {
            LogJobData(typeof(TestMethods), nameof(TestMethods.ScalePositions));
        }

        static void LogJobData(Type type, string methodName)
        {
            var methodInfo = type.GetMethod(methodName);
            
            if (!ReflectionUtil.TryGetMethodJobData(methodInfo, out var jobData))
                return;
            
            foreach (var field in jobData.Fields)
            {
                Debug.Log(field.Serialize());
            }
        }
    }
}