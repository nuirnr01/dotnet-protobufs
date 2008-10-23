﻿// Protocol Buffers - Google's data interchange format
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
using System;
using System.Collections.Generic;
using Google.ProtocolBuffers.Descriptors;
using Google.ProtocolBuffers.TestProtos;
using NUnit.Framework;

namespace Google.ProtocolBuffers {
  [TestFixture]
  public class GeneratedMessageTest {
    ReflectionTester reflectionTester;
    ReflectionTester extensionsReflectionTester;
    
    [SetUp]
    public void SetUp() {
      reflectionTester = ReflectionTester.CreateTestAllTypesInstance();
      extensionsReflectionTester = ReflectionTester.CreateTestAllExtensionsInstance();
    }

    [Test]
    public void RepeatedAddPrimitiveBeforeBuild() {
      TestAllTypes message = new TestAllTypes.Builder { RepeatedInt32List = { 1, 2, 3 } }.Build();
      TestUtil.AssertEqual(new int[]{1, 2, 3}, message.RepeatedInt32List);
    }

    [Test]
    public void AddPrimitiveFailsAfterBuild() {
      TestAllTypes.Builder builder = new TestAllTypes.Builder();
      IList<int> list = builder.RepeatedInt32List;
      list.Add(1); // Fine
      builder.Build();

      try {
        list.Add(2);
        Assert.Fail("List should be frozen");
      } catch (NotSupportedException) {
        // Expected
      }
    }

    [Test]
    public void RepeatedAddMessageBeforeBuild() {
      TestAllTypes message = new TestAllTypes.Builder {
          RepeatedNestedMessageList = { new TestAllTypes.Types.NestedMessage.Builder { Bb = 10 }.Build() } }.Build();
      Assert.AreEqual(1, message.RepeatedNestedMessageCount);
      Assert.AreEqual(10, message.RepeatedNestedMessageList[0].Bb);
    }

    [Test]
    public void AddMessageFailsAfterBuild() {
      TestAllTypes.Builder builder = new TestAllTypes.Builder();
      IList<TestAllTypes.Types.NestedMessage> list = builder.RepeatedNestedMessageList;
      builder.Build();

      try {
        list.Add(new TestAllTypes.Types.NestedMessage.Builder { Bb = 10 }.Build());
        Assert.Fail("List should be frozen");
      } catch (NotSupportedException) {
        // Expected
      }
    }

    [Test]
    public void DefaultInstance() {
      Assert.AreSame(TestAllTypes.DefaultInstance, TestAllTypes.DefaultInstance.DefaultInstanceForType);
      Assert.AreSame(TestAllTypes.DefaultInstance, TestAllTypes.CreateBuilder().DefaultInstanceForType);
    }

    [Test]
    public void Accessors() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();
      TestUtil.SetAllFields(builder);
      TestAllTypes message = builder.Build();
      TestUtil.AssertAllFieldsSet(message);
    }
    
