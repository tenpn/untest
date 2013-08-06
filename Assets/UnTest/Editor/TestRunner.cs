/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using Object = System.Object;

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
   
        // setups: find everything in current type,
        // and private setups in bases. also any non-private setup in bases
        // that aren't virtual.

        var setupMethods = new List<MethodInfo>();
        setupMethods
            .AddRange(testSuite.GetMethods(methodSearch)
                      .Where(m => m.GetCustomAttributes(false).Any(
                                 att => att.GetType().Name == typeof(TestSetup).Name)));

        var chainType = testSuite.BaseType;
        while(chainType != null) {
            var chainSetups = chainType.GetMethods(
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(false).Any(
                           att => att.GetType().Name == typeof(TestSetup).Name))
                .Where(m => m.IsVirtual == false);
            setupMethods.AddRange(chainSetups);
            chainType = chainType.BaseType;
        }

        var testMethods = testSuite.GetMethods(methodSearch | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttributes(false).Any(att => att.GetType().Name == typeof(Test).Name))
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
		return RunAllTestsInAssemblies (testableAssemblies);

    }

    public static IEnumerable<Assembly> FilterAssemblies(IEnumerable<Assembly> assembliesToFilter) {
        return assembliesToFilter.Where(assembly => s_testableAssemblies.Any(
            testableAssembly => assembly.FullName.Contains(testableAssembly)));
    }

    public static ExecutionResults RunAllTestsInAssemblies(
        IEnumerable<Assembly> assembliesToTest) {
        
		
		var failures = new List<TestFailure>();

		int totalTestsRun = 0;
		foreach(var testSuite in FindAllTestSuites(assembliesToTest)) {

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
                Body = failure.FailureException.ToString() + "\n"
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
        
        return FilterAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }

    private static IEnumerable<Type> FindAllTestSuites(
        IEnumerable<Assembly> testableAssemblies) {

        return testableAssemblies.SelectMany(assembly => FindAllTestSuites(assembly));
    }

    private static IEnumerable<Type> FindAllTestSuites(
        Assembly assemblyToTest) {
    
        var allTypes = assemblyToTest.GetTypes();
        foreach(var type in allTypes) {

            try {

                var allAttributes = type.GetCustomAttributes(false);
                if(allAttributes
                   .Any(attribute => attribute.GetType().Name == typeof(TestSuite).Name) 
                     == false) {

                    continue;
                }
                
            } catch (MissingMethodException) {
                continue;
            }

            yield return type;
        }
    }
    

    private static StackFrame GetTestFrameFromCallStack(
        IEnumerable<StackFrame> callStack, MethodBase testFunction) {

        return callStack.Reverse()
            .FirstOrDefault(frame => frame.GetMethod() == testFunction);
    }

}

}