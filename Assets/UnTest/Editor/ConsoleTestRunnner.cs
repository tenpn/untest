/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;
using System.Reflection;
using System.Linq;
using System.IO;

namespace UnTest {
    
// finds all assemblies in dir from command line, loads, runs tests
public class ConsoleTestRunner {
    
    static void Main(string[] args)
    {
        RunTestsOnAssemblyAtPath(args[0]);
    }

    public static void RunTestsOnAssemblyAtPath(string assemblyDirPath) {
   
		var assemblyDir = new DirectoryInfo (assemblyDirPath);

		var assemblies = assemblyDir.GetFiles("*.dll")
				.Select (assemblyPath => Assembly.LoadFrom (assemblyPath.FullName));
			
        Console.WriteLine("running tests on assembly dir " + assemblyDirPath);

        var results = TestRunner.RunAllTestsInAssemblies(TestRunner.FilterAssemblies(assemblies));

        if(TestRunner.OutputTestResults(results, Console.WriteLine) == false) {
            Environment.Exit(-1);
        }
    }

}
}