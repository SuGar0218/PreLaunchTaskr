// WinUI3

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Media.Imaging;

using PreLaunchTaskr.GUI.Common.AbstractViewModels.PageModels;
using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;
using PreLaunchTaskr.GUI.WinUI3.Views;

using System.Collections.Generic;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class ProgramConfigViewModel : ObservableObject, IProgramConfigViewModel<BitmapImage, ProgramConfigCategoryItem>
{
    public ProgramConfigViewModel(ProgramListItem programListItem)
    {
        ProgramListItem = programListItem;

        attachArgumentViewModel = new AttachArgumentViewModel(programListItem);
        blockArgumentViewModel = new BlockArgumentViewModel(programListItem);
        preLaunchTaskViewModel = new PreLaunchTaskViewModel(programListItem);
        environmentVariableViewModel = new EnvironmentVariableViewModel(programListItem);

        categoryViewModels = [attachArgumentViewModel, blockArgumentViewModel, preLaunchTaskViewModel, environmentVariableViewModel];

        Categories = [
            new ProgramConfigCategoryItem(typeof(AttachArgumentPage), "附加参数", attachArgumentViewModel),
            new ProgramConfigCategoryItem(typeof(BlockArgumentPage), "屏蔽参数", blockArgumentViewModel),
            new ProgramConfigCategoryItem(typeof(PreLaunchTaskPage), "启动前任务", preLaunchTaskViewModel),
            new ProgramConfigCategoryItem(typeof(EnvironmentVariablePage), "专属环境变量", environmentVariableViewModel)];
    }

    public BitmapImage? Icon => ProgramListItem.Icon;

    public string Name => ProgramListItem.Name;

    public string Path => ProgramListItem.Path;

    public bool Enabled
    {
        get => ProgramListItem.Enabled;
        set
        {
            if (ProgramListItem.Enabled == value)
                return;

            ProgramListItem.Enabled = value;
            OnPropertyChanged(nameof(Enabled));
        }
    }

    public bool SaveChanges()
    {
        for (int i = 0; i < categoryViewModels.Length; i++)
        {
            categoryViewModels[i].SaveChanges();
        }
        return true;
    }

    public List<ProgramConfigCategoryItem> Categories { get; init; }

    public ProgramListItem ProgramListItem { get; init; }

    private readonly AttachArgumentViewModel attachArgumentViewModel;
    private readonly BlockArgumentViewModel blockArgumentViewModel;
    private readonly PreLaunchTaskViewModel preLaunchTaskViewModel;
    private readonly EnvironmentVariableViewModel environmentVariableViewModel;

    private readonly IProgramConfigCategoryViewModel[] categoryViewModels;
}
