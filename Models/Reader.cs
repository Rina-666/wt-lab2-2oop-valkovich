using wt_lab2_2oop_valkovich.Interfaces;

namespace wt_lab2_2oop_valkovich.Models;

public class Reader : BaseEntity, IStorable
{
    public string Name { get; set; }
    public string TicketNumber { get; private set; }

    public Reader(int id, string name, string ticketNumber)
    {
        Id = id;
        Name = name;
        TicketNumber = ticketNumber;
    }

    public override string GetDescription() => $"Читатель: {Name}, билет {TicketNumber}";
    public string GetShortInfo() => $"{Name} [{TicketNumber}]";
}