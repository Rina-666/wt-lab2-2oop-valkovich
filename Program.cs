using wt_lab2_2oop_valkovich.Interfaces;
using wt_lab2_2oop_valkovich.Models;
using wt_lab2_2oop_valkovich.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("========================================");
Console.WriteLine("  Библиотечная система — Лабораторная №2");
Console.WriteLine("========================================");

// ===== Данные — доступны всем пунктам меню =====
var books = new List<Book>
{
    new Book(1, "Война и мир", "Классика", 1869, 1500m),
    new Book(2, "1984", "Антиутопия", 1949, 900m),
    new Book(3, "Преступление и наказание", "Классика", 1866, 1200m),
    new Book(4, "Метро 2033", "Постапокалипсис", 2005, 700m),
    new Book(5, "Хоббит", "Фэнтези", 1937, 850m),
    new Book(6, "Гарри Поттер", "Фэнтези", 1997, 1100m),
    new Book(7, "Дюна", "Фантастика", 1965, 1300m)
};

// Словарь для быстрого поиска — строится один раз и обновляется при добавлении
var bookDict = books.ToDictionary(b => b.Id);

// ===== Меню =====
bool exit = false;
while (!exit)
{
    Console.WriteLine("\nМеню:");
    Console.WriteLine("1. Показать все книги");
    Console.WriteLine("2. Найти книгу по Id");
    Console.WriteLine("3. Добавить книгу");
    Console.WriteLine("4. Статистика (LINQ)");
    Console.WriteLine("5. Асинхронная загрузка книг");
    Console.WriteLine("6. Демонстрация полиморфизма");
    Console.WriteLine("7. Демонстрация сервиса и protected");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");

    switch (Console.ReadLine())
    {
        // ===== Пункт 1: список книг =====
        case "1":
        {
            Console.WriteLine("\n--- Все книги ---");
            foreach (var b in books)
                b.PrintInfo();
            break;
        }

        // ===== Пункт 2: поиск через Dictionary + TryGetValue =====
        case "2":
        {
            Console.Write("Введите Id: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (bookDict.TryGetValue(id, out var found))
                    found.PrintInfo();
                else
                    Console.WriteLine("Книга с таким Id не найдена.");
            }
            else
            {
                Console.WriteLine("Некорректный Id.");
            }
            break;
        }

        // ===== Пункт 3: добавление книги (заглушка) =====
        case "3":
        {
            Console.WriteLine("Добавление книги — будет реализовано позже.");
            break;
        }

        // ===== Пункт 4: LINQ-статистика =====
        case "4":
        {
            Console.WriteLine("\n===== СТАТИСТИКА (LINQ) =====");

            // Where
            Console.WriteLine("\n[Where] Жанр = Классика:");
            foreach (var b in books.Where(b => b.Genre == "Классика"))
                b.PrintInfo();

            // OrderByDescending
            Console.WriteLine("\n[OrderByDescending] По году издания (новые первыми):");
            foreach (var b in books.OrderByDescending(b => b.Year))
                b.PrintInfo();

            // Select
            Console.WriteLine("\n[Select] Только названия и цены:");
            foreach (var item in books.Select(b => new { b.Title, b.Price }))
                Console.WriteLine($"{item.Title} — {item.Price:C}");

            // Where + OrderBy + Select
            Console.WriteLine("\n[Where + OrderBy + Select] Фэнтези и классика, сортировка по цене:");
            var combo = books
                .Where(b => b.Genre == "Фэнтези" || b.Genre == "Классика")
                .OrderBy(b => b.Price)
                .Select(b => new { b.Title, b.Genre, b.Price });
            foreach (var item in combo)
                Console.WriteLine($"{item.Title} ({item.Genre}) — {item.Price:C}");

            // Агрегаты
            Console.WriteLine("\n[Агрегаты]");
            Console.WriteLine($"Count: {books.Count()}");
            Console.WriteLine($"Sum: {books.Sum(b => b.Price):C}");
            Console.WriteLine($"Average: {books.Average(b => b.Price):C}");
            Console.WriteLine($"Min: {books.Min(b => b.Price):C}");
            Console.WriteLine($"Max: {books.Max(b => b.Price):C}");

            // GroupBy
            Console.WriteLine("\n[GroupBy] Группировка по жанрам:");
            foreach (var group in books.GroupBy(b => b.Genre))
            {
                Console.WriteLine($"\n  Жанр: {group.Key}");
                Console.WriteLine($"    Книг: {group.Count()}");
                Console.WriteLine($"    Средняя цена: {group.Average(b => b.Price):C}");
                Console.WriteLine($"    Сумма цен: {group.Sum(b => b.Price):C}");
                Console.WriteLine($"    Диапазон лет: {group.Min(b => b.Year)}–{group.Max(b => b.Year)}");
                foreach (var b in group)
                    Console.WriteLine($"      - {b.Title} ({b.Year})");
            }
            break;
        }

        // ===== Пункт 5: async (Уровни 1, 2, 3) =====
        case "5":
        {
            Console.WriteLine("\n--- Асинхронная загрузка ---");

            // Уровень 1
            await SimulateDelayAsync();

            // Уровень 2
            Console.WriteLine("\n--- Получение книг из API ---");
            var apiBooks = await LoadBooksFromApiAsync();

            Console.WriteLine("\nЗагруженные книги:");
            foreach (var b in apiBooks)
                b.PrintInfo();

            foreach (var b in apiBooks)
            {
                books.Add(b);
                bookDict[b.Id] = b;    // обновляем словарь
            }
            Console.WriteLine($"\nВсего книг в каталоге теперь: {books.Count}");

            // Уровень 3
            Console.WriteLine("\n--- Параллельная загрузка по жанрам ---");
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            try
            {
                var tasks = new[]
                {
                    LoadGenreAsync("Фэнтези", cts.Token),
                    LoadGenreAsync("Фантастика", cts.Token),
                    LoadGenreAsync("IT", cts.Token)
                };

                var results = await Task.WhenAll(tasks);

                Console.WriteLine("\nРезультаты параллельной загрузки:");
                foreach (var list in results)
                    foreach (var b in list)
                        b.PrintInfo();

                foreach (var list in results)
                    foreach (var b in list)
                    {
                        books.Add(b);
                        bookDict[b.Id] = b;
                    }

                Console.WriteLine($"\nВсего книг в каталоге теперь: {books.Count}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  [async] Операция отменена по таймауту.");
            }
            break;
        }

        // ===== Пункт 6: полиморфизм =====
        case "6":
        {
            Console.WriteLine("\n--- Демонстрация полиморфизма ---");
            BaseEntity[] entities =
            {
                new Book(1, "Война и мир", "Классика", 1869, 1500m),
                new Author(10, "Лев Толстой", "Россия"),
                new Reader(20, "Пётр Валькович", "AB-001")
            };
            foreach (var e in entities)
                Console.WriteLine(e.GetDescription());
            break;
        }

        // ===== Пункт 7: сервис и protected =====
        case "7":
        {
            Console.WriteLine("\n--- Демонстрация сервиса ---");

            LibraryService<Book> concrete = new LibraryService<Book>();
            ILibraryService<Book> bookService = concrete;

            bookService.Add(new Book(1, "Война и мир", "Классика", 1869, 1500m));
            bookService.Add(new Book(2, "1984", "Антиутопия", 1949, 900m));
            bookService.Add(new Book(3, "Хоббит", "Фэнтези", 1937, 850m));
            bookService.Add(new Book(4, "Дюна", "Фантастика", 1965, 1300m));

            Console.WriteLine("\nВсе книги (через интерфейсную ссылку):");
            foreach (var b in bookService.GetAll())
                b.PrintInfo();

            Console.WriteLine("\nFind(b => b.Year > 1950):");
            foreach (var b in concrete.Find(b => b.Year > 1950))
                b.PrintInfo();

            Console.WriteLine("\nFind(\"Война\"):");
            foreach (var b in concrete.Find("Война"))
                b.PrintInfo();

            Console.WriteLine("\nGetById(2):");
            bookService.GetById(2)?.PrintInfo();

            Console.WriteLine("\n--- Демонстрация protected ---");
            var extended = new ExtendedLibraryService<Book>();
            extended.Add(new Book(1, "Книга А", "Жанр", 2000, 500m));
            extended.Add(new Book(2, "Книга Б", "Жанр", 2001, 600m));
            extended.Add(new Book(3, "Книга В", "Жанр", 2002, 700m));

            Console.WriteLine($"Всего элементов: {extended.CountItems()}");
            Console.WriteLine($"Последний элемент: {extended.GetLast()?.Title}");
            break;
        }

        // ===== Пункт 0: выход =====
        case "0":
            exit = true;
            Console.WriteLine("Выход. До свидания!");
            break;

        default:
            Console.WriteLine("Неизвестная команда.");
            break;
    }
}

