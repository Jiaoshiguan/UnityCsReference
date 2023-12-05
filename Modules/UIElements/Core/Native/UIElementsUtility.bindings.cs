// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
    // This is the required interface to UIElementsUtility for Runtime game components.
    [NativeHeader("Modules/UIElements/Core/Native/UIElementsRuntimeUtilityNative.h")]
    [VisibleToOtherModules("Unity.UIElements")]
    internal static class UIElementsRuntimeUtilityNative
    {
        internal static Action UpdateRuntimePanelsCallback;
        internal static Action RepaintOverlayPanelsCallback;
        internal static Action RepaintOffscreenPanelsCallback;
        internal static Action RepaintWorldPanelsCallback;

        [RequiredByNativeCode]
        // 更新 Runtime 的面板
        public static void UpdateRuntimePanels()
        {
            UpdateRuntimePanelsCallback?.Invoke();
        }

        [RequiredByNativeCode]
        // 重画 Overlay 面板
        public static void RepaintOverlayPanels()
        {
            RepaintOverlayPanelsCallback?.Invoke();
        }

        [RequiredByNativeCode]
        // 重画 画面之外 的面板
        public static void RepaintOffscreenPanels()
        {
            RepaintOffscreenPanelsCallback?.Invoke();
        }

        [RequiredByNativeCode]
        // 重画 世界坐标的 面板
        // ？现在都没有世界坐标的支持，应该是仅做了接口设计
        public static void RepaintWorldPanels()
        {
            RepaintWorldPanelsCallback?.Invoke();
        }

        // 注册 Playerloop 回调
        public extern static void RegisterPlayerloopCallback();
        // 注销 Playerloop 回调
        public extern static void UnregisterPlayerloopCallback();

        // 看不懂这个方法，没有返回值，没有参数，仅有一个 Focusable 引用了它，Focusable 是 VisualElement 的父类
        public extern static void VisualElementCreation();
    }
}
