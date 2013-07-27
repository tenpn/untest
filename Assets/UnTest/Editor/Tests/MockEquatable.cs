/*
UnTest Unity3D Unit Testing Framework Copyright (C) 2013 Andrew Fray

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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