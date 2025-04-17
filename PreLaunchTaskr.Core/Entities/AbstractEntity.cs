namespace PreLaunchTaskr.Core.Entities;

public abstract class AbstractEntity
{
    protected AbstractEntity(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}
