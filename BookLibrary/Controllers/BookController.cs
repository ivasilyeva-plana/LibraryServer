using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Models;
using MongoDB.Bson;

namespace BookLibrary.Controllers
{
    [Produces("application/json")]
    [Route("api/Book")]
    public class BookController : Controller
    {

        private readonly LibraryContext db;
        public BookController(LibraryContext context)
        {
            db = context;
        }

        [HttpGet]
        public Task<IEnumerable<Book>> Get(string writerId, string genre, string title)
        {
            return db.GetBooks(writerId, genre, title);
        }

        // GET: api/Book/8
        [HttpGet("{id}")]
        public Task<Book> Get(string id)
        {
            var arrParams = id.Split('&');
            var bookId = Convert.ToInt32(arrParams[1]);
            return db.GetBook(arrParams[0], bookId);
        }

        /*
        // GET: api/Book/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        */
        // POST: api/Book
        [HttpPost]
        public async Task Post([FromBody]Book book)
        {
            await db.CreateBook(book);
        }

        // PUT: api/Book/5
        [HttpPut]
        public async Task Put([FromBody]Book book)
        {
            await db.UpdateBook(book);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var arrParams = id.Split('&');
            var bookId = Convert.ToInt32(arrParams[1]);
            await db.RemoveBook(arrParams[0], bookId);
        }

        [HttpOptions]
        public void Options()
        {

        }

        [HttpOptions("{id}")]
        public void Options(int id)
        {

        }
    }
}
