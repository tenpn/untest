using System;

namespace UnTest {

// any function in a TestSuite with this attribute will be run before every test
[AttributeUsage(AttributeTargets.Method)]
public class SuiteSetup : Attribute {
    
}

    
}
