using System;

namespace UnTest {
    
public static class Assert {
    

    public static void IsTrue(bool condition) {
        if (condition == false) {
            throw new Exception("Assert failed");
        }
    }

    public static void IsEqual(object lhs, object rhs) {
        IsTrue(lhs.Equals(rhs));
    }
}

}