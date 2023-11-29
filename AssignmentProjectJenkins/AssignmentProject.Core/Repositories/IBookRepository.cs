using AssignmentProject.Core.Entities;
using AssignmentProject.Core.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentProject.Core.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByNameAsync(string productName);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName);
    }
}
