﻿using System;

using UINT = System.UInt32;

namespace OpenTK.NT.Native
{
    /// <summary>
    /// Used in <see cref="User32.Message.PeekMessage(out Msg, IntPtr, UINT, UINT, PeekMessageActions)"/>
    /// to define what actions to take for retrieved messages.
    /// </summary>
    [Flags]
    public enum PeekMessageActions : UINT
    {
        /// <summary>
        /// Messages are not removed from the queue after processing by
        /// <see cref="User32.Message.PeekMessage(out Msg, IntPtr, UINT, UINT, PeekMessageActions)"/>.
        /// </summary>
        NoRemove = 0,

        /// <summary>
        /// Messages are removed from the queue after processing by
        /// <see cref="User32.Message.PeekMessage(out Msg, IntPtr, UINT, UINT, PeekMessageActions)"/>.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Prevents the system from releasing any thread that is waiting for the caller to go idle
        /// (see WaitForInputIdle). Combine this value with either <see cref="NoRemove"/> or <see cref="Remove"/>.
        /// </summary>
        NoYield = 2,

        /// <summary>
        /// Process mouse and keyboard messages.
        /// </summary>
        QSInput = QueueMessageTypes.Input << 16,

        /// <summary>
        /// Process paint messages.
        /// </summary>
        QSPaint = QueueMessageTypes.Paint << 16,

        /// <summary>
        /// Process all posted messages, including timers and hotkeys.
        /// </summary>
        QSPostMessage = (QueueMessageTypes.PostMessage | QueueMessageTypes.Hotkey | QueueMessageTypes.Timer) << 16,

        /// <summary>
        /// Process all sent messages.
        /// </summary>
        QSSendMessage = QueueMessageTypes.SendMessage << 16
    }
}
