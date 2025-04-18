namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IProgramConfigCategoryItemBase
{

    public string Name { get; }

    // 无法确定实际应用的 UI 框架是基于什么导航，Type? Page? ViewModel?
    //public Type PageType { get; }
    //public IProgramConfigCategoryViewModel? PageViewModel { get; }
}
