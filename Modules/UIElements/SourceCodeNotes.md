## UIToolkit 源码解析

整个 UI Toolkit 包，位于`UnityCsReference/Modules/UIElements`。整体结构包含了`Core`,`InputSystem`,`Managed`,`ScriptBindings`四个文件夹和`AssemblyInfo.cs`一个配置文件。

![image-20231205134739144](/Users/jiaoshiguan/Library/Application Support/typora-user-images/image-20231205134739144.png)

需要注意的是：**它现在作为内置包，不能被修改**。Unity Editor也依赖了它，修改或移除它后，可能会让整个Editor都无法启动和运行。但官方**后期**有将它作为一个普通package发布的意向。（论坛的官方讨论）

> UIToolkit的文件夹名是UIElements的原因是，这个包在最开始被命名为UIElements，后期才被改名为 UI Toolkit，官方为了向前兼容和稳定性，就没有调整这个命名空间和文件夹名，只是在宣传概念上改为了UI ToolKit。
>
> 另外，xxToolKit应该是Unity官方对**功能复杂的内部包**的一个新命名规范，类似的还有XRITK。

阅读的版本为`master`分支的`2023.2.2f1`，后面以文件夹为结构来逐个解析。

## AssemblyInfo

AssemblyInfo是C#技术栈中的概念，它定义了程序集相关信息的值，一般是自动生成的，其功能类似与Unity中的`AsmDef`，分为以下几组信息：

- 程序集标识特性
- 信息性特性
- 程序集清单特性
- 强名称特性

若想了解更多AssemblyInfo，可以参考下面的文档：

[设置程序集属性 - .NET | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/standard/assembly/set-attributes)