    [Test]
    public void RepeatedSetters() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();
      TestUtil.SetAllFields(builder);
      TestUtil.ModifyRepeatedFields(builder);
      TestAllTypes message = builder.Build();
      TestUtil.AssertRepeatedFieldsModified(message);
    }

    [Test]
    public void RepeatedAppend() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();

      builder.AddRangeRepeatedInt32(new int[]{1, 2, 3, 4});
      builder.AddRangeRepeatedForeignEnum((new ForeignEnum[] { ForeignEnum.FOREIGN_BAZ }));

      ForeignMessage foreignMessage = ForeignMessage.CreateBuilder().SetC(12).Build();
      builder.AddRangeRepeatedForeignMessage(new ForeignMessage[] {foreignMessage});

      TestAllTypes message = builder.Build();
      TestUtil.AssertEqual(message.RepeatedInt32List, new int[]{1, 2, 3, 4});
      TestUtil.AssertEqual(message.RepeatedForeignEnumList, new ForeignEnum[] {ForeignEnum.FOREIGN_BAZ});
      Assert.AreEqual(1, message.RepeatedForeignMessageCount);
      Assert.AreEqual(12, message.GetRepeatedForeignMessage(0).C);
    }

    [Test]
    public void SettingForeignMessageUsingBuilder() {
      TestAllTypes message = TestAllTypes.CreateBuilder()
          // Pass builder for foreign message instance.
          .SetOptionalForeignMessage(ForeignMessage.CreateBuilder().SetC(123))
          .Build();
      TestAllTypes expectedMessage = TestAllTypes.CreateBuilder()
          // Create expected version passing foreign message instance explicitly.
          .SetOptionalForeignMessage(ForeignMessage.CreateBuilder().SetC(123).Build())
          .Build();
      Assert.AreEqual(expectedMessage, message);
    }

    [Test]
    public void SettingRepeatedForeignMessageUsingBuilder() {
      TestAllTypes message = TestAllTypes.CreateBuilder()
          // Pass builder for foreign message instance.
          .AddRepeatedForeignMessage(ForeignMessage.CreateBuilder().SetC(456))
          .Build();
      TestAllTypes expectedMessage = TestAllTypes.CreateBuilder()
          // Create expected version passing foreign message instance explicitly.
          .AddRepeatedForeignMessage(ForeignMessage.CreateBuilder().SetC(456).Build())
          .Build();
      Assert.AreEqual(expectedMessage, message);
    }

    
    [Test]
    public void Defaults() {
      TestUtil.AssertClear(TestAllTypes.DefaultInstance);
      TestUtil.AssertClear(TestAllTypes.CreateBuilder().Build());

      Assert.AreEqual("\u1234", TestExtremeDefaultValues.DefaultInstance.Utf8String);
    }

    [Test]
    public void ReflectionGetters() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();
      TestUtil.SetAllFields(builder);
      TestAllTypes message = builder.Build();
      reflectionTester.AssertAllFieldsSetViaReflection(message);
    }

    [Test]
    public void ReflectionSetters() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();
      reflectionTester.SetAllFieldsViaReflection(builder);
      TestAllTypes message = builder.Build();
      TestUtil.AssertAllFieldsSet(message);
    }

    [Test]
    public void ReflectionRepeatedSetters() {
      TestAllTypes.Builder builder = TestAllTypes.CreateBuilder();
      reflectionTester.SetAllFieldsViaReflection(builder);
      reflectionTester.ModifyRepeatedFieldsViaReflection(builder);
      TestAllTypes message = builder.Build();
      TestUtil.AssertRepeatedFieldsModified(message);
    }

    [Test]
    public void ReflectionDefaults() {
      reflectionTester.AssertClearViaReflection(TestAllTypes.DefaultInstance);
      reflectionTester.AssertClearViaReflection(TestAllTypes.CreateBuilder().Build());
    }
    // =================================================================
    // Extensions.

    [Test]
    public void ExtensionAccessors() {
      TestAllExtensions.Builder builder = TestAllExtensions.CreateBuilder();
      TestUtil.SetAllExtensions(builder);
      TestAllExtensions message = builder.Build();
      TestUtil.AssertAllExtensionsSet(message);
    }

    [Test]
    public void ExtensionRepeatedSetters() {
      TestAllExtensions.Builder builder = TestAllExtensions.CreateBuilder();
      TestUtil.SetAllExtensions(builder);
      TestUtil.ModifyRepeatedExtensions(builder);
      TestAllExtensions message = builder.Build();
      TestUtil.AssertRepeatedExtensionsModified(message);
    }

    [Test]
    public void ExtensionDefaults() {
      TestUtil.AssertExtensionsClear(TestAllExtensions.DefaultInstance);
      TestUtil.AssertExtensionsClear(TestAllExtensions.CreateBuilder().Build());
    }

    [Test]
    public void ExtensionReflectionGetters() {
      TestAllExtensions.Builder builder = TestAllExtensions.CreateBuilder();
      TestUtil.SetAllExtensions(builder);
      TestAllExtensions message = builder.Build();
      extensionsReflectionTester.AssertAllFieldsSetViaReflection(message);
    }

    [Test]
    public void ExtensionReflectionSetters() {
      TestAllExtensions.Builder builder = TestAllExtensions.CreateBuilder();
      extensionsReflectionTester.SetAllFieldsViaReflection(builder);
      TestAllExtensions message = builder.Build();
      TestUtil.AssertAllExtensionsSet(message);
    }

    [Test]
    public void ExtensionReflectionRepeatedSetters() {
      TestAllExtensions.Builder builder = TestAllExtensions.CreateBuilder();
      extensionsReflectionTester.SetAllFieldsViaReflection(builder);
      extensionsReflectionTester.ModifyRepeatedFieldsViaReflection(builder);
      TestAllExtensions message = builder.Build();
      TestUtil.AssertRepeatedExtensionsModified(message);
    }

    [Test]
    public void ExtensionReflectionDefaults() {
      extensionsReflectionTester.AssertClearViaReflection(TestAllExtensions.DefaultInstance);
      extensionsReflectionTester.AssertClearViaReflection(TestAllExtensions.CreateBuilder().Build());
    }    
    
    [Test]
    public void ClearExtension() {
      // ClearExtension() is not actually used in TestUtil, so try it manually.
      Assert.IsFalse(TestAllExtensions.CreateBuilder()
          .SetExtension(UnitTestProtoFile.OptionalInt32Extension, 1)
          .ClearExtension(UnitTestProtoFile.OptionalInt32Extension)
          .HasExtension(UnitTestProtoFile.OptionalInt32Extension));
      Assert.AreEqual(0, TestAllExtensions.CreateBuilder()
          .AddExtension(UnitTestProtoFile.RepeatedInt32Extension, 1)
          .ClearExtension(UnitTestProtoFile.RepeatedInt32Extension)
          .GetExtensionCount(UnitTestProtoFile.RepeatedInt32Extension));
    }

    /* Removed multiple files option for the moment
    [Test]
    public void MultipleFilesOption() {
      // We mostly just want to check that things compile.
      MessageWithNoOuter message = MessageWithNoOuter.CreateBuilder()
          .SetNested(MessageWithNoOuter.Types.NestedMessage.CreateBuilder().SetI(1))
          .AddForeign(TestAllTypes.CreateBuilder().SetOptionalInt32(1))
          .SetNestedEnum(MessageWithNoOuter.Types.NestedEnum.BAZ)
          .SetForeignEnum(EnumWithNoOuter.BAR)
          .Build();
      Assert.AreEqual(message, MessageWithNoOuter.ParseFrom(message.ToByteString()));

      Assert.AreEqual(MultiFileProto.Descriptor, MessageWithNoOuter.Descriptor.File);

      FieldDescriptor field = MessageWithNoOuter.Descriptor.FindDescriptor<FieldDescriptor>("foreign_enum");
      Assert.AreEqual(MultiFileProto.Descriptor.FindTypeByName<EnumDescriptor>("EnumWithNoOuter")
        .FindValueByNumber((int)EnumWithNoOuter.BAR), message[field]);

      Assert.AreEqual(MultiFileProto.Descriptor, ServiceWithNoOuter.Descriptor.File);

      Assert.IsFalse(TestAllExtensions.DefaultInstance.HasExtension(MultiFileProto.ExtensionWithOuter));
    }*/

    [Test]
    public void OptionalFieldWithRequiredSubfieldsOptimizedForSize() {      
      TestOptionalOptimizedForSize message = TestOptionalOptimizedForSize.DefaultInstance;
      Assert.IsTrue(message.IsInitialized);

      message = TestOptionalOptimizedForSize.CreateBuilder().SetO(
          TestRequiredOptimizedForSize.CreateBuilder().BuildPartial()
          ).BuildPartial();
      Assert.IsFalse(message.IsInitialized);

      message = TestOptionalOptimizedForSize.CreateBuilder().SetO(
          TestRequiredOptimizedForSize.CreateBuilder().SetX(5).BuildPartial()
          ).BuildPartial();
      Assert.IsTrue(message.IsInitialized);
    }

    [Test]
    public void TestOptimizedForSizeMergeUsesAllFieldsFromTarget() {
      TestOptimizedForSize withFieldSet = new TestOptimizedForSize.Builder { I = 10 }.Build();
      TestOptimizedForSize.Builder builder = new TestOptimizedForSize.Builder();
      builder.MergeFrom(withFieldSet);
      TestOptimizedForSize built = builder.Build();
      Assert.AreEqual(10, built.I);
    }

    [Test]
    public void UninitializedExtensionInOptimizedForSizeMakesMessageUninitialized() {
      TestOptimizedForSize.Builder builder = new TestOptimizedForSize.Builder();
      builder.SetExtension(TestOptimizedForSize.TestExtension2,
          new TestRequiredOptimizedForSize.Builder().BuildPartial());
      Assert.IsFalse(builder.IsInitialized);
      Assert.IsFalse(builder.BuildPartial().IsInitialized);

      builder = new TestOptimizedForSize.Builder();
      builder.SetExtension(TestOptimizedForSize.TestExtension2,
          new TestRequiredOptimizedForSize.Builder { X = 10 }.BuildPartial());
      Assert.IsTrue(builder.IsInitialized);
      Assert.IsTrue(builder.BuildPartial().IsInitialized);
    }
  }
}