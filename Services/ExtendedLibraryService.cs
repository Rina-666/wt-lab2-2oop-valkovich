using wt_lab2_2oop_valkovich.Models;

namespace wt_lab2_2oop_valkovich.Services;

// Наследник LibraryService<T> — может обращаться к protected _items
public class ExtendedLibraryService<T> : LibraryService<T> where T : BaseEntity
{
    // Метод использует protected-поле _items напрямую — это возможно только в наследнике
    public int CountItems() => _items.Count;

    public T? GetLast()
    {
        if (_items.Count == 0) return null;
        return _items[^1];   // ^1 = последний элемент
    }
}