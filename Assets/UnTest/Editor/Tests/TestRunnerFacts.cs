/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System.Collections.Generic;
using System.Reflection;

namespace UnTest.Tests {
    

    [TestSuite]
    class TestRunnerFacts {
        
        [TestSuite]
        class MockBaseTestSuite {
            
            public static int BaseSetupRuns = 0;
            public static int BaseTestRuns = 0;

            [TestSetup] public virtual void Setup() { ++BaseSetupRuns; }

            [Test] public void Test1() { ++BaseTestRuns; }
            [Test] public void Test2() { ++BaseTestRuns; }
            [Test] public void Test3() { ++BaseTestRuns; }
        }

        [TestSuite]
        class MockDerivedTestSuite : MockBaseTestSuite {

            public static int DerivedSetupRuns = 0;
            public static int DerivedTestRuns = 0;

            [TestSetup] public void SetupDerived() { ++DerivedSetupRuns; }

            [Test] public void DerivedTest1() { ++DerivedTestRuns; }
            [Test] public void DerivedTest2() { ++DerivedTestRuns; }
            [Test] public void DerivedTest3() { ++DerivedTestRuns; }

        }

        [TestSetup]
        void Setup() {
            MockBaseTestSuite.BaseSetupRuns = 0;
            MockBaseTestSuite.BaseTestRuns = 0;
            MockDerivedTestSuite.DerivedSetupRuns = 0;
            MockDerivedTestSuite.DerivedTestRuns = 0;
        }

        [Test]
        void RunTestsInSuite_SuiteWithSetupAndTests_RunsSetupBeforeEachTest() {
            
            var failures = new List<TestRunner.TestFailure>();
         
            var setups = new MethodInfo[] { 
                typeof(MockBaseTestSuite).GetMethod("Setup"),
            };

            var tests = new MethodBase[] { 
                typeof(MockBaseTestSuite).GetMethod("Test1"),
                typeof(MockBaseTestSuite).GetMethod("Test2"),
                typeof(MockBaseTestSuite).GetMethod("Test3"),
            };

            var testInstance = new MockBaseTestSuite();

            TestRunner.RunTestsInSuite(testInstance, failures, setups, tests);

            Assert.IsEqual(MockBaseTestSuite.BaseSetupRuns, 3);
        }

        [Test]
        void RunTestsInSuite_SuiteWithDifferentlyNamedBaseSetup_RunsSetupFromBase() {
            
            var failures = new List<TestRunner.TestFailure>();

            TestRunner.RunTestsInSuite(typeof(MockDerivedTestSuite), failures);

            Assert.IsEqual(MockBaseTestSuite.BaseSetupRuns, 3);
        }

        [Test]
        void RunTestsInSuite_SuiteWithDifferentlyNamedBaseSetup_DoesNotRunBaseTests() {
            
            var failures = new List<TestRunner.TestFailure>();

            TestRunner.RunTestsInSuite(typeof(MockDerivedTestSuite), failures);

            Assert.IsEqual(MockBaseTestSuite.BaseTestRuns, 0);
        }


        [TestSuite]
        class MockDerivedTestSuiteWithOverridenSetup : MockBaseTestSuite {

            public static int DerivedSetupRuns = 0;
            public static int DerivedTestRuns = 0;

            [TestSetup] public override void Setup() { ++DerivedSetupRuns; }

            [Test] public void DerivedTest1() { ++DerivedTestRuns; }
            [Test] public void DerivedTest2() { ++DerivedTestRuns; }
            [Test] public void DerivedTest3() { ++DerivedTestRuns; }

        }

        [Test]
        void RunTestsInSuite_SuiteWithOverridenSetup_DoesNotRunBaseSetup() {
            
            var failures = new List<TestRunner.TestFailure>();

            TestRunner.RunTestsInSuite(typeof(MockDerivedTestSuiteWithOverridenSetup), 
                                       failures);

            Assert.IsEqual(MockBaseTestSuite.BaseSetupRuns, 0);
        }

    }

}