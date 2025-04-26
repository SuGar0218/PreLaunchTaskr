using CommunityToolkit.Mvvm.ComponentModel;

using PreLaunchTaskr.GUI.WinUI3.ViewModels.ItemModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.WinUI3.ViewModels.PageModels;

public partial class MultiTabViewModel : ObservableObject
{
    public ObservableCollection<TabStripItem> TabStripItems { get; } = [];

    [ObservableProperty]
    public partial int CurrentTabIndex { get; set; } = -1;

    [ObservableProperty]
    public partial TabStripItem? CurrentTabItem { get; set; } = null;

    public void AddTabStripItem(TabStripItem item, bool select = true)
    {
        TabStripItems.Add(item);
        if (select)
        {
            CurrentTabIndex = TabStripItems.Count - 1;
            CurrentTabItem = item;
        }
    }

    public void RemoveTabStripItem(TabStripItem item)
    {
        int index = TabStripItems.IndexOf(item);
        if (index == CurrentTabIndex)
        {
            CurrentTabIndex--;
            CurrentTabItem = CurrentTabIndex < 0 ? null : TabStripItems[CurrentTabIndex];
        }
        TabStripItems.RemoveAt(index);
    }

    /// <summary>
    /// 时间复杂度至少为 O(n)
    /// </summary>
    /// <param name="item">尝试添加的标签页项</param>
    /// <param name="areSame">判断两个标签页项相同的方法</param>
    /// <returns>是否添加成功</returns>
    public bool TryAddUniqueTabStripItem(TabStripItem item, Func<TabStripItem, TabStripItem, bool> areSame, bool select = true)
    {
        int i = 0;
        foreach (TabStripItem existed in TabStripItems)
        {
            if (areSame(item, existed))
            {
                if (select)
                {
                    CurrentTabIndex = i;
                    CurrentTabItem = existed;
                }
                return false;
            }
            i++;
        }

        AddTabStripItem(item, select);
        return true;
    }
}
