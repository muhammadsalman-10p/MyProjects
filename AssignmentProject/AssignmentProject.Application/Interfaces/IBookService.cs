using AssignmentProject.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentProject.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetBookList();
        Task<BookModel> GetBookById(int Id);
        Task<IEnumerable<BookModel>> GetBooksByName(string bookName);
        Task<IEnumerable<BookModel>> GetBooksByAuthor(string authorName);
        Task Create(BookModel bookModel);
        Task Update(BookModel bookModel);
        Task Delete(BookModel bookModel);
    }
}
