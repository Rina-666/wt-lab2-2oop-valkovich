using wt_lab2_2oop_valkovich.Models;

namespace wt_lab2_2oop_valkovich.Interfaces;

public interface ILibraryService<T> where T : BaseEntity
{
    void Add(T item);
    T? GetById(int id);
    IEnumerable<T> GetAll();
}