// ===== Вспомогательные методы =====

static async Task SimulateDelayAsync()
{
    Console.WriteLine("  [async] Начало загрузки каталога...");
    await Task.Delay(1000);
    Console.WriteLine("  [async] Каталог загружен.");
}

static async Task<List<Book>> LoadBooksFromApiAsync()
{
    var result = new List<Book>();
    try
    {
        Console.WriteLine("  [async] Запрос к API...");
        await Task.Delay(1500);

        result.Add(new Book(100, "Чистый код", "IT", 2008, 2000m));
        result.Add(new Book(101, "CLR via C#", "IT", 2012, 2500m));
        result.Add(new Book(102, "Алгоритмы", "IT", 2011, 3200m));

        Console.WriteLine($"  [async] Получено книг: {result.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  [async] Ошибка загрузки: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("  [async] Операция загрузки завершена.");
    }

    return result;
}

static async Task<List<Book>> LoadGenreAsync(string genre, CancellationToken token)
{
    Console.WriteLine($"  [async] Начало загрузки жанра «{genre}»...");
    await Task.Delay(2000, token);

    var books = new List<Book>
    {
        new Book(Random.Shared.Next(1000, 9999), $"{genre} #1", genre, 2020, 500m),
        new Book(Random.Shared.Next(1000, 9999), $"{genre} #2", genre, 2021, 600m)
    };

    Console.WriteLine($"  [async] Жанр «{genre}» загружен ({books.Count} книг)");
    return books;
}