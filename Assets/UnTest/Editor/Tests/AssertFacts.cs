/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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

    [Test]
    private void IsEqual_NullSecondParam_ThrowsException() {

        var objA = new MockObject();

        bool isExceptionThrown = false;
        try {

            Assert.IsEqual(objA, null);

        } catch (Exception) {

            isExceptionThrown = true;

        } finally {

            Assert.IsTrue(isExceptionThrown);
        }
    }

    [Test]
    private void IsEqual_NullSecondParam_ThrowsUnequalException() {

        var objA = new MockObject();

        try {

            Assert.IsEqual(objA, null);

        } catch (Exception e) {

            Assert.IsEqual(e.GetType(), typeof(Exception));

        } 
    }


    [Test]
    private void IsEqual_NullFirstParam_ThrowsException() {
        
        var objB = new MockObject();

        bool isExceptionThrown = false;
        try {

            Assert.IsEqual(null, objB);

        } catch (Exception) {

            isExceptionThrown = true;

        } finally {

            Assert.IsTrue(isExceptionThrown);
        }
    }


    [Test]
    private void IsEqual_NullFirstParam_ThrowsUnequalException() {
        
        var objB = new MockObject();

        try {

            Assert.IsEqual(null, objB);

        } catch (Exception e) {

            Assert.IsTrue(e.GetType() == typeof(Exception));

        } 
    }

           
           
}

}