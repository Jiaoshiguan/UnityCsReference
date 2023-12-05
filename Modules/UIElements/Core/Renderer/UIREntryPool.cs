// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

﻿using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
    // 该类是 ImplicitPool 的包装。 它是为了 UI Toolkit 网格生成的工作化而引入的。
    // 获取条目是线程安全的（仅来自主线程和作业工作者）。
    // 给定线程有一个从共享 ImplicitPool 获取数据的子池。 当子池为空时，获取一批条目 来自隐式池。

    // This class is a wrapper over ImplicitPool. It was introduced for the jobification of the UI Toolkit mesh
    // generation. Getting an entry is thread-safe (from main thread and job workers only). A given thread has a
    // sub-pool which feeds from the shared ImplicitPool. When the sub-pool is empty, a batch of entries is acquired
    // from the ImplicitPool.
    class EntryPool
    {
        const int k_StackSize = 128;
        Stack<Entry>[] m_ThreadEntries;
        ImplicitPool<Entry> m_SharedPool;

        static readonly Func<Entry> k_CreateAction = () => new Entry();
        static readonly Action<Entry> k_ResetAction = e =>
        {
            e.nextSibling = null;
            e.firstChild = null;
            e.lastChild = null;
            e.texture = null;
            e.material = null;
            e.gradientsOwner = null;
        };

        public EntryPool(int maxCapacity = 1024)
        {
            m_ThreadEntries = new Stack<Entry>[JobsUtility.ThreadIndexCount];
            for (int i = 0, count = JobsUtility.ThreadIndexCount; i < count; ++i)
                m_ThreadEntries[i] = new Stack<Entry>(k_StackSize);

            m_SharedPool = new ImplicitPool<Entry>(k_CreateAction, k_ResetAction, 128, maxCapacity);
        }

        public Entry Get()
        {
            Stack<Entry> entries = m_ThreadEntries[UIRUtility.GetThreadIndex()];

            if (entries.Count == 0)
            {
                // Take many from the pool at once
                lock (m_SharedPool)
                {
                    for (int i = 0; i < k_StackSize; ++i)
                        entries.Push(m_SharedPool.Get());
                }
            }

            return entries.Pop();
        }

        public void ReturnAll()
        {
            for (int i = 0, count = m_ThreadEntries.Length; i < count; ++i)
                m_ThreadEntries[i].Clear();

            m_SharedPool.ReturnAll();
        }
    }
}
