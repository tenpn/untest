/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;

namespace UnTest {

// put at top of test suite class to have it run
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TestSuite : Attribute {
    
}

}
