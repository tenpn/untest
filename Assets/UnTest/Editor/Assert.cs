/*
Copyright(c) 2013 Andrew Fray
Licensed under the MIT license. See the license.txt file for full details.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnTest {
    
public static class Assert {
    

    public static void IsTrue(bool condition) {
        if (condition == false) {
            throw new Exception("Assert failed");
        }
    }

    public static void IsFalse(bool condition) {
        if (condition) {
            throw new Exception("Assert.IsFalse failed");
        }
    }

    public static void IsEqual(object lhs, object rhs) {

        if (lhs == null) {
            if (rhs != null) {
                throw new Exception("Fail: NULL != " + rhs);
            }

        } else if (lhs.Equals(rhs) == false) {
            throw new Exception("Fail: " + lhs + " != " + rhs);
        }
    }

    public static void ThatThrowsException(Action lambda, Type exceptionType) {

        bool isThrown = false;

        try {
            lambda();

        } catch(Exception e) {
            isThrown = e.GetType() == exceptionType;

        } finally {
            if (isThrown == false) {
                throw new Exception("Did not get exception of type " 
                                    + exceptionType.ToString());
            }
        }
    }

    public static void IsEqualSequence<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) {
        
        if (lhs == null) {
            if (rhs == null) {
                return;
            }
        } else if (lhs.SequenceEqual(rhs)) {
            return;
        }

        var lhsEnumerator = lhs == null ? null : lhs.GetEnumerator();
        var rhsEnumerator = rhs == null ? null : rhs.GetEnumerator();

        var comp = new System.Text.StringBuilder();

        comp.Append("Sequences not equal\n");
        comp.AppendFormat("{0} elements in left-hand side, {1} elements in right\n",
                          (lhs == null ? -1 : lhs.Count()), 
                          (rhs == null ? -1 : rhs.Count()));
        comp.Append("LHS | RHS\n");

        do
        {
            if (lhsEnumerator != null && lhsEnumerator.MoveNext() == false) {
                lhsEnumerator = null;
            }
            if (rhsEnumerator != null && rhsEnumerator.MoveNext() == false) {
                rhsEnumerator = null;
            }

            var lhsValue = lhsEnumerator == null ? " " : lhsEnumerator.Current.ToString();
            var rhsValue = rhsEnumerator == null ? " " : rhsEnumerator.Current.ToString();
            
            comp.AppendFormat("{0} | {1}\n", lhsValue, rhsValue);
            
        } while(lhsEnumerator != null || rhsEnumerator != null);
        

        throw new Exception(comp.ToString());

    }

    public static void IsEmptySequence<T>(IEnumerable<T> seq) {
        
        if (seq != null && seq.Any() == false) {
            return;
        }

        var comp = new System.Text.StringBuilder();

        comp.AppendFormat("Assert.IsEmptySequence<{0}>() failed\n",
                          typeof(T));

        if (seq == null) {
            comp.Append("(null sequence)\n");

        } else {
            int index = 0;
            foreach(var val in seq) {
                comp.AppendFormat("{0}: {1}", index, val);
                ++index;
            }
        }

        throw new Exception("sequence was not empty:\n");
    }
}

}