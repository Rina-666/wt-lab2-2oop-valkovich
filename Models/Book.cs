using wt_lab2_2oop_valkovich.Interfaces;

namespace wt_lab2_2oop_valkovich.Models;

public class Book : BaseEntity, IStorable
{
    public string Title { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        private set => _price = value;
    }

    public Book(int id, string title, string genre, int year, decimal price)
    {
        Id = id;
        Title = title;
        Genre = genre;
        Year = year;
        UpdatePrice(price);   // ← не Price = price, а через метод с валидацией
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        _price = newPrice;
    }

    public override string GetDescription() => $"Книга «{Title}» ({Year})";
    public string GetShortInfo() => Title;

    public void PrintInfo()
    {
        Console.WriteLine($"#{Id} | {Title} | {Genre} | {Year} | {Price:C}");
    }
}