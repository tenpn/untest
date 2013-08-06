##UnTest Unity3D Testing Framework v0.4.0

Because there is always one less unit-testing framework than there needs to be.

###Installation

Download the latest release from here:

https://github.com/tenpn/untest/releases

...And extract into your project.

###Usage

A test suite looks like this:

```C#
using UnTest;
    
// all classes marked with TestSuite get tested 
[TestSuite]
class MyTestSuite {

    private MockFoo m_instance;

    // called before every test        
    [SuiteSetup]
    void DoSetup() {
        m_instance = new MockFoo();
    }

    // any exception is a test failure
    [Test]
    void GetResult_WithValidResult_GivesResult() {
        m_instance.SetResult(3);
        Assert.IsTrue(m_instance.Result == 3); 
    }

}
```

You should put them below an editor folder of your project so they're not shipped in builds.

####To Run

From the command line: 

You have two options on the command line. The Unity3D application can deal with added/removed files, but is quite slow, the output is quite verbose and it won't work if the Unity3D editor is currently open. This is the syntax: 

    /path/to/unity -projectPath "path/to/project" 
        -batchmode -logFile 
        -executeMethod UnTest.UnityTestRunner.RunTestsFromConsole 
        -quit
        
Alternatively you can use the .Net UnTest-Console tool to find and execute tests in your built Unity3D DLLs. This is the command:

    mono --debug /path/to/project/Assets/UnTest/Editor/UnTest-Console/bin/Release/UnTest-Console.exe path/to/DLL/folder
    
The `--debug` flag allows the tool to report on line numbers. Your DLL folder is in path/to/project/Libary/ScriptAssemblies if built with the editor, or path/to/project/Temp/bin/debug if built with monodevelop/mdtool. 

As a convenience, Untest/Editor/RunTests.sh will, when given the path to your project SLN, build the assemblies using mdtool then run the tests with UnTest-Console.
        
The tests can also be run from the editor. To do this, select _Run_ from the _Assets->Tests_ menu.

---

Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
