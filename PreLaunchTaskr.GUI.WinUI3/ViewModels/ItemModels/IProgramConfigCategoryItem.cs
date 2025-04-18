using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;
using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

using System;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

public interface IProgramConfigCategoryItem : IProgramConfigCategoryItemBase
{
    public Type PageType { get; }

    public IProgramConfigCategoryViewModel PageViewModel { get; }
}
