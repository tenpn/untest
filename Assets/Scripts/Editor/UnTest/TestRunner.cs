using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace UnTest {


// runs all unity tests it can find. run from console like this:
// /path/to/unity -projectPath "path/to/project" -batchmode -logFile -executeMethod TestRunner.RunTestsFromConsole -quit
// also run from Assets/Tests/Run menu.
public class TestRunner {

    public static void RunTestsFromConsole() {
    
        var failures = RunAllTests();

        if (failures.Any() == false) {
            Debug.Log("All tests passed!");
            return;
        }

        var failureOutput = CalculateFailureString(failures);
        
        var consoleOutput = new System.Text.StringBuilder();
        
        foreach(var failureLine in failureOutput) {
            consoleOutput.Append(failureLine.Subject + "\n" + failureLine.Body);
        }

        Debug.LogError(consoleOutput.ToString());
        Debug.LogError(failures.Count().ToString() + " test(s) failed\n");
        
        EditorApplication.Exit(-1);
    }

    [MenuItem("Assets/Tests/Run")]
    public static void RunTestsFromEditor() {
        
        var failures = RunAllTests();

        if (failures.Any() == false) {
            Debug.Log("All tests passed");

        } else {
            
            // file names have a full path, but that takes up a lot of log window.
            // unity can still function if we take the project as root.
            string assetsRoot = Application.dataPath; // /path/to/proj/assets
            string projectRoot = assetsRoot.Substring(0, 
                                                      assetsRoot.Length - "/Assets".Length);
            int rootToTrim = projectRoot.Length;            
            Func<string,string> tryTrimProjectRoot = msg => { 

                if (msg.StartsWith(projectRoot)) {
                    return msg.Substring(rootToTrim);
                    
                } else {
                    return msg;
                }
            };

            var failureOutput = CalculateFailureString(failures);
            foreach(var failureMessage in failureOutput) {

                var sanitisedSubject = tryTrimProjectRoot(failureMessage.Subject);

                Debug.LogError(sanitisedSubject + "\n" + failureMessage.Body);
            }
        }
    }
    
    //////////////////////////////////////////////////

    private static string[] s_testableAssemblies = new string [] { 
        "Assembly-UnityScript-Editor-firstpass",
        "Assembly-UnityScript-firstpass",
        "Assembly-CSharp-Editor",
        "Assembly-CSharp",
    };

    private struct TestFailure {
        public Exception FailureException;
        public string SuiteName;
        public string TestName;
        public string FileName;
        public int LineNumber;
        public int ColumnNumber;
    }

    private struct TestFailureMessage {
        public string Subject;
        public string Body;
    }

    //////////////////////////////////////////////////

    // returns list of test failures
    private static IEnumerable<TestFailure> RunAllTests() {
        
        var testableAssemblies = FindAllTestableAssemblies();

        var failures = new List<TestFailure>();
        
        foreach(var testSuite in FindAllTestSuites(testableAssemblies)) {
            
            RunTestsInSuite(testSuite, failures);
        }

        return failures;
    }


    private static IEnumerable<Assembly> FindAllTestableAssemblies() {
        
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => s_testableAssemblies.Any(
                       testableAssembly => assembly.FullName.Contains(testableAssembly)));
    }

    private static IEnumerable<Type> FindAllTestSuites(
        IEnumerable<Assembly> testableAssemblies) {

        return testableAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttributes(typeof(TestSuite), true).Length > 0);
    }

    // returns number of found tests.
    private static void RunTestsInSuite(Type testSuite, List<TestFailure> failureList) {
        
        var setupMethods = testSuite.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(SuiteSetup), false).Length > 0)
            .ToArray();

        var testMethods = testSuite.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(Test), false).Length > 0)
            .ToArray();

        RunTestsInSuite(testSuite, failureList, setupMethods, testMethods);
    }

    private static void RunTestsInSuite(Type testSuite, List<TestFailure> failureList, 
                                        IEnumerable<MethodBase> setupMethods,
                                        IEnumerable<MethodBase> testMethods) {
        
        var instance = Activator.CreateInstance(testSuite);

        foreach(var test in testMethods) {

            foreach(var setupMethod in setupMethods) {
                setupMethod.Invoke(instance, null);
            }

            try {
                test.Invoke(instance, null);

            } catch (Exception e) {

                var stackTrace = new StackTrace(e.InnerException, true);
                var callStack = stackTrace.GetFrames();

                var testFrame = GetTestFrameFromCallStack(callStack, test);

                failureList.Add(new TestFailure  {
                    FailureException = e.InnerException,
                    SuiteName = testSuite.Name,
                    TestName = test.Name,
                    FileName = testFrame.GetFileName(),
                    LineNumber = testFrame.GetFileLineNumber(),
                    ColumnNumber = testFrame.GetFileColumnNumber(),
                });
            }
            
        }
    }

    private static StackFrame GetTestFrameFromCallStack(
        IEnumerable<StackFrame> callStack, MethodBase testFunction) {

        return callStack.Reverse()
            .FirstOrDefault(frame => frame.GetMethod() == testFunction);
    }

    private static IEnumerable<TestFailureMessage> CalculateFailureString(
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
}

}