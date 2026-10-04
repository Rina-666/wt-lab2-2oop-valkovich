using wt_lab2_2oop_valkovich.Interfaces;
using wt_lab2_2oop_valkovich.Models;

namespace wt_lab2_2oop_valkovich.Services;

public class LibraryService<T> : ILibraryService<T> where T : BaseEntity
{
    // protected — доступно наследникам LibraryService<T>, но не извне
    protected readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> GetAll() => _items;

    // -------- Перегрузка метода Find --------

    // Версия 1: поиск по предикату (Func<T, bool>)
    public IEnumerable<T> Find(Func<T, bool> predicate)
        => _items.Where(predicate);

    // Версия 2: поиск по ключевому слову (string)
    public IEnumerable<T> Find(string keyword)
        => _items.Where(x =>
            x.GetDescription().Contains(keyword, StringComparison.OrdinalIgnoreCase));
}