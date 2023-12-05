// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using Unity.Profiling;
using UnityEngine.Scripting;
using UnityEngine.Rendering;

namespace UnityEngine.UIElements
{
    using UIR;

    // 主要内容在文件的下面
    // 一个渲染器组件，它应该被添加到uiddocument组件的旁边
    // 世界空间渲染。这个组件是由uiddocument自动添加的
    // 在世界空间中配置PanelSettings资源。

    /// <summary>
    /// A renderer Component that should be added next to a UIDocument Component to allow
    /// world-space rendering. This Component is added automatically by the UIDocument when
    /// the PanelSettings asset is configured in world-space.
    /// </summary>
    [NativeType(Header = "Modules/UIElements/Core/Native/Renderer/UIRenderer.h")]
    public sealed class UIRenderer : Renderer
    {
        // volatile 关键字指示一个字段可以由多个同时执行的线程修改
        // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/volatile
        internal volatile List<CommandList>[] commandLists;
        internal volatile bool skipRendering;

        internal extern void SetNativeData(int safeFrameIndex, int cmdListIndex, Material mat);

        [RequiredByNativeCode]
        static void OnRenderNodeExecute(UIRenderer renderer, int safeFrameIndex, int cmdListIndex)
        {
            if (renderer.skipRendering)
                return;

            var commandLists = renderer.commandLists;
            var cmdList = commandLists != null ? commandLists[safeFrameIndex] : null;
            if (cmdList != null && cmdListIndex < cmdList.Count)
                cmdList[cmdListIndex]?.Execute();
        }
    }
}

