using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_0
{
    /// <summary>
    /// Создал EDM по моделт Database First 
    /// Перенес код в данный класс
    ///
    /// === Отложенная загрузка (lazy loading) === 
    /// Доп. материалы: https://metanit.com/sharp/efcore/3.9.php
    /// Lazy loading или ленивая загрузка предполагает неявную автоматическую загрузку связанных данных при обращении к навигационному свойству. Однако здесь есть ряд условий:
    ///  - При конфигурации контекста данных вызвать метод UseLazyLoadingProxies()
    /// - Все навигационные свойства должны быть определены как виртуальные(то есть с модификатором virtual), при этом сами классы моделей должны быть открыты для наследования
    /// 1. Установим пакет EF
    /// </summary>
    public static class DbLibraryContext
    {
        public static string FillAll(int count = 10)
        {
            FillAuthors(count);
            FillPublisher(count);
            FillBooks(count);

            return $"Таблицы заполнены ({count})!";
        }

        #region === Authors ===
        public static int CountAuthors
        {
            get
            {
                int count = 0;

                using (LibraryEntities db = new LibraryEntities())
                {
                    count = db.Author.Count();
                }

                return count;
            }
        }

        public static string FillAuthors(int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                AddAuthor(new Author()
                {
                    FirstName = "Имя " + i.ToString(),
                    LastName = "Фамилия " + i.ToString(),
                });
            }

            return $"Авторы заполнены ({count})!";
        }

        public static string ClearAuthors()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Database.ExecuteSqlCommand("delete from Author");
            }

            return "Авторы очищены!";
        }

        public static string AddAuthor(Author author)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Author.Add(author);  // действие с сущностью (локально)
                db.SaveChanges();       // сохранение действия в БД
            }

            return "Автор добавлен!";
        }

        public static string AddAuthor_Log(Author author)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                // Теперь при вызове данного метода 
                // в консоле получим всю логированную
                // информацию об операции 
                //db.Database.Log = Console.Write;
                db.Database.Log = MyLogger.ConsoleLog;

                db.Author.Add(author);  // действие с сущностью (локально)
                db.SaveChanges();       // сохранение действия в БД
            }

            return "Автор добавлен!";
        }

        public static string AuthorTransaction()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                // Используем транзакцию:
                // Кроме того, Entity Framework поддерживает работу с
                // транзакциями, начатыми вне Entity Framework. Для этого
                // используется метод Database.UseTransaction().
                using (System.Data.Entity.DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Author author = new Author
                        {
                            FirstName = "FirstName Transaction",
                            LastName = "LastName Transaction"
                        };

                        // Действия для транзакции:
                        db.Author.Add(author);
                        db.Author.Remove(author);

                        db.SaveChanges();
                        dbTran.Commit(); // выполнение транзакции
                    }
                    catch (Exception ex)
                    {
                        dbTran.Rollback(); // откат транзакции 
                        MyLogger.ConsoleLog(ex.Message);
                    }
                }
            }

            return "Транзакция совершена!";
        }

        public static void ConsolePrintAuthors()
        {
            Console.WriteLine("Все авторы: ");

            using (LibraryEntities db = new LibraryEntities())
            {
                var authors = db.Author.ToList();

                foreach (var a in authors)
                {
                    Console.WriteLine(a.FirstName + " " + a.LastName);
                }
            }
        }
        #endregion

        #region === Publishers ===
        public static int CountPublishers
        {
            get
            {
                int count = 0;

                using (LibraryEntities db = new LibraryEntities())
                {
                    count = db.Publisher.Count();
                }

                return count;
            }
        }

        public static string ClearPublishers()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Database.ExecuteSqlCommand("delete from Publisher");
            }

            return "Издатели очищены!";
        }

        public static string FillPublisher(int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                AddPublisher(new Publisher()
                {
                    PublisherName = "Издатель " + i.ToString(),
                    Address = "Адрес " + i.ToString()
                });
            }

            return $"Издатели заполнены ({count})!";
        }

        public static string AddPublisher(Publisher publisher)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Publisher.Add(publisher);
                db.SaveChanges();
            }

            return "Издатель добавлен!";
        }

        public static void ConsolePrintPublisher()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = db.Book.OrderBy((x) =>
                x.Title).ToList();
                foreach (var a in au)
                {
                    Console.WriteLine("Book: " + a.Title +
                    " price: " + a.Price + " books: "
                    + a.Author.FirstName + " " + a.
                    Author.LastName);
                }
            }
        }
        #endregion

        #region === Books ===
        public static int CountBooks 
        {
            get {
                int count = 0;

                using (LibraryEntities db = new LibraryEntities())
                {
                    count = db.Book.Count();
                }

                return count;
            }
        }

        public static string FillBooks(int count = 10)
        {
            List<Book> books = new List<Book>();

            for (int i = 0; i < count; i++)
            {
                var book = new Book()
                {
                    Title = "Книга " + i.ToString(),
                    IdAuthor = i + 4,
                    Pages = 100 * (i + 1),
                    Price = 100 * (i + 1),
                    IdPublisher = i + 1,
                };

                using (LibraryEntities db = new LibraryEntities())
                {
                    var author = db.Author.First(r => r.Id == book.IdAuthor);
                    if (author != null)
                    {
                        book.Author = author;
                    }

                    var publisher = db.Publisher.FirstOrDefault(r => r.Id == book.IdPublisher);
                    if (publisher != null)
                    {
                        book.Publisher = publisher;
                    }

                    db.SaveChanges();
                }

                books.Add(book);
            }

            foreach(var b in books)
                AddBook(b);

            return $"Книги заполнены ({count})!";
        }

        public static string ClearBooks()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Database.ExecuteSqlCommand("delete from Book");
            }

            return "Книги очищены!";
        }

        public static string AddBook(Book book)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                db.Book.Add(book);  
                db.SaveChanges();
            }

            return "Книга добавлена!";
        }

        /// <summary>
        /// Отложенная загрузка - загрузка связанных компонент происходит при обращении к ним
        /// </summary>
        public static void ConsolePrintBooks_LazуLoading()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (LibraryEntities db = new LibraryEntities())
            {
                var books = db.Book.ToList();

                // В цикле при обращение к внешним компонентам
                // такие как автор и издатель, происходит их 
                // соответсвующая загрузка из БД - это и есть 
                // отложенная загрузка (lazy loading)
                // (Нагружает систему)
                foreach (var b in books)
                {
                    Console.WriteLine($"Название: {b.Title}");
                    Console.WriteLine($"Автор: {b.Author.FirstName}");
                    Console.WriteLine($"Страниц: {b.Pages}");
                    Console.WriteLine($"Стоимость: {b.Price}");
                    Console.WriteLine($"Издатель: {b.Publisher.PublisherName}");
                    Console.WriteLine($"========================");
                }
            }

            stopwatch.Stop();
            Console.WriteLine("\nВремя выполнения {0} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Безотложная загрузка - загрузка связанных данных основывается на прямом указании в запросе тех связанных данных, которые необходимо получить.
        /// </summary>
        public static void ConsolePrintBooks_EagerLoading()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (LibraryEntities db = new LibraryEntities())
            {
                // Применение безотложной загрузки базируется на использовании метода Include()
                // Именно этот метод позволяет в запросе для одной сущности указат другой, связанной сущности

                // Метод расширения LINQ
                var books = db.Book.Include("Author").Include("Publisher").ToList<Book>();

                // Синтаксис запроса LINQ
                // var books = (from s in db.Book.
                // Include("Author") select s).ToList<Book>();                     

                // При безотложной загрузки получается снизить количество обращений к БД 
                foreach (var b in books)
                {
                    Console.WriteLine($"Название: {b.Title}");
                    Console.WriteLine($"Автор: {b.Author.FirstName}");
                    Console.WriteLine($"Страниц: {b.Pages}");
                    Console.WriteLine($"Стоимость: {b.Price}");
                    Console.WriteLine($"Издатель: {b.Publisher.PublisherName}");
                    Console.WriteLine($"========================");
                }
            }

            stopwatch.Stop();
            Console.WriteLine("\nВремя выполнения {0} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Явная загрузка - при данной загрузке необходимо загружать связанные данные непосредственным вызовом специальных методов
        /// Плюсы явной загрузки:
        /// * навигационное свойство (данные) не обязаны быть virtual (для наслдования)
        /// * контроль создаваемых запросов 
        /// Основные методы: Entry(), Reference(), Collection() и Load()
        /// Для получения одиночного свойства навигации, необходимо использовать методы Entry() и Reference()
        /// </summary>
        public static void ConsolePrintBook_ExplicitLoading()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (LibraryEntities db = new LibraryEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                var books = db.Book.ToList();

                foreach(var b in books)
                {
                    // Сами контролируем что и когда загружать
                    // Reference - одни данные
                    db.Entry(b).Reference(a => a.Author).Load();
                    db.Entry(b).Reference(a => a.Publisher).Load();

                    // Collection - много / колекция / набор 
                    //db.Entry(author).Collection("Book").Load();

                    Console.WriteLine($"Название: {b.Title}");
                    Console.WriteLine($"Автор: {b.Author.FirstName}");
                    Console.WriteLine($"Страниц: {b.Pages}");
                    Console.WriteLine($"Стоимость: {b.Price}");
                    Console.WriteLine($"Издатель: {b.Publisher.PublisherName}");
                    Console.WriteLine($"========================");
                }               
            }

            stopwatch.Stop();
            Console.WriteLine("\nВремя выполнения {0} ms", stopwatch.ElapsedMilliseconds);
        }
        #endregion

        #region === LINQ to Entities Examples ===
        // Доп. материал: https://metanit.com/sharp/entityframework/4.1.php
        public static Author GetAuthorByName_ByQuerySyntaxLINQ(string fname)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                // В формате: синтаксис LINQ запроса 
                var author = (from s in db.Author
                              where s.FirstName == fname
                              select s).FirstOrDefault<Author>();
                return author;
            }
        }

        public static Author GetAuthorByName_ByExtensionMthodLINQ(string fname)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                // В формате: метод расширения LINQ запроса 
                var author = db.Author.Where(x =>
                x.FirstName == fname).FirstOrDefault();

                return author;
            }
        }

        // Single() и SingleOrDefault() - вернут единственную запись
        public static Author GetAuthorById_ByQuerySyntaxLINQ(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var author = (from s in db.Author
                              where s.Id == id
                              select s).Single();

                return author;
            }
        }

        public static Author GetAuthorById_ByExtensionMthodLINQ(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var author = db.Author.Where((x) =>
                x.Id == id).SingleOrDefault();
                return author;
            }
        }

        // ToList() - формирует список, например для формирования списка определенного отбора
        public static void GetAllAuthors_ByQuerySyntaxLINQ()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = (from a in db.Author
                          where a.LastName.StartsWith("A")
                          select a).ToList();

                foreach (var a in au)
                {
                    Console.WriteLine(a.FirstName + " " + a.
                    LastName);
                }
            }
        }

        public static void GetAllAuthors_ByExtensionMthodLINQ()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = db.Author.Where((x) =>
                x.LastName.StartsWith("A")).ToList();
                foreach (var a in au)
                {
                    Console.WriteLine(a.FirstName + " "
                    + a.LastName);
                }
            }
        }

        // OrderBy() - сортировка
        public static void GetAllAuthorsOrderBy_ByQuerySyntaxLINQ()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = (from a in db.Author
                          orderby
                a.LastName ascending
                          select a).ToList();
                foreach (var a in au)
                {
                    Console.WriteLine(a.FirstName + " " + a.LastName);
                }
            }
        }

        public static void GetAllAuthorsOrderBy_ByExtensionMthodLINQ()
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = db.Author.OrderBy((x) =>
                x.LastName).ToList();
                foreach (var a in au)
                {
                    Console.WriteLine(a.FirstName + "  " + a.LastName);
                }
            }
        }

        // Find() - поиск объекта
        public static Author GetAuthorFindById_ByExtensionMthodLINQ(int id)
        {
            using (LibraryEntities db = new LibraryEntities())
            {
                var au = db.Author.Find(id);
                Console.WriteLine(au.FirstName + " " +
                au.LastName);
                return au;
            }
        }
        #endregion
    }
}
