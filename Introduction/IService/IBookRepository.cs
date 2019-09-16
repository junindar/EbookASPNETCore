using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Introduction.Models;

namespace Introduction.IService
{
    public interface IBookRepository
    {
        void Insert(Book book);
        void Update(Book book);
        void Delete(int bookId);
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int bookId);

    }
}
