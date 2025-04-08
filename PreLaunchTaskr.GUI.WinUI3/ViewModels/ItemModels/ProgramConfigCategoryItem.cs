using PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

/// <summary>
/// 已添加的程序的设置页的分类 NavigationView 的每一项的 ViewModel
/// </summary>
public class ProgramConfigCategoryItem
{
    public Type PageType { get; }
    public string Name { get; }
    public IProgramConfigCategoryViewModel? ViewModel { get; }

    /// <summary>
    /// viewModel 在构造时可以为 null，这是为了延迟加载。
    /// </summary>
    public ProgramConfigCategoryItem(Type pageType, string name, IProgramConfigCategoryViewModel? viewModel)
    {
        PageType = pageType;
        Name = name;
        ViewModel = viewModel;
    }
}
