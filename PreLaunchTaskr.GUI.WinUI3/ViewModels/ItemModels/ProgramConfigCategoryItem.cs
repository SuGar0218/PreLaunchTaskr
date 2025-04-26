using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

using System;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

/// <summary>
/// 已添加的程序的设置页的分类 NavigationView 的每一项的 ExtraData
/// </summary>
public class ProgramConfigCategoryItem : IProgramConfigCategoryItem
{
    public Type PageType { get; }

    public string Name { get; }

    public IProgramConfigCategoryViewModel? PageViewModel { get; }

    /// <summary>
    /// pageViewModel 在构造时可以为 null，这是为了延迟加载。
    /// </summary>
    public ProgramConfigCategoryItem(Type pageType, string name, IProgramConfigCategoryViewModel? pageViewModel)
    {
        PageType = pageType;
        Name = name;
        PageViewModel = pageViewModel;
    }
}
