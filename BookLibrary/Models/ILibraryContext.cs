using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public interface ILibraryContext
    {
        /// <summary>
        /// получаем все документы, используя критерии фальтрации
        /// </summary>
        /// <param name="country"></param>
        /// <param name="name"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<WriterPagination> GetWriters(string country, string name, int page);

        /// <summary>
        /// Get list of Books
        /// </summary>
        /// <param name="writerId"></param>
        /// <param name="genre"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        Task<IEnumerable<Book>> GetBooks(string writerId, string genre, string title);

        Task<Writer> GetWriter(string id);

        Task<Book> GetBook(string writerId, int bookId);

        Task Create(Writer w);

        Task Update(Writer w);

        Task Remove(string id);

        Task CreateBook(Book b);

        // обновление документа
        Task UpdateBook(Book b);

        Task RemoveBook(string writerId, int bookId);

        Task<IEnumerable<Report>> Report(int numReport, int year);
    }
}
