// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TextNativeSettings
    {
        public string text;
        public Font font;
        public int size;
        public float scaling;
        public FontStyle style;
        public Color color;
        public TextAnchor anchor;
        public bool wordWrap;
        public float wordWrapWidth;
        public bool richText;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TextVertex
    {
        public Vector3 position;
        public Color32 color;
        public Vector2 uv0;
    }

    [NativeHeader("Modules/UIElements/Core/Native/TextNative.bindings.h")]
    // 文本
    internal static class TextNative
    {
        // 获取光标位置
        public static Vector2 GetCursorPosition(TextNativeSettings settings, Rect rect, int cursorIndex)
        {
            if (settings.font == null)
            {
                Debug.LogError("Cannot process a null font.");
                return Vector2.zero;
            }

            return DoGetCursorPosition(settings, rect, cursorIndex);
        }

        // 计算文本宽度
        public static float ComputeTextWidth(TextNativeSettings settings)
        {
            if (settings.font == null)
            {
                Debug.LogError("Cannot process a null font.");
                return 0;
            }

            if (string.IsNullOrEmpty(settings.text))
                return 0;

            return DoComputeTextWidth(settings);
        }

        // 计算文本高度
        public static float ComputeTextHeight(TextNativeSettings settings)
        {
            if (settings.font == null)
            {
                Debug.LogError("Cannot process a null font.");
                return 0;
            }

            if (string.IsNullOrEmpty(settings.text))
                return 0;

            return DoComputeTextHeight(settings);
        }

        // 获取顶点
        public static unsafe NativeArray<TextVertex> GetVertices(TextNativeSettings settings)
        {
            int vertexCount = 0;
            GetVertices(settings, IntPtr.Zero, UnsafeUtility.SizeOf<TextVertex>(), ref vertexCount);

            var array = new NativeArray<TextVertex>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            if (vertexCount > 0)
            {
                GetVertices(settings, (IntPtr)array.GetUnsafePtr(), UnsafeUtility.SizeOf<TextVertex>(), ref vertexCount);
                Debug.Assert(vertexCount == array.Length);
            }
            return array;
        }

        // 获取偏移
        public static Vector2 GetOffset(TextNativeSettings settings, Rect screenRect)
        {
            if (settings.font == null)
            {
                Debug.LogError("Cannot process a null font.");
                return new Vector2(0, 0);
            }

            settings.text = settings.text ?? "";

            return DoGetOffset(settings, screenRect);
        }

        // 计算文本缩放
        public static float ComputeTextScaling(Matrix4x4 worldMatrix, float pixelsPerPoint)
        {
            var axisX = new Vector3(worldMatrix.m00, worldMatrix.m10, worldMatrix.m20);
            var axisY = new Vector3(worldMatrix.m01, worldMatrix.m11, worldMatrix.m21);
            float worldScale = (axisX.magnitude + axisY.magnitude) / 2;
            return worldScale * pixelsPerPoint;
        }

        [FreeFunction(Name = "TextNative::ComputeTextWidth")]
        // 计算文本宽度
        private static extern float DoComputeTextWidth(TextNativeSettings settings);

        [FreeFunction(Name = "TextNative::ComputeTextHeight")]
        // 计算文本高度
        private static extern float DoComputeTextHeight(TextNativeSettings settings);

        [FreeFunction(Name = "TextNative::GetCursorPosition")]
        // 获取焦点坐标
        private static extern Vector2 DoGetCursorPosition(TextNativeSettings settings, Rect rect, int cursorPosition);

        [FreeFunction(Name = "TextNative::GetVertices")]
        // 生成顶点
        private static extern void GetVertices(TextNativeSettings settings, IntPtr buffer, int vertexSize, ref int vertexCount);

        [FreeFunction(Name = "TextNative::GetOffset")]
        // 获取偏移
        private static extern Vector2 DoGetOffset(TextNativeSettings settings, Rect rect);
    }
}
