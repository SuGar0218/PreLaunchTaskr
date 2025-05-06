using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;

using WinRT;

namespace PreLaunchTaskr.GUI.WinUI3.Helpers;

/// <summary>
/// 控制 MainWindow 的窗口材质，包括 SystemBackdrop 和背景色。
/// <br/>
/// ActiveBackground 属性是我自己加的，Window 类没有。
/// </summary>
public class MainWindowMaterialController
{
    public MainWindowMaterialController(MainWindow window)
    {
        this.window = window;
    }

    public void SetBackground(Brush brush)
    {
        window.ActiveBackground = brush;
    }

    public void SetBackdrop(SystemBackdrop backdrop)
    {
        window.SystemBackdrop = backdrop;
    }

    public void SetCustomBackdrop(ISystemBackdropControllerWithTargets controller, bool fallback)
    {
        window.Activated -= OnWindowActivated;
        systemBackdropConfiguration ??= new();
        systemBackdropController?.Dispose();
        systemBackdropController = controller;
        systemBackdropController.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
        systemBackdropController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
        if (fallback)
        {
            window.Activated += OnWindowActivated;
        }
    }

    private void OnWindowActivated(object _, WindowActivatedEventArgs args)
    {
        if (systemBackdropConfiguration is not null)
        {
            systemBackdropConfiguration.IsInputActive = (args.WindowActivationState != WindowActivationState.Deactivated);
        }
    }

    private readonly MainWindow window;

    private ISystemBackdropControllerWithTargets? systemBackdropController;
    private SystemBackdropConfiguration? systemBackdropConfiguration;
}
