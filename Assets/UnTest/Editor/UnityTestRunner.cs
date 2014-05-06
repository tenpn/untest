/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnTest {
    
// runs all unity tests it can find. run from console like this:
// /path/to/unity -projectPath "path/to/project" -batchmode -logFile -executeMethod TestRunner.RunTestsFromConsole -quit
// also run from Assets/Tests/Run menu.
public class UnityTestRunner {
    
    public static void RunTestsFromConsole() {
    
        var results = TestRunner.RunAllTests();

        if (TestRunner.OutputTestResults(results, Debug.LogError) == false) {
            EditorApplication.Exit(-1);
        }
    }

    [MenuItem("Assets/Tests/Run Selected")]
    public static void RunSelectedTestFromEditor() {
        var typesToTest = new List<Type>();
        if (null != Selection.objects) {
            foreach (var obj in Selection.objects) {
                var script = obj as MonoScript;
                if (null != script) {
                    var type = script.GetClass();
                    if (null == type) {
                        Debug.LogWarning("Can't GetClass() from MonoScript with mismatched file and fully qualified class names", script);
                        continue;
                    }
                    typesToTest.Add(type);
                }
            }
        }

        var results = TestRunner.RunAllTestsInTypes(typesToTest);

        ProcessTestResults(results);
    }

    [MenuItem("Assets/Tests/Run All")]
    public static void RunAllTestsFromEditor() {
        var results = TestRunner.RunAllTests();

        ProcessTestResults(results);
    }


    private static void ProcessTestResults(TestRunner.ExecutionResults results)
    {
        Debug.Log("Executed " + results.TotalTestsRun + " tests");

        if (results.Failures.Any() == false) {
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

            var failureOutput = TestRunner.CalculateFailureString(results.Failures);
            foreach(var failureMessage in failureOutput) {

                var sanitisedSubject = tryTrimProjectRoot(failureMessage.Subject);

                Debug.LogError(sanitisedSubject + "\n" + failureMessage.Body);
            }
        }
    }


}

}