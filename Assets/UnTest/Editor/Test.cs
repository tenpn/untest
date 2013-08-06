/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;

namespace UnTest {

// any function in a TestSuite with this attribute will be run before every test
[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute {
    
}

}
