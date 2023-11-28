using AssignmentProject.Core.Entities;
using AssignmentProject.Core.Repositories;
using AssignmentProject.Infrastructure.Data;
using AssignmentProject.Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentProject.Infrastructure.Repository
{
    internal sealed class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<Book>> GetBooksByNameAsync(string name)
        {
            return await _dbContext.Book
               .Where(x => x.Name.ToUpper().Contains(name.ToUpper()))
               .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName)
        {
            return await _dbContext.Book
                .Where(x => x.Author.ToUpper().Contains(authorName.ToUpper()))
                .ToListAsync();
        }
    }
}
