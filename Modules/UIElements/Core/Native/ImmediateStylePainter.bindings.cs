// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
    [NativeHeader("Modules/UIElements/Core/Native/ImmediateStylePainter.h")]
    [StructLayout(LayoutKind.Sequential)]
    // ImmediateStyle 绘制器
    internal class ImmediateStylePainter
    {
        // 绘制 Rect
        internal static extern void DrawRect(Rect screenRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses);

        // 绘制 Texture
        internal static extern void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses, int leftBorder, int topBorder,
            int rightBorder, int bottomBorder, bool usePremultiplyAlpha);

        // 绘制 Text
        internal static extern void DrawText(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap,
            float wordWrapWidth, bool richText, TextClipping textClipping);
    }
}
