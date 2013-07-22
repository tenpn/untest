using System;

namespace UnTest {

// put at top of test suite class to have it run
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TestSuite : Attribute {
    
}

}
