using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IProgramListItem<TImage>
{
    public int Id { get; }
    public string Name { get; }
    public string Path { get; }
    public TImage? Icon { get; }
    public bool Enabled { get; set; }
}
