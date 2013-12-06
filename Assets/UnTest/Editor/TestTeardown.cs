/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;

namespace UnTest {

// any function in a TestSuite with this attribute will be run after every test, succeeded or failed
[AttributeUsage(AttributeTargets.Method)]
public class TestTeardown : Attribute {
    
}

    
}
