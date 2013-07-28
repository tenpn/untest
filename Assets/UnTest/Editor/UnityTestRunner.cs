/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
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

        string executedTestsMessage = results.TotalTestsRun.ToString() + " tests executed\n";

        if (results.Failures.Any() == false) {
            Debug.Log(executedTestsMessage + "All tests passed!");
            return;
        }

        var failureOutput = TestRunner.CalculateFailureString(results.Failures);
        
        var consoleOutput = new System.Text.StringBuilder();
        consoleOutput.Append(executedTestsMessage);
        
        foreach(var failureLine in failureOutput) {
            consoleOutput.Append(failureLine.Subject + "\n" + failureLine.Body);
        }

        Debug.LogError(consoleOutput.ToString());
        Debug.LogError(results.Failures.Count().ToString() + " test(s) failed\n");
        
        EditorApplication.Exit(-1);
    }

    [MenuItem("Assets/Tests/Run")]
    public static void RunTestsFromEditor() {
        
        var results = TestRunner.RunAllTests();

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