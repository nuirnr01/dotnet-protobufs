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

// Author: jonskeet@google.com (Jon Skeet)
//  Following the Java generator by kenton@google.com (Kenton Varda)
//  Based on original Protocol Buffers design by
//  Sanjay Ghemawat, Jeff Dean, and others.

#include <map>
#include <string>

#include <google/protobuf/compiler/csharp/csharp_message_field.h>
#include <google/protobuf/compiler/csharp/csharp_helpers.h>
#include <google/protobuf/io/printer.h>
#include <google/protobuf/wire_format.h>
#include <google/protobuf/stubs/strutil.h>

namespace google {
namespace protobuf {
namespace compiler {
namespace csharp {

namespace {

// TODO(kenton):  Factor out a "SetCommonFieldVariables()" to get rid of
//   repeat code between this and the other field types.
void SetMessageVariables(const FieldDescriptor* descriptor,
                         map<string, string>* variables) {
  (*variables)["name"] =
    UnderscoresToCamelCase(descriptor);
  (*variables)["capitalized_name"] =
    UnderscoresToCapitalizedCamelCase(descriptor);
  (*variables)["number"] = SimpleItoa(descriptor->number());
  (*variables)["type"] = ClassName(descriptor->message_type());
  (*variables)["group_or_message"] =
    (descriptor->type() == FieldDescriptor::TYPE_GROUP) ?
    "Group" : "Message";
}

}  // namespace

// ===================================================================

MessageFieldGenerator::
MessageFieldGenerator(const FieldDescriptor* descriptor)
  : descriptor_(descriptor) {
  SetMessageVariables(descriptor, &variables_);
}

MessageFieldGenerator::~MessageFieldGenerator() {}

void MessageFieldGenerator::
GenerateMembers(io::Printer* printer) const {
  printer->Print(variables_,
    "private boolean has$capitalized_name$;\r\n"
    "private $type$ $name$_ = $type$.getDefaultInstance();\r\n"
    "public boolean has$capitalized_name$() { return has$capitalized_name$; }\r\n"
    "public $type$ get$capitalized_name$() { return $name$_; }\r\n");
}

void MessageFieldGenerator::
GenerateBuilderMembers(io::Printer* printer) const {
  printer->Print(variables_,
    "public boolean has$capitalized_name$() {\r\n"
    "  return result.has$capitalized_name$();\r\n"
    "}\r\n"
    "public $type$ get$capitalized_name$() {\r\n"
    "  return result.get$capitalized_name$();\r\n"
    "}\r\n"
    "public Builder set$capitalized_name$($type$ value) {\r\n"
    "  result.has$capitalized_name$ = true;\r\n"
    "  result.$name$_ = value;\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder set$capitalized_name$($type$.Builder builderForValue) {\r\n"
    "  result.has$capitalized_name$ = true;\r\n"
    "  result.$name$_ = builderForValue.build();\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder merge$capitalized_name$($type$ value) {\r\n"
    "  if (result.has$capitalized_name$() &&\r\n"
    "      result.$name$_ != $type$.getDefaultInstance()) {\r\n"
    "    result.$name$_ =\r\n"
    "      $type$.newBuilder(result.$name$_).mergeFrom(value).buildPartial();\r\n"
    "  } else {\r\n"
    "    result.$name$_ = value;\r\n"
    "  }\r\n"
    "  result.has$capitalized_name$ = true;\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder clear$capitalized_name$() {\r\n"
    "  result.has$capitalized_name$ = false;\r\n"
    "  result.$name$_ = $type$.getDefaultInstance();\r\n"
    "  return this;\r\n"
    "}\r\n");
}

void MessageFieldGenerator::
GenerateMergingCode(io::Printer* printer) const {
  printer->Print(variables_,
    "if (other.has$capitalized_name$()) {\r\n"
    "  merge$capitalized_name$(other.get$capitalized_name$());\r\n"
    "}\r\n");
}

void MessageFieldGenerator::
GenerateBuildingCode(io::Printer* printer) const {
  // Nothing to do for singular fields.
}

void MessageFieldGenerator::
GenerateParsingCode(io::Printer* printer) const {
  printer->Print(variables_,
    "$type$.Builder subBuilder = $type$.newBuilder();\r\n"
    "if (has$capitalized_name$()) {\r\n"
    "  subBuilder.mergeFrom(get$capitalized_name$());\r\n"
    "}\r\n");

  if (descriptor_->type() == FieldDescriptor::TYPE_GROUP) {
    printer->Print(variables_,
      "input.readGroup($number$, subBuilder, extensionRegistry);\r\n");
  } else {
    printer->Print(variables_,
      "input.readMessage(subBuilder, extensionRegistry);\r\n");
  }

  printer->Print(variables_,
    "set$capitalized_name$(subBuilder.buildPartial());\r\n");
}

void MessageFieldGenerator::
GenerateSerializationCode(io::Printer* printer) const {
  printer->Print(variables_,
    "if (has$capitalized_name$()) {\r\n"
    "  output.write$group_or_message$($number$, get$capitalized_name$());\r\n"
    "}\r\n");
}

void MessageFieldGenerator::
GenerateSerializedSizeCode(io::Printer* printer) const {
  printer->Print(variables_,
    "if (has$capitalized_name$()) {\r\n"
    "  size += com.google.protobuf.CodedOutputStream\r\n"
    "    .compute$group_or_message$Size($number$, get$capitalized_name$());\r\n"
    "}\r\n");
}

string MessageFieldGenerator::GetBoxedType() const {
  return ClassName(descriptor_->message_type());
}

// ===================================================================

RepeatedMessageFieldGenerator::
RepeatedMessageFieldGenerator(const FieldDescriptor* descriptor)
  : descriptor_(descriptor) {
  SetMessageVariables(descriptor, &variables_);
}

RepeatedMessageFieldGenerator::~RepeatedMessageFieldGenerator() {}

void RepeatedMessageFieldGenerator::
GenerateMembers(io::Printer* printer) const {
  printer->Print(variables_,
    "private java.util.List<$type$> $name$_ =\r\n"
    "  java.util.Collections.emptyList();\r\n"
    "public java.util.List<$type$> get$capitalized_name$List() {\r\n"
    "  return $name$_;\r\n"   // note:  unmodifiable list
    "}\r\n"
    "public int get$capitalized_name$Count() { return $name$_.size(); }\r\n"
    "public $type$ get$capitalized_name$(int index) {\r\n"
    "  return $name$_.get(index);\r\n"
    "}\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateBuilderMembers(io::Printer* printer) const {
  printer->Print(variables_,
    // Note:  We return an unmodifiable list because otherwise the caller
    //   could hold on to the returned list and modify it after the message
    //   has been built, thus mutating the message which is supposed to be
    //   immutable.
    "public java.util.List<$type$> get$capitalized_name$List() {\r\n"
    "  return java.util.Collections.unmodifiableList(result.$name$_);\r\n"
    "}\r\n"
    "public int get$capitalized_name$Count() {\r\n"
    "  return result.get$capitalized_name$Count();\r\n"
    "}\r\n"
    "public $type$ get$capitalized_name$(int index) {\r\n"
    "  return result.get$capitalized_name$(index);\r\n"
    "}\r\n"
    "public Builder set$capitalized_name$(int index, $type$ value) {\r\n"
    "  result.$name$_.set(index, value);\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder set$capitalized_name$(int index, "
      "$type$.Builder builderForValue) {\r\n"
    "  result.$name$_.set(index, builderForValue.build());\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder add$capitalized_name$($type$ value) {\r\n"
    "  if (result.$name$_.isEmpty()) {\r\n"
    "    result.$name$_ = new java.util.ArrayList<$type$>();\r\n"
    "  }\r\n"
    "  result.$name$_.add(value);\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder add$capitalized_name$($type$.Builder builderForValue) {\r\n"
    "  if (result.$name$_.isEmpty()) {\r\n"
    "    result.$name$_ = new java.util.ArrayList<$type$>();\r\n"
    "  }\r\n"
    "  result.$name$_.add(builderForValue.build());\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder addAll$capitalized_name$(\r\n"
    "    java.lang.Iterable<? extends $type$> values) {\r\n"
    "  if (result.$name$_.isEmpty()) {\r\n"
    "    result.$name$_ = new java.util.ArrayList<$type$>();\r\n"
    "  }\r\n"
    "  super.addAll(values, result.$name$_);\r\n"
    "  return this;\r\n"
    "}\r\n"
    "public Builder clear$capitalized_name$() {\r\n"
    "  result.$name$_ = java.util.Collections.emptyList();\r\n"
    "  return this;\r\n"
    "}\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateMergingCode(io::Printer* printer) const {
  printer->Print(variables_,
    "if (!other.$name$_.isEmpty()) {\r\n"
    "  if (result.$name$_.isEmpty()) {\r\n"
    "    result.$name$_ = new java.util.ArrayList<$type$>();\r\n"
    "  }\r\n"
    "  result.$name$_.addAll(other.$name$_);\r\n"
    "}\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateBuildingCode(io::Printer* printer) const {
  printer->Print(variables_,
    "if (result.$name$_ != java.util.Collections.EMPTY_LIST) {\r\n"
    "  result.$name$_ =\r\n"
    "    java.util.Collections.unmodifiableList(result.$name$_);\r\n"
    "}\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateParsingCode(io::Printer* printer) const {
  printer->Print(variables_,
    "$type$.Builder subBuilder = $type$.newBuilder();\r\n");

  if (descriptor_->type() == FieldDescriptor::TYPE_GROUP) {
    printer->Print(variables_,
      "input.readGroup($number$, subBuilder, extensionRegistry);\r\n");
  } else {
    printer->Print(variables_,
      "input.readMessage(subBuilder, extensionRegistry);\r\n");
  }

  printer->Print(variables_,
    "add$capitalized_name$(subBuilder.buildPartial());\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateSerializationCode(io::Printer* printer) const {
  printer->Print(variables_,
    "for ($type$ element : get$capitalized_name$List()) {\r\n"
    "  output.write$group_or_message$($number$, element);\r\n"
    "}\r\n");
}

void RepeatedMessageFieldGenerator::
GenerateSerializedSizeCode(io::Printer* printer) const {
  printer->Print(variables_,
    "for ($type$ element : get$capitalized_name$List()) {\r\n"
    "  size += com.google.protobuf.CodedOutputStream\r\n"
    "    .compute$group_or_message$Size($number$, element);\r\n"
    "}\r\n");
}

string RepeatedMessageFieldGenerator::GetBoxedType() const {
  return ClassName(descriptor_->message_type());
}

}  // namespace csharp
}  // namespace compiler
}  // namespace protobuf
}  // namespace google