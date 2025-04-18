using PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

public interface IProgramConfigViewModel<TIcon, TProgramConfigCategoryItem>
    where TProgramConfigCategoryItem : IProgramConfigCategoryItemBase
{
    public TIcon? Icon { get; }

    public string Name { get; }

    public string Path { get; }

    public bool Enabled { get; set; }

    public bool SaveChanges();

    /// <summary>
    /// 设置的种类，例如：附加参数、屏蔽参数、环境变量
    /// </summary>
    public List<TProgramConfigCategoryItem> Categories { get; }
}
