namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;

/// <summary>
/// 程序配置的分类（参数、启动任务、环境变量）的页面的 ViewModel，放在 ProgramConfigPage 里的子页面。
/// </summary>
public interface IProgramConfigCategoryViewModel
{
    public bool SaveChanges();
}
