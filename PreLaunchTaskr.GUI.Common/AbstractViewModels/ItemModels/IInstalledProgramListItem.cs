namespace PreLaunchTaskr.GUI.Common.AbstractViewModels.ItemModels;

public interface IInstalledProgramListItem<TIcon>
{
    public string? Name { get; }

    public TIcon? Icon { get; }

    public string? Version { get; }

    public string? Publisher { get; }

    public string? PossiblePath { get; }
}
