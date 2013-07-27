##UnTest Unity3D Testing Framework

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

####To Run

From the command line: 

    /path/to/unity -projectPath "path/to/project" 
        -batchmode -logFile 
        -executeMethod UnTest.TestRunner.RunTestsFromConsole 
        -quit
        
Unfortunately only works if the Unity3D editor is closed.

From the editor: _Assets->Tests->Run_

---

UnTest Unity3D Unit Testing Framework
Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

