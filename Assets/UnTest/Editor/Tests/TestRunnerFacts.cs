/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
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

        [TestSuite]
        class PrivateBaseSetup {
            
            [TestSuite]
            private class BaseSuiteWithPrivateSetup {
            
                public static int s_privateBaseSetupRunCount = 0;

                [TestSetup]
                void PrivateBaseSetup() {
                    ++s_privateBaseSetupRunCount;
                }
            }

            [TestSuite]
            private class DerivedSuiteWithPrivateSetup : BaseSuiteWithPrivateSetup {
                
                public static int s_privateDerivedSetupRunCount = 0;

                [TestSetup]
                void PrivateDerivedSetup() {
                    ++s_privateDerivedSetupRunCount;
                }

                [Test]
                void SampleTest() {
                }
            }

            List<TestRunner.TestFailure> m_failures;

            [TestSetup]
            void Setup() {
                BaseSuiteWithPrivateSetup.s_privateBaseSetupRunCount = 0;
                DerivedSuiteWithPrivateSetup.s_privateDerivedSetupRunCount = 0;
                m_failures = new List<TestRunner.TestFailure>();
            }

            [Test]
            void RunTestsInSuite_SuiteBaseWithPrivateSetup_RunsDerivedSetup() {
                
                TestRunner.RunTestsInSuite(typeof(DerivedSuiteWithPrivateSetup),
                                           m_failures);

                Assert.IsEqual(DerivedSuiteWithPrivateSetup.s_privateDerivedSetupRunCount,
                               1);
            }

            [Test]
            void RunTestsInSuite_SuiteBaseWithPrivateSetup_RunsBaseSetup() {
                TestRunner.RunTestsInSuite(typeof(DerivedSuiteWithPrivateSetup),
                                           m_failures);

                Assert.IsEqual(BaseSuiteWithPrivateSetup.s_privateBaseSetupRunCount,
                               1);
            }

        }

        [TestSuite]
        class ProtectedVirtualBaseSetup {
            
            [TestSuite]
            private class BaseSuiteWithProtectedVirtualSetup {
            
                public static int s_baseSetupRunCount = 0;

                [TestSetup]
                protected virtual void Setup() {
                    ++s_baseSetupRunCount;
                }
            }

            [TestSuite]
            private class DerivedSuiteWithOverridenSetup : BaseSuiteWithProtectedVirtualSetup {
                
                public static int s_derivedSetupRunCount = 0;

                [TestSetup]
                protected override void Setup() {
                    base.Setup();
                    ++s_derivedSetupRunCount;
                }

                [Test]
                void SampleTest() {
                }
            }

            List<TestRunner.TestFailure> m_failures;

            [TestSetup]
            void Setup() {
                BaseSuiteWithProtectedVirtualSetup.s_baseSetupRunCount = 0;
                DerivedSuiteWithOverridenSetup.s_derivedSetupRunCount = 0;
                m_failures = new List<TestRunner.TestFailure>();
            }

            [Test]
            void RunTestsInSuite_SuiteBaseWithProtectedVirtualSetup_RunsDerivedSetup() {
                
                TestRunner.RunTestsInSuite(typeof(DerivedSuiteWithOverridenSetup),
                                           m_failures);

                Assert.IsEqual(DerivedSuiteWithOverridenSetup.s_derivedSetupRunCount,
                               1);
            }

            [Test]
            void RunTestsInSuite_SuiteBaseWithPrivateSetup_RunsBaseSetup() {
                TestRunner.RunTestsInSuite(typeof(DerivedSuiteWithOverridenSetup),
                                           m_failures);

                Assert.IsEqual(BaseSuiteWithProtectedVirtualSetup.s_baseSetupRunCount,
                               1);
            }

        }

    }

}