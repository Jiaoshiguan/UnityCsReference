// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements.Collections;

namespace UnityEngine.UIElements.InputSystem
{
    // 它提供了类似UGUI技术栈中，重写Standalone Input Module所能获得自定义输入源的能力。
    // 这一点在之前需要模拟安卓motion时没有发现，使用了不那么合适的方式模拟的安卓焦点输入。
    // 而在最新Unity中，安卓的焦点又变得正常，并不需要模拟了。。之后若有定制输入的需求或World Space模拟输入的需求，可以利用这儿！

    /// <summary>
    /// Handles input and sending events to UIElements Panel through use of Unity's Input System package.
    /// </summary>
    [AddComponentMenu("UI Toolkit/Input System Event System (UI Toolkit)")]
    public class InputSystemEventSystem : MonoBehaviour
    {
        /// <summary>
        /// Returns true if the application has the focus. Events are sent only if this flag is set to true.
        /// </summary>
        public bool isAppFocused { get; private set; } = true;

        /// <summary>
        /// Overrides the default input when NavigationEvents are sent.
        /// </summary>
        /// <remarks>
        /// Use this override to bypass the default input system with your own input system.
        /// This is useful when you want to send fake input to the event system.
        /// This property will be ignored if the New Input System is used.
        /// </remarks>
        [Obsolete("EventSystem no longer supports input override for legacy input. Install Input System package for full input binding functionality.")]
        public InputWrapper inputOverride { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected InputSystemEventSystem()
        {
        }

        void OnApplicationFocus(bool hasFocus)
        {
            isAppFocused = hasFocus;
        }

    }

    internal interface IKeyboardEventProcessor
    {
        void OnEnable();
        void OnDisable();
        void ProcessKeyboardEvents(InputSystemEventSystem eventSystem);
    }
}
