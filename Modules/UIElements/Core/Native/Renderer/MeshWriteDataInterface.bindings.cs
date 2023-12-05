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
    // LayoutKind:当将对象导出到非托管代码时，控制该对象的布局。https://learn.microsoft.com/zh-cn/dotnet/api/system.runtime.interopservices.layoutkind?view=net-7.0
    // Auto	3	
    // 运行库自动为非托管内存中的对象的成员选择适当的布局。 使用此枚举成员定义的对象不能在托管代码的外部公开。 尝试这样做将引发异常。
    // Explicit	2	
    // 在未管理内存中的每一个对象成员的精确位置是被显式控制的，服从于 Pack 字段的设置。 每个成员必须使用 FieldOffsetAttribute 指示该字段在类型中的位置。
    // Sequential	0	
    // 对象的成员按照它们在被导出到非托管内存时出现的顺序依次布局。 这些成员根据在 Pack 中指定的封装进行布局，并且可以是不连续的。

    // 定义了 mesh 的顶点和索引
    [StructLayout(LayoutKind.Sequential)]
    internal struct MeshWriteDataInterface
    {
        public IntPtr vertices;
        public IntPtr indices;
        public int vertexCount;
        public int indexCount;
    }
}
