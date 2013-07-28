/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Object = System.Object;
using Debug = UnityEngine.Debug;

namespace UnTest {

public class TestRunner {

    public struct TestFailure {
        public Exception FailureException;
        public string SuiteName;
        public string TestName;
        public string FileName;
        public int LineNumber;
        public int ColumnNumber;
    }

    public struct ExecutionResults {
        public int TotalTestsRun;
        public IEnumerable<TestFailure> Failures;
    }

    public struct TestFailureMessage {
        public string Subject;
        public string Body;
    }

    // returns true if all tests passed
    public static bool OutputTestResults(
        ExecutionResults failures, 
        Action<string> writeLine) {

        string executedTestsMessage = failures.TotalTestsRun.ToString() 
            + " tests executed\n";

        if (failures.Failures.Any() == false) {
            writeLine(executedTestsMessage + "All tests passed!");
            return true;
        }

        var failureOutput = TestRunner.CalculateFailureString(failures.Failures);
        
        var consoleOutput = new System.Text.StringBuilder();
        consoleOutput.Append(executedTestsMessage);
        
        foreach(var failureLine in failureOutput) {
            consoleOutput.Append(failureLine.Subject + "\n" + failureLine.Body);
        }

        writeLine(consoleOutput.ToString());
        writeLine(failures.Failures.Count().ToString() + " test(s) failed\n");

        return false;
    }

    // returns number of found tests.
    public static int RunTestsInSuite(Type testSuite, List<TestFailure> failureList) {
     
        var methodSearch = BindingFlags.NonPublic | BindingFlags.Public
            | BindingFlags.Static | BindingFlags.Instance;
   
        // setups found in whole hierarchy
        var setupMethods = testSuite.GetMethods(methodSearch | BindingFlags.FlattenHierarchy)
                .Where(m => m.GetCustomAttributes(typeof(TestSetup), false).Length > 0);

        var testMethods = testSuite.GetMethods(methodSearch | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttributes(typeof(Test), false).Length > 0)
            .ToArray();

        return RunTestsInSuite(testSuite, failureList, setupMethods, testMethods);
    }

    // returns number of found tests
    public static int RunTestsInSuite(Type testSuite, List<TestFailure> failureList, 
                                      IEnumerable<MethodInfo> setupMethods,
                                      IEnumerable<MethodBase> testMethods) {
        
        var instance = Activator.CreateInstance(testSuite);

        return RunTestsInSuite(instance, failureList, setupMethods, testMethods);
    }

    // returns number of found tests
    public static int RunTestsInSuite(Object suiteInstance, List<TestFailure> failureList, 
                                      IEnumerable<MethodInfo> setupMethods,
                                      IEnumerable<MethodBase> testMethods) {
        
        foreach(var test in testMethods) {

            foreach(var setupMethod in setupMethods) {
                setupMethod.Invoke(suiteInstance, null);
            }

            try {
                test.Invoke(suiteInstance, null);

            } catch (Exception e) {

                var stackTrace = new StackTrace(e.InnerException, true);
                var callStack = stackTrace.GetFrames();

                var testFrame = GetTestFrameFromCallStack(callStack, test);

                failureList.Add(new TestFailure  {
                    FailureException = e.InnerException,
                    SuiteName = suiteInstance.GetType().Name,
                    TestName = test.Name,
                    FileName = testFrame.GetFileName(),
                    LineNumber = testFrame.GetFileLineNumber(),
                    ColumnNumber = testFrame.GetFileColumnNumber(),
                });
            }
            
        }

        return testMethods.Count();

    }
    
    // returns list of test failures
    public static ExecutionResults RunAllTests() {
        
        var testableAssemblies = FindAllTestableAssemblies();

        var failures = new List<TestFailure>();
        
        int totalTestsRun = 0;
        foreach(var testSuite in FindAllTestSuites(testableAssemblies)) {
            
            totalTestsRun += RunTestsInSuite(testSuite, failures);
        }

        return new ExecutionResults  {
            TotalTestsRun = totalTestsRun,
            Failures = failures
        };
    }

    public static ExecutionResults RunAllTestsInAssembly(
        Assembly assemblyToTest) {
        
        var failures = new List<TestFailure>();
        
        int totalTestsRun = 0;
        foreach(var testSuite in FindAllTestSuites(assemblyToTest)) {
            
            totalTestsRun += RunTestsInSuite(testSuite, failures);
        }

        return new ExecutionResults  {
            TotalTestsRun = totalTestsRun,
            Failures = failures
        };
    }

    public static IEnumerable<TestFailureMessage> CalculateFailureString(
        IEnumerable<TestFailure> failures) {

        yield return new TestFailureMessage  {
            Subject = failures.Count().ToString() + " test failure(s)"
        };            

        foreach(var failure in failures) {
            var failureHeadline = string.Format(
                "{0}({1},{2}): error {3}.{4} failed:",
                new System.Object[] { 
                    failure.FileName,
                    failure.LineNumber,
                    failure.ColumnNumber,
                    failure.SuiteName,
                    failure.TestName,
                });
            yield return new TestFailureMessage {
                Subject = failureHeadline,
                Body = failure.FailureException.ToString()
            };
        }
    }

    
    //////////////////////////////////////////////////

    private static string[] s_testableAssemblies = new string [] { 
        "Assembly-UnityScript-Editor-firstpass",
        "Assembly-UnityScript-firstpass",
        "Assembly-CSharp-Editor",
        "Assembly-CSharp",
    };

    //////////////////////////////////////////////////

    private static IEnumerable<Assembly> FindAllTestableAssemblies() {
        
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => s_testableAssemblies.Any(
                       testableAssembly => assembly.FullName.Contains(testableAssembly)));
    }

    private static IEnumerable<Type> FindAllTestSuites(
        IEnumerable<Assembly> testableAssemblies) {

        return testableAssemblies.SelectMany(assembly => FindAllTestSuites(assembly));
    }

    private static IEnumerable<Type> FindAllTestSuites(
        Assembly assemblyToTest) {
    
        return assemblyToTest.GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(TestSuite), true).Length > 0);
    }
    

    private static StackFrame GetTestFrameFromCallStack(
        IEnumerable<StackFrame> callStack, MethodBase testFunction) {

        return callStack.Reverse()
            .FirstOrDefault(frame => frame.GetMethod() == testFunction);
    }

}

}