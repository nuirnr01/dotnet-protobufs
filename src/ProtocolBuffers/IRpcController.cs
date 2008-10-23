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

namespace Google.ProtocolBuffers {
  /// <summary>
  /// Mediates a single method call. The primary purpose of the controller
  /// is to provide a way to manipulate settings specific to the
  /// RPC implementation and to find out about RPC-level errors.
  /// 
  /// The methods provided by this interface are intended to be a "least
  /// common denominator" set of features which we expect all implementations to
  /// support. Specific implementations may provide more advanced features,
  /// (e.g. deadline propagation).
  /// </summary>
  public interface IRpcController {

    #region Client side calls
    // These calls may be made from the client side only.  Their results
    // are undefined on the server side (may throw exceptions).

    /// <summary>
    /// Resets the controller to its initial state so that it may be reused in
    /// a new call.  This can be called from the client side only.  It must not
    /// be called while an RPC is in progress.
    /// </summary>
    void Reset();

    /// <summary>
    /// After a call has finished, returns true if the call failed.  The possible
    /// reasons for failure depend on the RPC implementation. Failed must
    /// only be called on the client side, and must not be called before a call has
    /// finished.
    /// </summary>
    bool Failed { get; }

    /// <summary>
    /// If Failed is true, ErrorText returns a human-readable description of the error.
    /// </summary>
    string ErrorText { get; }

    /// <summary>
    /// Advises the RPC system that the caller desires that the RPC call be
    /// canceled. The RPC system may cancel it immediately, may wait awhile and
    /// then cancel it, or may not even cancel the call at all. If the call is
    /// canceled, the "done" callback will still be called and the RpcController
    /// will indicate that the call failed at that time.
    /// </summary>
    void StartCancel();
    #endregion

    #region Server side calls
    // These calls may be made from the server side only.  Their results
    // are undefined on the client side (may throw exceptions).

    /// <summary>
    /// Causes Failed to return true on the client side. <paramref name="reason"/>
    /// will be incorporated into the message returned by ErrorText.
    /// If you find you need to return machine-readable information about
    /// failures, you should incorporate it into your response protocol buffer
    /// and should *not* call SetFailed.
    /// </summary>
    void SetFailed(string reason);

    /// <summary>
    /// If true, indicates that the client canceled the RPC, so the server may as
    /// well give up on replying to it. This method must be called on the server
    /// side only. The server should still call the final "done" callback.
    /// </summary>
    bool isCanceled();

    /// <summary>
    /// Requests that the given callback be called when the RPC is canceled.
    /// The parameter passed to the callback will always be null. The callback will
    /// be called exactly once. If the RPC completes without being canceled, the
    /// callback will be called after completion. If the RPC has already been canceled
    /// when NotifyOnCancel is called, the callback will be called immediately.
    /// 
    /// NotifyOnCancel must be called no more than once per request. It must be
    /// called on the server side only.
    /// </summary>
    /// <param name="callback"></param>
    void NotifyOnCancel(Action<object> callback);
    #endregion
  }
}