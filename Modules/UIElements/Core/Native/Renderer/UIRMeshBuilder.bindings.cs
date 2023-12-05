// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
    [NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRMeshBuilder.bindings.h")]
    // Mesh 构造器
    internal static class MeshBuilderNative
    {
        public const float kEpsilon = 0.001f;

        public struct NativeColorPage
        {
            public int isValid;
            public Color32 pageAndID;
        }

        // 边框参数
        public struct NativeBorderParams
        {
            public Rect rect;

            public Color leftColor;
            public Color topColor;
            public Color rightColor;
            public Color bottomColor;

            public float leftWidth;
            public float topWidth;
            public float rightWidth;
            public float bottomWidth;

            // 标记了圆角
            public Vector2 topLeftRadius;
            public Vector2 topRightRadius;
            public Vector2 bottomRightRadius;
            public Vector2 bottomLeftRadius;

            internal NativeColorPage leftColorPage;
            internal NativeColorPage topColorPage;
            internal NativeColorPage rightColorPage;
            internal NativeColorPage bottomColorPage;
        }

        // Rect 参数
        public struct NativeRectParams
        {
            public Rect rect;
            public Rect subRect;
            public Rect uv;
            public Color color;
            public ScaleMode scaleMode;

            // 标记了圆角
            public Vector2 topLeftRadius;
            public Vector2 topRightRadius;
            public Vector2 bottomRightRadius;
            public Vector2 bottomLeftRadius;

            public IntPtr texture;
            public IntPtr sprite;
            public IntPtr vectorImage;

            // Extracted sprite properties for the job system
            public IntPtr spriteTexture;
            public IntPtr spriteVertices;
            public IntPtr spriteUVs;
            public IntPtr spriteTriangles;

            public Rect spriteGeomRect;
            public Vector2 contentSize;
            public Vector2 textureSize;
            public float texturePixelsPerPoint;

            public int leftSlice;
            public int topSlice;
            public int rightSlice;
            public int bottomSlice;
            public float sliceScale;

            public Vector4 rectInset;

            public NativeColorPage colorPage;

            public int meshFlags;
        }

        // 生成边框
        [ThreadSafe] public static extern MeshWriteDataInterface MakeBorder(NativeBorderParams borderParams, float posZ);
        // 生成实体的矩形
        [ThreadSafe] public static extern MeshWriteDataInterface MakeSolidRect(NativeRectParams rectParams, float posZ);
        // 生成有纹理的矩形
        [ThreadSafe] public static extern MeshWriteDataInterface MakeTexturedRect(NativeRectParams rectParams, float posZ);
        // 构造矢量图形，拉伸背景
        [ThreadSafe] public static extern MeshWriteDataInterface MakeVectorGraphicsStretchBackground(Vertex[] svgVertices, UInt16[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Rect sourceUV, ScaleMode scaleMode, Color tint, NativeColorPage colorPage);
        // 构造矢量图形，9切片背景
        // LTRB 是 left top right bottom
        [ThreadSafe] public static extern MeshWriteDataInterface MakeVectorGraphics9SliceBackground(Vertex[] svgVertices, UInt16[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Vector4 sliceLTRB, Color tint, NativeColorPage colorPage);
    }
}
