using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;

public interface IProgramConfigCategoryItem : IProgramConfigCategoryItemBase
{
    public Page Page { get; }
}
