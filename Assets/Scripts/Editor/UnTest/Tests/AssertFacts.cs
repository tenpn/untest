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

    [Test]
    private void IsEqual_EqualStrings_DoesNothing() {
        Assert.IsEqual("bob", "bob");
    }

    [Test]
    private void IsEqual_EqualInts_DoesNothing() {
        Assert.IsEqual(3, 3);
    }

    [Test]
    private void IsEqual_EqualFloats_DoesNothing() {
        Assert.IsEqual(3f, 3f);
    }

    [Test]
    private void IsEqual_EqualEquatableObjects_DoesNothing() {
        var boxed1 = new MockEquatable<int>(1);
        var boxed2 = new MockEquatable<int>(1);
        
        Assert.IsEqual(boxed1, boxed2);
    }

    [Test]
    private void IsEqual_DifferentValues_ThrowsException() {

        bool isExceptionThrown = false;
        try {

            Assert.IsEqual(1,2);

        } catch (Exception) {

            isExceptionThrown = true;

        } finally {

            Assert.IsTrue(isExceptionThrown);
        }
    }

    [Test]
    private void IsEqual_RefObjects_ThrowsException() {

        var objA = new MockObject();
        var objB = new MockObject();

        bool isExceptionThrown = false;
        try {

            Assert.IsEqual(objA, objB);

        } catch (Exception) {

            isExceptionThrown = true;

        } finally {

            Assert.IsTrue(isExceptionThrown);
        }
    }
}

}