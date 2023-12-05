// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System.Runtime.CompilerServices;

// OK: "friend" and test assemblies from the same product area, evolving in lockstep with this module
// 来自同一产品区域的“朋友”和测试组件，与本模块同步发展

// 用于测试 UI Toolkit
[assembly: InternalsVisibleTo("Unity.UIElements.Tests")]
[assembly: InternalsVisibleTo("Unity.UIElements.PlayModeTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.RuntimeTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.TestComponents")]
[assembly: InternalsVisibleTo("Assembly-CSharp-testable")]

[assembly: InternalsVisibleTo("UnityEngine.UIElements.Tests")] // for UI Test Framework

// 一个单元测试框架 主要用来做 Mock 相关介绍：
// https://learn.microsoft.com/zh-cn/shows/visual-studio-toolbox/unit-testing-moq-framework
// https://zhuanlan.zhihu.com/p/355346063
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // for Moq

// 编辑器环境下的一些依赖
[assembly: InternalsVisibleTo("UnityEditor.StyleSheetsModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIBuilderModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsModule")]

// 编辑器环境下的测试用例(不确定)
[assembly: InternalsVisibleTo("Unity.UIElements.EditorTests")]
[assembly: InternalsVisibleTo("Unity.UIElements.TestComponents.Editor")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-testable")]
[assembly: InternalsVisibleTo("Unity.UI.Builder.EditorTests")]
[assembly: InternalsVisibleTo("Unity.UXMLReferenceGenerator.Bridge")]
[assembly: InternalsVisibleTo("UnityEditor.UIElements.Tests")] // for UI Test Framework

// TOLERATED: modules or core packages evolving in lockstep with this module
// Reducing this list means to improve the API design of this module.
// 可容忍:模块或核心包与此模块同步发展
// 减少这个列表意味着改进这个模块的API设计。

// 注意这里：它跟 UGUI 还有点耦合
[assembly: InternalsVisibleTo("UnityEngine.UI")] // com.unity.ugui

[assembly: InternalsVisibleTo("UnityEditor.CoreModule")]
[assembly: InternalsVisibleTo("UnityEditor.EditorToolbarModule")]
[assembly: InternalsVisibleTo("UnityEditor.GraphViewModule")]
[assembly: InternalsVisibleTo("UnityEditor.GraphToolsFoundationModule")]
[assembly: InternalsVisibleTo("UnityEditor.Graphs")]
[assembly: InternalsVisibleTo("UnityEditor.GridAndSnapModule")] // ButtonStripField
[assembly: InternalsVisibleTo("UnityEditor.PresetsModule")]
[assembly: InternalsVisibleTo("UnityEditor.PresetsUIModule")]
[assembly: InternalsVisibleTo("UnityEditor.QuickSearchModule")]
[assembly: InternalsVisibleTo("UnityEditor.SceneTemplateModule")]
[assembly: InternalsVisibleTo("UnityEditor.SceneViewModule")]
[assembly: InternalsVisibleTo("UnityEditor.UnityConnectModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsSamplesModule")]
[assembly: InternalsVisibleTo("UnityEditor.UI.EditorTests")]

[assembly: InternalsVisibleTo("Unity.2D.Sprite.Editor")] // com.unity.2d.sprite: VisualElement.styleSheetList, FocusController.IsFocused
[assembly: InternalsVisibleTo("Unity.2D.Tilemap.Editor")] // com.unity.2d.tilemap: IGenericMenu
[assembly: InternalsVisibleTo("Unity.2D.Tilemap.EditorTests")] // com.unity.2d.tilemap.tests: UIElementsUtility

// NOT TOLERATED: assemblies distributed in packages not evolving in lockstep with this module
// Until this list is empty, your internal API is included in your public API, and changing internal APIs is considered a breaking change.
// 不允许:在包中分发的程序集不与该模块同步发展
// 在此列表为空之前，你的内部API被包含在你的公共API中，改变内部API被认为是一个破坏性的改变。

// 从命名看，下面是一些用于访问C++API的魔法库（尾缀了他们相关的上层库）。没有具体深究内部的实现
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.001")] // com.unity.2d.common: VisualElement.pseudoStates, PseudoStates
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.002")] // com.unity.entities: VisualElementBridge.cs, ListViewBridge.cs
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.003")] // com.unity.vectorgraphics: VectorImage, GradientSettings
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.015")] // com.unity.graphtoolsauthoringframework: TextUtilities
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.017")] // com.unity.motion: UIElementsUtility


[assembly: InternalsVisibleTo("UnityEditor.Purchasing")] // com.unity.purchasing, VisualElement.AddStyleSheetPath
