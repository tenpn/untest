##UnTest Unity3D Testing Framework

Because there is always one less unit-testing framework than there needs to be.

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
        Assert.Equal(m_instance.Result, 3); // there's no assert class yet!
    }

}
```
