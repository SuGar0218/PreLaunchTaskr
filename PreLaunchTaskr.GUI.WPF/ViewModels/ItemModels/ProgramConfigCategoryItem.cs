using System.Windows.Controls;

namespace PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;

public class ProgramConfigCategoryItem : IProgramConfigCategoryItem
{
    public Page Page { get; }

    public string Name { get; }

    public ProgramConfigCategoryItem(Page page, string name)
    {
        Page = page;
        Name = name;
    }
}
