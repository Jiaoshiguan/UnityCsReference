# UI Toolkit 源码阅读

## 前置知识

在阅读源码之前，应该大体去了解一下 UI Toolkit，可以通过官方资料或者这篇文档进行。// TODO: 贴一下之前写的文档

## 源码来源

Unity正在越来越重视C#语言和.NET运行时，本文主要讨论的 UI Toolkit 就是基于C#的产物。

可以通过下面几点发现Unity对待C#的态度：

1. 推出了 Burst，可以利用LLVM将C#编译为原生代码。
2. 将Mono .NET Runtime转换为.NET CoreCLR。
3. 正在将代码重心从C++迁移到C#，UI Toolkit 和 UGUI 等很多重要的框架、组件都在此之内。（不光是做一层 Wrapper，很多逻辑都是编写在C#层的）
4. Unity 官方将 CSharp 部分的第一方package等，都共享到了 github 的仓库中——[UnityCSReference](https://github.com/Unity-Technologies/UnityCsReference)。

[.NET和Unity的未来 - 技术专栏 - Unity官方开发者社区](https://developer.unity.cn/projects/62bbc040edbc2a7848d45ae8)

[Unity - Manual: Overview of .NET in Unity --- Unity - 手册：Unity 中的 .NET 概述 (unity3d.com)](https://docs.unity3d.com/Manual/overview-of-dot-net-in-unity.html)

### 一份 fork

为了便于自己阅读、笔记和演示，我 fork 了一份代码到 [Jiaoshiguan/UnityCsReference](https://github.com/Jiaoshiguan/UnityCsReference) 之中。

在这份 fork 里，可以看到一些中文注释和逻辑图、额外文档等。



## 为什么要读 UI Toolkit 的源码

### 学习它的实现细节和设计思路

- 将思路和技巧应用到其他地方。
- 更好的去使用这个框架。（性能、效率、工作流）

### 尝试解决World Space

目前官方并没有为 UI Toolkit 提供世界空间关系的 3D UI 

尝试从源码去了解它没有3D化的原因是什么？还拥有那些限制？能不能自己解决？

### 尝试解决UX for XR

我觉得未来 XR 的 UX 一定不是只基于 2D 界面的，但可能受限于开发工具链的成熟度和开发者习惯，或许是3-5年内都是类似visionPro中那些windowed应用或YVR HOME一样，仅仅做一些z轴上的偏移或多个面片的摆放，还是使用2维面片而非3D模型。

去深入了解一个 2D UX 框架，可能是解决和深入理解XR中UX问题比较好的方式。将 2D UX 的做法作为参考后，可以通过类比和发散，去进行展望和设想。

## 源码解析

与 UI Toolkit 相关性极强的代码库主要分为以下几个：

- 🏠UIBuilder：用来创建和查看、编辑 UI Toolkit 中资源对象的工具。~~与运行时无关。~~

- 🏠UIElementsEditor：

  - 用来处理 Editor 中的调试功能
    - 高亮调试元素
    - 展示和编辑调试数据

  - 提供部分仅在 Editor 下生效的组件和功能
    - Tooltip
    - Bindings

  - 提供一些与 UI Toolkit 相关的 Mono组件的自定义 Inspector 面板
    - UIDocument
    - PanelSettings
    - UIRendererEditor（在 Inspector 中对 UXML 元素进行预览）

  - 提供一些代码生成
    - UIElementsTemplate
    - USS和UXML的右键创建菜单

- 🏠UIElementsSamplesEditor：一个展示UIElements功能的小窗口。~~只是一些用法上的小展示，没有什么实际意义。~~

- 🚗[UIElements](./SourceCodeNotes.md)：他就是 UI Toolkit 的 Runtime 核心，是这次会议会主要讨论的一个库。

  - 样式解析

  - 布局计算

  - 动态图集生成和贴图合批

  - 圆角计算

  - 渲染

  - 事件生成和传递