namespace UnityEngine.UIElements.UIR
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GfxUpdateBufferRange
    {
        public UInt32 offsetFromWriteStart;
        public UInt32 size;
        public UIntPtr source;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DrawBufferRange
    {
        public int firstIndex;
        public int indexCount;
        public int minIndexVal;
        public int vertsReferenced;
    }

    [NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRendererUtility.h")]
    [VisibleToOtherModules("Unity.UIElements")]
    // UI渲染器工具集
    internal partial class Utility
    {
        internal enum GPUBufferType { Vertex, Index }
        unsafe public class GPUBuffer<T> : IDisposable where T : struct
        {
            IntPtr buffer;
            int elemCount;
            int elemStride;

            unsafe public GPUBuffer(int elementCount, GPUBufferType type)
            {
                elemCount = elementCount;
                elemStride = UnsafeUtility.SizeOf<T>();
                buffer = AllocateBuffer(elementCount, elemStride, type == GPUBufferType.Vertex);
            }

            public void Dispose()
            {
                FreeBuffer(buffer);
            }

            public void UpdateRanges(NativeSlice<GfxUpdateBufferRange> ranges, int rangesMin, int rangesMax)
            {
                UpdateBufferRanges(buffer, new IntPtr(ranges.GetUnsafePtr()), ranges.Length, rangesMin, rangesMax);
            }

            public int ElementStride { get { return elemStride; } }
            public int Count { get { return elemCount; } }
            internal IntPtr BufferPointer { get { return buffer; } }
        }

        unsafe public static void SetVectorArray<T>(MaterialPropertyBlock props, int name, NativeSlice<T> vector4s) where T : struct
        {
            int vector4Count = (vector4s.Length * vector4s.Stride) / (sizeof(float) * 4);
            SetVectorArray(props, name, new IntPtr(vector4s.GetUnsafePtr()), vector4Count);
        }

        // 图形资源重建时
        public static event Action<bool> GraphicsResourcesRecreate;
        // 引擎 update 时
        public static event Action EngineUpdate;
        // 清除挂起的资源时
        public static event Action FlushPendingResources;

        [RequiredByNativeCode]
        internal static void RaiseGraphicsResourcesRecreate(bool recreate)
        {
            GraphicsResourcesRecreate?.Invoke(recreate);
        }

        static ProfilerMarker s_MarkerRaiseEngineUpdate = new ProfilerMarker("UIR.RaiseEngineUpdate");

        [RequiredByNativeCode]
        // RequiredByNativeCode 是什么鬼？ 这种写法会在 Update 时执行？
        internal static void RaiseEngineUpdate()
        {
            if (EngineUpdate != null)
            {
                s_MarkerRaiseEngineUpdate.Begin();
                EngineUpdate.Invoke();
                s_MarkerRaiseEngineUpdate.End();
            }
        }

        [RequiredByNativeCode]
        internal static void RaiseFlushPendingResources()
        {
            FlushPendingResources?.Invoke();
        }

        // 分配 buffer
        [ThreadSafe] extern static IntPtr AllocateBuffer(int elementCount, int elementStride, bool vertexBuffer);
        // 释放 buffer
        [ThreadSafe] extern static void FreeBuffer(IntPtr buffer);
        [ThreadSafe] extern static void UpdateBufferRanges(IntPtr buffer, IntPtr ranges, int rangeCount, int writeRangeStart, int writeRangeEnd);
        [ThreadSafe] extern static void SetVectorArray(MaterialPropertyBlock props, int name, IntPtr vector4s, int count);
        [ThreadSafe] public extern static IntPtr GetVertexDeclaration(VertexAttributeDescriptor[] vertexAttributes);

        // 绘制一个范围
        [ThreadSafe] public extern unsafe static void DrawRanges(IntPtr ib, IntPtr* vertexStreams, int streamCount, IntPtr ranges, int rangeCount, IntPtr vertexDecl);
        [ThreadSafe] public extern static void SetPropertyBlock(MaterialPropertyBlock props);
        [ThreadSafe] public extern static void SetScissorRect(RectInt scissorRect);
        [ThreadSafe] public extern static void DisableScissor();
        [ThreadSafe] public extern static bool IsScissorEnabled();
        [ThreadSafe] public extern static IntPtr CreateStencilState(StencilState stencilState);
        // 设置模板状态
        [ThreadSafe] public extern static void SetStencilState(IntPtr stencilState, int stencilRef);
        [ThreadSafe] public extern static bool HasMappedBufferRange();
        // 在计算机领域，"Fence"通常指的是内存屏障或内存栅栏。内存屏障是一种同步机制，用于确保指令按照程序员的意图进行重新排序。
        // 内存栅栏可以分为多种类型，包括加载屏障、存储屏障和全屏障，它们有助于确保内存操作的顺序性。内存屏障在并行计算和多线程编程中起到关键作用，帮助确保正确性和一致性。
        [ThreadSafe] public extern static UInt32 InsertCPUFence();
        // Fence标志通常用于标识CPU执行的任务或命令是否已经完成
        [ThreadSafe] public extern static bool CPUFencePassed(UInt32 fence);
        // 在图形编程中帮助实现CPU和GPU之间的同步，确保在需要时CPU能够等待GPU端任务的完成。
        [ThreadSafe] public extern static void WaitForCPUFencePassed(UInt32 fence);
        // 同步渲染线程
        [ThreadSafe] public extern static void SyncRenderThread();
        [ThreadSafe] public extern static RectInt GetActiveViewport();

        // profile begin 服务于 ProfilerMarker
        [ThreadSafe] public extern static void ProfileDrawChainBegin();
        // profile end 服务于 ProfilerMarker
        [ThreadSafe] public extern static void ProfileDrawChainEnd();
        // 通知 UIR 事件
        public extern static void NotifyOfUIREvents(bool subscribe);
        // 获取 Unity 投影矩阵
        [ThreadSafe] public extern static Matrix4x4 GetUnityProjectionMatrix();
        // 原意应该是 获取 GPU 投影矩阵
        [ThreadSafe] public extern static Matrix4x4 GetDeviceProjectionMatrix();
        [ThreadSafe] public extern static bool DebugIsMainThread(); // For debug code only
    }
}
