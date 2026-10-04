using wt_lab2_2oop_valkovich.Interfaces;

namespace wt_lab2_2oop_valkovich.Models;

public class Author : BaseEntity, IStorable
{
    public string FullName { get; set; }
    public string Country { get; set; }

    public Author(int id, string fullName, string country)
    {
        Id = id;
        FullName = fullName;
        Country = country;
    }

    public override string GetDescription() => $"Автор: {FullName} ({Country})";
    public string GetShortInfo() => FullName;
}