/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
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