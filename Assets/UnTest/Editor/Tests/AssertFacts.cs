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

        Assert.ThatThrowsException(() => { Assert.IsTrue(false); },
                                   typeof(Exception));
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

        Assert.ThatThrowsException(() => { Assert.IsEqual(1,2); },
                                   typeof(Exception));
    }

    [Test]
    private void IsEqual_RefObjects_ThrowsException() {

        var objA = new MockObject();
        var objB = new MockObject();

        Assert.ThatThrowsException(() => { Assert.IsEqual(objA, objB); },
                                   typeof(Exception));
    }

    [Test]
    private void IsEqual_NullSecondParam_ThrowsException() {

        var objA = new MockObject();

        Assert.ThatThrowsException(() => { Assert.IsEqual(objA, null); },
                                   typeof(Exception));
    }

    [Test]
    private void IsEqual_NullFirstParam_ThrowsException() {
        
        var objB = new MockObject();
        Assert.ThatThrowsException(() => { Assert.IsEqual(null, objB); },
                                   typeof(Exception));
    }

    [Test]
    private void ThatThrowsException_ThowingLambda_DoesNothing() {
        
        Assert.ThatThrowsException(() => { throw new Exception(); },
                                   typeof(Exception));
    }

    [Test]
    private void ThatThrowsException_NonThrowingLambda_ThrowsException() {
        
        bool threwException = false;
        try {
            Assert.ThatThrowsException(() => { },
                                       typeof(Exception));
        } catch(Exception) { 
            threwException = true;

        } finally {
            Assert.IsTrue(threwException);
        }
    }

    [Test]
    private void ThatThrowsException_ThrowsWrongException_ThrowsException() {
        
        bool threwException = false;
        try {
            Assert.ThatThrowsException(() => { throw new ArgumentException(); },
                                       typeof(NotImplementedException));
        } catch(Exception) { 
            threwException = true;

        } finally {
            Assert.IsTrue(threwException);
        }
    }

    [Test]
    private void IsEqualSequence_NullParams_DoesNothing() {
        
        Assert.IsEqualSequence<int>(null, null);
    }

    [Test]
    private void IsEqualSequence_IdenticalEmptySequences_DoesNothing() {
        
        Assert.IsEqualSequence(new int[] {  },
                               new int[] {  });
    }

    [Test]
    private void IsEqualSequence_IdenticalSequences_DoesNothing() {
        
        Assert.IsEqualSequence(new int[] { 1, 2, 3 },
                               new int[] { 1, 2, 3 });
    }

    [Test]
    private void IsEqualSequence_NullFirstSequenceEmptySecond_AssertFails() {
        
        Assert.ThatThrowsException(() => { 
                Assert.IsEqualSequence(null, new int[] { });
            }, typeof(Exception));
    }

    [Test]
    private void IsEqualSequence_UnEqualSequences_AssertFails() {
        
         Assert.ThatThrowsException(() => { 
                Assert.IsEqualSequence(new int[] { 1, 2 }, new int[] { 2, 3 });
            }, typeof(Exception));
    }

    [Test]
    private void IsFalse_FalseParam_DoesNothing() {
        
        Assert.IsFalse(false);
    }

    [Test]
    private void IsFalse_TrueParam_AssertFails() {
        
        Assert.ThatThrowsException(() => { 
                Assert.IsFalse(true);
            }, typeof(Exception));
    }
}

}