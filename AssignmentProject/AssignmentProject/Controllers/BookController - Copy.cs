using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Models;
using LoggingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILoggerManager _logger;

        public BookController(IBookService bookSerice, ILoggerManager logger)
        {
            _bookService = bookSerice;
            _logger = logger;
        }

        public IEnumerable<BookModel> BooksList { get; set; } = new List<BookModel>();

        //[BindProperty]
        //public BookModel Book { get; set; }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                //BookModel a = null;
                //var bm = a?.Author;

                BooksList = await _bookService.GetBookList();
                return Ok(BooksList);
            }
            catch (Exception ex)
            {
               // _logger.Log(LogLevel.Error, ex.ToString());
                return StatusCode(500, "Internal server error");
                throw;
            }

        }
        [HttpGet("getbook")]
        public async Task<IActionResult> GetBook(int? id)
        {
            BookModel Book = new BookModel();
            if (id > 0)//Edit
            {
                Book = await _bookService.GetBookById(id.Value);
                if (Book == null)
                {
                    return NotFound();
                }
            }
            return Ok(Book);
        }

        [HttpPost("upsert")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromBody] BookModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    await _bookService.Update(model);
                }
                else
                {
                    await _bookService.Create(model);
                }
                //return RedirectToAction("Index");

            }
            return Ok();

        }
    }
}