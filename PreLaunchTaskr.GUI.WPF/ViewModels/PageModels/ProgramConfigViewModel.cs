// WPF

using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WPF.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WPF.Views;

using System.Windows.Media.Imaging;

namespace PreLaunchTaskr.GUI.WPF.ViewModels.PageModels;

public class ProgramConfigViewModel : IProgramConfigViewModel<BitmapImage, ProgramConfigCategoryItem>
{
    public ProgramConfigViewModel(ProgramListItem programListItem)
    {
        this.ProgramListItem = programListItem;

        attachArgumentViewModel = new AttachArgumentViewModel(programListItem);
        blockArgumentViewModel = new BlockArgumentViewModel(programListItem);
        preLaunchTaskViewModel = new PreLaunchTaskViewModel(programListItem);
        environmentVariableViewModel = new EnvironmentVariableViewModel(programListItem);

        categoryViewModels = new IProgramConfigCategoryViewModel[] { attachArgumentViewModel, blockArgumentViewModel, preLaunchTaskViewModel, environmentVariableViewModel };

        Categories = new List<ProgramConfigCategoryItem> {
            new(new AttachArgumentPage(attachArgumentViewModel), "附加参数"),
            new(new BlockArgumentPage(blockArgumentViewModel), "屏蔽参数"),
            new(new PreLaunchTaskPage(preLaunchTaskViewModel), "启动前任务"),
            new(new EnvironmentVariablePage(environmentVariableViewModel), "专属环境变量") };
    }

    public BitmapImage? Icon => ProgramListItem.Icon;

    public string Name => ProgramListItem.Name;

    public string Path => ProgramListItem.Path;

    public bool Enabled
    {
        get => ProgramListItem.Enabled;
        set => throw new NotImplementedException();
    }

    public List<ProgramConfigCategoryItem> Categories { get; }

    public bool SaveChanges()
    {
        for (int i = 0; i < categoryViewModels.Length; i++)
        {
            categoryViewModels[i].SaveChanges();
        }
        return true;
    }

    public ProgramListItem ProgramListItem { get; }

    private readonly AttachArgumentViewModel attachArgumentViewModel;
    private readonly BlockArgumentViewModel blockArgumentViewModel;
    private readonly PreLaunchTaskViewModel preLaunchTaskViewModel;
    private readonly EnvironmentVariableViewModel environmentVariableViewModel;

    private readonly IProgramConfigCategoryViewModel[] categoryViewModels;
}
