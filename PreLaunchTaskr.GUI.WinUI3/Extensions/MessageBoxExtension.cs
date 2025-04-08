using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System;
using System.Threading.Tasks;

using Windows.UI;

namespace PreLaunchTaskr.GUI.WinUI3.Extensions;

public static class MessageBoxExtension
{
    public static async Task MessageBox(this Window parent, object content, string? title = null) => await MessageBox(parent.Content.XamlRoot, content, title, MessageBoxButtons.OK);
    public static async Task MessageBox(this UIElement parent, object content, string? title = null) => await MessageBox(parent.XamlRoot, content, title, MessageBoxButtons.OK);

    public static async Task<DialogResult> MessageBox(
        this Window parent,
        object content, string? title,
        MessageBoxButtons buttons,
        MessageBoxDefaultButton? defaultButton = MessageBoxDefaultButton.Button1)
        => await MessageBox(parent.Content.XamlRoot, content, title, buttons, defaultButton);

    public static async Task<DialogResult> MessageBox(
        this UIElement parent,
        object content, string? title,
        MessageBoxButtons buttons,
        MessageBoxDefaultButton? defaultButton = MessageBoxDefaultButton.Button1)
        => await MessageBox(parent.XamlRoot, content, title, buttons, defaultButton);

    public static async Task<DialogResult> MessageBox(
        XamlRoot xamlRoot,
        object content, string? title,
        MessageBoxButtons buttons,
        MessageBoxDefaultButton? defaultButton = MessageBoxDefaultButton.Button1)
    {

        ContentDialog dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            XamlRoot = xamlRoot,
            //Style = new Style
            //{
            //    TargetType = typeof(ContentDialog),
            //    BasedOn = Application.Current.Resources["DefaultContentDialogStyle"] as Style,  // 没有这个出现时没动画
            //    Setters = {
            //        new Setter {
            //            Property = ContentDialog.BackgroundProperty,
            //            Value = new AcrylicBrush {
            //                TintColor = (Color) Application.Current.Resources["ControlOnImageFillColorDefault"],
            //                TintOpacity = 0.618
            //            }
            //        }
            //    }
            //}
        };
        //dialog.Resources["ContentDialogTopOverlay"] = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        ContentDialogButton contentDialogDefaultButton = defaultButton switch
        {
            MessageBoxDefaultButton.Button1 => ContentDialogButton.Primary,
            MessageBoxDefaultButton.Button2 => ContentDialogButton.Secondary,
            MessageBoxDefaultButton.Button3 => ContentDialogButton.Close,
            null => ContentDialogButton.None,
            _ => throw new ArgumentException("MessageBoxDefaultButton defaultButton should be in {Button1=0, Button2=256, Button3=512}")
        };
        dialog.DefaultButton = contentDialogDefaultButton;

        switch (buttons)
        {
            case MessageBoxButtons.OK:
                dialog.PrimaryButtonText = "OK";
                break;

            case MessageBoxButtons.OKCancel:
                dialog.PrimaryButtonText = "OK";
                dialog.SecondaryButtonText = "Cancel";
                break;

            case MessageBoxButtons.YesNo:
                dialog.PrimaryButtonText = "Yes";
                dialog.SecondaryButtonText = "No";
                //dialog.PrimaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "Y" }
                //    }
                //};
                //dialog.SecondaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "N" }
                //    }
                //};
                break;

            case MessageBoxButtons.YesNoCancel:
                dialog.PrimaryButtonText = "Yes";
                dialog.SecondaryButtonText = "No";
                dialog.CloseButtonText = "Cancel";
                //dialog.PrimaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "Y" }
                //    }
                //};
                //dialog.SecondaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "N" }
                //    }
                //};
                break;

            case MessageBoxButtons.AbortRetryIgnore:
                dialog.PrimaryButtonText = "Abort";
                dialog.SecondaryButtonText = "Retry";
                dialog.CloseButtonText = "Ignore";
                //dialog.PrimaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "A" }
                //    }
                //};
                //dialog.SecondaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "R" }
                //    }
                //};
                //dialog.CloseButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "I" }
                //    }
                //};
                break;

            case MessageBoxButtons.RetryCancel:
                dialog.PrimaryButtonText = "Retry";
                dialog.SecondaryButtonText = "Cancel";
                //dialog.PrimaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "R" }
                //    }
                //};
                break;

            case MessageBoxButtons.CancelTryContinue:
                dialog.PrimaryButtonText = "Continue";
                dialog.SecondaryButtonText = "Try again";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Close;
                //dialog.PrimaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "C" }
                //    }
                //};
                //dialog.SecondaryButtonStyle = new() {
                //    TargetType = typeof(Button),
                //    Setters = {
                //        new Setter { Property = Button.AccessKeyProperty, Value = "T" }
                //    }
                //};
                break;

        }

