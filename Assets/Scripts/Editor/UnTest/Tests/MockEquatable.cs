using System;

namespace UnTest.Tests {
    
    class MockEquatable<T> : IEquatable<MockEquatable<T>> where T : IEquatable<T> {
        
        public MockEquatable(T value) {
            Value = value;
        }

        public T Value;

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public override bool Equals(Object obj) {
            if (obj is MockEquatable<T>) {
                return Equals((MockEquatable<T>)obj);
            } else {
                return base.Equals(obj);
            }
        }

        public bool Equals(MockEquatable<T> other) {
            return Value.Equals(other.Value);
        }
    }
}