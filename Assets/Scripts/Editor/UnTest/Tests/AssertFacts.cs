using UnityEngine;
using UnTest;
using System;

namespace UnTest.Tests {
    
[TestSuite]
class AssertFacts  {

    [Test]
    private void IsTrue_True_DoesNothing() {

        Assert.IsTrue(true);
    }

    [Test]
    private void IsTrue_False_ThrowsException() {
        
        bool isExceptionThrown = false;
        try {

            Assert.IsTrue(false);

        } catch (Exception) {

            isExceptionThrown = true;

        } finally {

            if (isExceptionThrown == false) {
                throw new Exception("no exception thrown from Assert.IsTrue(false)");
            }
        }
    }

}

}