        ContentDialogResult result = await dialog.ShowAsync();
        // None    0
        // 未点击任何按钮 或 CloseButton (ESC)

        // Primary 1
        // 用户已点击主按钮

        // Secondary   2
        // 用户点击了辅助按钮
        var results = DialogResultsOf(buttons);
        return results[result switch
        {
            ContentDialogResult.Primary => 0,
            ContentDialogResult.Secondary => 1,
            ContentDialogResult.None => results.Length - 1,
            _ => throw new ArgumentException()
        }];
    }

    private static readonly DialogResult[][] resultGroups = [
        [DialogResult.OK],
        [DialogResult.OK, DialogResult.Cancel],
        [DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore],
        [DialogResult.Yes, DialogResult.No, DialogResult.Cancel],
        [DialogResult.Yes, DialogResult.No],
        [DialogResult.Retry, DialogResult.Cancel],
        [DialogResult.Continue, DialogResult.TryAgain, DialogResult.Cancel]
    ];

    private static DialogResult[] DialogResultsOf(MessageBoxButtons buttons) => resultGroups[(int) buttons];

    private static readonly Style defaultStyle = new()
    {
        TargetType = typeof(ContentDialog),
        BasedOn = Application.Current.Resources["DefaultContentDialogStyle"] as Style
    };
}

public enum MessageBoxButtons
{
    AbortRetryIgnore = 2,
    // 消息框包含“中止”、“重试”和“忽略”按钮。

    CancelTryContinue = 6,
    // 指定消息框包含“取消”、“重试”和“继续”按钮。

    OK = 0,
    // 消息框包含“确定”按钮。

    OKCancel = 1,
    // 消息框包含“确定”和“取消”按钮。

    RetryCancel = 5,
    // 消息框包含“重试”和“取消”按钮。

    YesNo = 4,
    // 消息框包含“是”和“否”按钮。

    YesNoCancel = 3,
    // 消息框包含“是”、“否”和“取消”按钮。
}

public enum DialogResult
{
    Abort = 3,
    // 对话框的返回值是 Abort（通常从标签为“中止”的按钮发送）。

    Cancel = 2,
    // 对话框的返回值是 Cancel（通常从标签为“取消”的按钮发送）。

    Continue = 11,
    // 对话框返回值是“继续” (通常从标记为“继续”) 的按钮发送。

    Ignore = 5,
    // 对话框的返回值是 Ignore（通常从标签为“忽略”的按钮发送）。

    No = 7,
    // 对话框的返回值是 No（通常从标签为“否”的按钮发送）。

    None = 0,
    // 从对话框返回了 Nothing。 这表明有模式对话框继续运行。

    OK = 1,
    // 对话框的返回值是 OK（通常从标签为“确定”的按钮发送）。

    Retry = 4,
    // 对话框的返回值是 Retry（通常从标签为“重试”的按钮发送）。

    TryAgain = 10,
    // 对话框返回值是“重试” (通常从标记为“重试”的按钮发送) 。

    Yes = 6,
    // 对话框的返回值是 Yes（通常从标签为“是”的按钮发送）
}

public enum MessageBoxDefaultButton
{
    Button1 = 0,
    // 消息框上的第一个按钮是默认按钮。

    Button2 = 256,
    // 消息框上的第二个按钮是默认按钮。

    Button3 = 512,
    // 消息框上的第三个按钮是默认按钮。
}

