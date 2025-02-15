// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// 包含了具体的Jobs，例如 mesh 拷贝、mesh 转换、
namespace UnityEngine.UIElements.UIR
{
    class JobManager : IDisposable
    {
        NativePagedList<NudgeJobData> m_NudgeJobs = new NativePagedList<NudgeJobData>(64);
        NativePagedList<ConvertMeshJobData> m_ConvertMeshJobs = new NativePagedList<ConvertMeshJobData>(64);
        NativePagedList<CopyMeshJobData> m_CopyMeshJobs = new NativePagedList<CopyMeshJobData>(64);

        JobMerger m_JobMerger = new JobMerger(128);

        public void Add(ref NudgeJobData job)
        {
            m_NudgeJobs.Add(ref job);
        }

        public void Add(ref ConvertMeshJobData job)
        {
            m_ConvertMeshJobs.Add(ref job);
        }

        public void Add(ref CopyMeshJobData job)
        {
            m_CopyMeshJobs.Add(ref job);
        }

        public unsafe void CompleteNudgeJobs()
        {
            foreach (NativeSlice<NudgeJobData> page in m_NudgeJobs.GetPages())
                m_JobMerger.Add(JobProcessor.ScheduleNudgeJobs((IntPtr)page.GetUnsafePtr(), page.Length));
            m_JobMerger.MergeAndReset().Complete();
            m_NudgeJobs.Reset();
        }

        public unsafe void CompleteConvertMeshJobs()
        {
            foreach (NativeSlice<ConvertMeshJobData> page in m_ConvertMeshJobs.GetPages())
                m_JobMerger.Add(JobProcessor.ScheduleConvertMeshJobs((IntPtr)page.GetUnsafePtr(), page.Length));
            m_JobMerger.MergeAndReset().Complete();
            m_ConvertMeshJobs.Reset();
        }

        public unsafe void CompleteCopyMeshJobs()
        {
            foreach (NativeSlice<CopyMeshJobData> page in m_CopyMeshJobs.GetPages())
                m_JobMerger.Add(JobProcessor.ScheduleCopyMeshJobs((IntPtr)page.GetUnsafePtr(), page.Length));
            m_JobMerger.MergeAndReset().Complete();
            m_CopyMeshJobs.Reset();
        }

        #region Dispose Pattern

        protected bool disposed { get; private set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                m_NudgeJobs.Dispose();
                m_ConvertMeshJobs.Dispose();
                m_CopyMeshJobs.Dispose();

                m_JobMerger.Dispose();
            }
            else DisposeHelper.NotifyMissingDispose(this);

            disposed = true;
        }

        #endregion // Dispose Pattern
    }

    // *** The following structs must remain in sync with those defined in UIRendererJobProcessor ***
    [StructLayout(LayoutKind.Sequential)]
    struct NudgeJobData
    {
        public IntPtr headSrc;
        public IntPtr headDst;
        public int headCount;

        public IntPtr tailSrc;
        public IntPtr tailDst;
        public int tailCount;

        public Matrix4x4 transform;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ConvertMeshJobData
    {
        public IntPtr vertSrc;
        public IntPtr vertDst;
        public int vertCount;
        public Matrix4x4 transform;
        public Color32 xformClipPages;
        public Color32 ids;
        public Color32 addFlags;
        public Color32 opacityPage;
        public Color32 textCoreSettingsPage;
        public int isSdfText;
        public float textureId;
        public int gradientSettingsIndexOffset;

        public IntPtr indexSrc;
        public IntPtr indexDst;
        public int indexCount;
        public int indexOffset;

        public int flipIndices;
        public int forceZ;
        public float positionZ;

        public int remapUVs;
        public Rect atlasRect;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CopyMeshJobData
    {
        public IntPtr vertSrc;
        public IntPtr vertDst;
        public int vertCount;

        public IntPtr indexSrc;
        public IntPtr indexDst;
        public int indexCount;
        public int indexOffset;
    }
}
