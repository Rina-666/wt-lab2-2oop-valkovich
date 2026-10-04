namespace wt_lab2_2oop_valkovich.Models;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.Now;

    public abstract string GetDescription();
}