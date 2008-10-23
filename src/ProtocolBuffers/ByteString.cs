﻿using System;
using System.Collections;
using System.Collections.Generic;
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.
// http://code.google.com/p/protobuf/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Text;

namespace Google.ProtocolBuffers {
  /// <summary>
  /// Immutable array of bytes.
  /// TODO(jonskeet): Implement the common collection interfaces?
  /// </summary>
  public sealed class ByteString : IEnumerable<byte>, IEquatable<ByteString> {

    private static readonly ByteString empty = new ByteString(new byte[0]);

    private readonly byte[] bytes;

    /// <summary>
    /// Constructs a new ByteString from the given byte array. The array is
    /// *not* copied, and must not be modified after this constructor is called.
    /// </summary>
    private ByteString(byte[] bytes) {
      this.bytes = bytes;
    }

    /// <summary>
    /// Returns an empty ByteString.
    /// </summary>
    public static ByteString Empty {
      get { return empty; }
    }

    /// <summary>
    /// Returns the length of this ByteString in bytes.
    /// </summary>
    public int Length {
      get { return bytes.Length; }
    }

    public bool IsEmpty {
      get { return Length == 0; }
    }

    public byte[] ToByteArray() {
      return (byte[])bytes.Clone();
    }

    /// <summary>
    /// Constructs a ByteString from the given array. The contents
    /// are copied, so further modifications to the array will not
    /// be reflected in the returned ByteString.
    /// </summary>
    public static ByteString CopyFrom(byte[] bytes) {
      return new ByteString((byte[]) bytes.Clone());
    }

    /// <summary>
    /// Constructs a ByteString from a portion of a byte array.
    /// </summary>
    public static ByteString CopyFrom(byte[] bytes, int offset, int count) {
      byte[] portion = new byte[count];
      Array.Copy(bytes, offset, portion, 0, count);
      return new ByteString(portion);
    }

    /// <summary>
    /// Creates a new ByteString by encoding the specified text with
    /// the given encoding.
    /// </summary>
    public static ByteString CopyFrom(string text, Encoding encoding) {
      return new ByteString(encoding.GetBytes(text));
    }

    /// <summary>
    /// Creates a new ByteString by encoding the specified text in UTF-8.
    /// </summary>
    public static ByteString CopyFromUtf8(string text) {
      return CopyFrom(text, Encoding.UTF8);
    }
    
    /// <summary>
    /// Retuns the byte at the given index.
    /// </summary>
    public byte this[int index] {
      get { return bytes[index]; }
    }

    public string ToString(Encoding encoding) {
      return encoding.GetString(bytes);
    }

    public string ToStringUtf8() {
      return ToString(Encoding.UTF8);
    }

    public IEnumerator<byte> GetEnumerator() {
      return ((IEnumerable<byte>) bytes).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    /// Creates a CodedInputStream from this ByteString's data.
    /// </summary>
    public CodedInputStream CreateCodedInput() {
      
      // We trust CodedInputStream not to reveal the provided byte array or modify it
      return CodedInputStream.CreateInstance(bytes);
    }

    // TODO(jonskeet): CopyTo if it turns out to be required

    public override bool Equals(object obj) {
      ByteString other = obj as ByteString;
      if (obj == null) {
        return false;
      }
      return Equals(other);
    }

    public override int GetHashCode() {
      int ret = 23;
      foreach (byte b in bytes) {
        ret = (ret << 8) | b;
      }
      return ret;
    }

    public bool Equals(ByteString other) {
      if (other.bytes.Length != bytes.Length) {
        return false;
      }
      for (int i = 0; i < bytes.Length; i++) {
        if (other.bytes[i] != bytes[i]) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Builder for ByteStrings which allows them to be created without extra
    /// copying being involved. This has to be a nested type in order to have access
    /// to the private ByteString constructor.
    /// </summary>
    internal sealed class CodedBuilder {
      private readonly CodedOutputStream output;
      private readonly byte[] buffer;

      internal CodedBuilder(int size) {
        buffer = new byte[size];
        output = CodedOutputStream.CreateInstance(buffer);
      }

      public ByteString Build() {
        output.CheckNoSpaceLeft();

        // We can be confident that the CodedOutputStream will not modify the
        // underlying bytes anymore because it already wrote all of them.  So,
        // no need to make a copy.
        return new ByteString(buffer);
      }

      public CodedOutputStream CodedOutput {
        get {
          return output;
        }
      }
    }
  }
}