[Microsoft.NET.Sdk 的 MSBuild 属性 - .NET | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/core/project-sdk/msbuild-props#assembly-attribute-properties)



## Core

core定义了很多子文件夹来划分一些细分功能，例如数据绑定、渲染等。除此外，在根目录也有大量对象。

下面从各个文件夹进行解析，最后再阅读一下根目录中的几个关键脚本。



### 1. 数据绑定

#### Bindings 文件夹

定义了一系列绑定模式，以及数据绑定的具体实现。利用一个path来持久化存储了它所对应的数据。并利用一个`BindingUpdateTrigger`来使数据改变时，能够响应到绑定的属性上。

主要应用情景是：

1. UI Builder中修改属性，能直接映射到UXML和预览器中。
2. UI Debugger中修改属性，在Editor Runtime直接能反映到这个修改。

#### Conversions 文件夹 [internal]

里面有一个访问者(Visitor Pattern)，主要服务于数据绑定，帮助访问特定路径的属性。



### 2. 控件

用户界面上的控件是**用来与用户交互**以及**控制应用程序行为的元素**。Controls可以包括按钮、复选框、文本输入框、下拉菜单、滑块等，通过这些控件用户可以进行各种操作，比如输入信息、进行选择、触发特定的功能，或者调整应用程序的设置。

#### Collections 文件夹

定义了一些**容器**概念的抽象控制器**，例如用于容纳**可回收列表**元素的`ListViewController`和用于展示多列**树视图**的`DefaultMultiColumnTreeViewController<T>`。

**抽象控制器**本身并不作为控件被直接使用，它描述了容器对象的回收机制、构造方式、复用方法等。

它是具体**控件**的抽象定义。

#### Controls 文件夹

包含了一些非容器的**抽象控制器**，以及大量基于这些抽象，已经实现好的基础的控件，如：`Image`、`Label`、`DropdownField`等。

需要注意的是，**容器**概念的控件实现也在这个文件夹内，例如：`TreeView`和`ScrollView`等。

> 这个文件夹是非常重要的，熟悉了它才能更好的使用各种控件的上层接口，已经正确的响应和操作控件。



### 3. 事件

#### DragAndDrop 文件夹 [internal]

主要定义了一个`DragEventsProcessor`用来处理上层控件的拖拽行为，通过UITK的`PointerDownEvent`、`PointerUpEvent`、`PointerLeaveEvent`等，识别出拖拽行为，并利用事件参数计算出拖拽的具体数据。

属于一个工具类，服务与上层的`ListView`、`TreeView`等需要拖拽的控件。

#### Events 文件夹

定义了一系列输入事件和UI事件。事件的定义，如：Pointer事件、Click事件、普通的控件事件等，都与uGUI很类似。

但事件在传递设计上更强大，引入了两个新概念：**涓流**和**冒泡**。

有所不同的地方是：

1. LayoutEvent：此事件在布局计算之后，当元素的位置或尺寸发生变化时发送。
2. LinkTagEvent：对Link类型有额外支持。
3. PanelEvent：元素在被`Attach`和`Detach`时，会触发的事件。它不代表元素本身被销毁或生成，这只意味着它开始参与渲染和离开渲染了。类似GameObject对象的添加到场景树。
4. TransitionEvent：触发UI动画时的一系列事件。
5. ChangeEvent：泛型的值更改事件，类似于ReactiveProperty，涵盖了toggle(bool)、input field(string)、dropdown(int、string)等可交互控件的值修改事件。

### 4. 渲染*

渲染部分拥有9万行左右的代码，但也是这次阅读最重视的部分。

关键的类：

1. UITKTextJobSystem.cs：文本渲染流程



#### Native 文件夹

这部分的脚本利用`extern`关键字和`[NativeHeader]`与C++部分建立绑定，但他没有使用`[DllImport]`而是部分使用了 `[FreeFunction]`。

下面是关于`FreeFunction`特性原理的讨论：[How does the [FreeFunction\] attribute work?](https://forum.unity.com/threads/how-does-the-freefunction-attribute-work.857653/)

它们描述了哪部分行为是**没有**在C#层处理的，这些应该都是引擎的*核心功能*或*性能强相关*功能，值得仔细看看。

见源码。



#### Layout 文件夹

其中主要做了一些Layout的数据定义。

主要是`LayoutNode`，它提供了：

1. 左右边界、上下边界、世界空间、相对空间等属性。
2. Next、Parent、Index等布局元素的关系访问和索引访问。
3. Insert、Remove、Clear等布局操作接口。

但遗憾的是，它基本上是一个C#层的封装，通过`LayoutDataAccess`和`LayoutHandle`获取来自Native的方法。

它的所有核心工作都在Native部分，例如布局计算，移除元素，添加元素等。

> 这里也不是一无是处，因为他跟native部分交互紧密。
>
> 里面使用了大量的值类型和相关特性（ref、unsafe、unmanaged、ptr、Unity.Collections.LowLevel.Unsafe）。
>
> 有与Native高效率、高频次交换数据的情景时，可以参考这里的实现。



#### Text 文件夹

主要控制了文本的渲染。

文本输入事件的控制、TextGenerationSettings和TextGenerator等。

见源码。



#### Renderer 文件夹

这部分代码量巨大，不可能详细的阅读和解析完所有部分。



#### Style、StyleSheets 文件夹

主要提供了对uss（css）文件的解析能力，核心是一个Regex：

```c#
(?<id>#[-]?\w[\w-]*)|(?<class>\.[\w-]+)|(?<pseudoclass>:[\w-]+(\((?<param>.+)\))?)|(?<type>[^\-]\w+)|(?<wildcard>\*)|\s+
```

另外，还定义了一堆变量类型，用来描述宽高等样式的属性。

里面对`struct`、`运算符重载`和`Equals重写`运用的很多，想参考这一块的话，值得一看。

#### UXML 文件夹

负责处理uxml资源的序列化和实例化等。里面可以看到它是如何通过一个SO脚本+UXML文本，提供UI布局和USS的链接，并在Editor视图提供预览和信息查询等。



### 5. 其他

#### GameObjects 文件夹

因为UI Toolkit脱离了GameObjects和MonoBehavior，但Unity的Scene又基于此。所以需要一个GameObject来建立UI Toolkit与Scene的联系。

这个文件夹下的文件大多数是服务于此的，见源码。



### 6. Core 根目录

#### VisualElement *

里面与渲染最相关的地方是他有个internal的uiRenderer字段。





## InputSystem

它提供了类似UGUI技术栈中，重写Standalone Input Module所能获得自定义输入源的能力。

> 这一点在之前需要模拟安卓motion时没有发现，使用了不那么合适的方式模拟的安卓焦点输入。
>
> 而在最新Unity中，安卓的焦点又变得正常，并不需要模拟了。。之后若有定制输入的需求或World Space模拟输入的需求，可以利用这儿！

另外，他更建议使用`Input System`。

见源码`InputWrapper.cs`中的`Input`实现。



## Managed

定义了一些数据结构，但仅在`ScriptBindings`中被使用了。

见源码。



## ScriptBindings

preview功能，而且runtime不支持。**暂时没仔细读**，大体是靠维护了一堆委托，来使数据修改时`VisualElement`能得到刷新。

核心逻辑脚本是`VisualManager`。



## Shaders

TODO: UI Toolkit的shader是内置的，看不到具体的代码，在网上也没有相关的讨论和信息。我尝试强行阅读builtindata，才能看到一点点信息。

