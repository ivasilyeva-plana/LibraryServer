﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Models;
using BookLibrary.Response;

namespace BookLibrary.Controllers
{
    [Produces("application/json")]
    [Route("api/Writer")]
    public class WriterController : Controller
    {

        private readonly LibraryContext db;
        public WriterController(LibraryContext context)
        {
            db = context;
        }

        // GET: api/Writer
        [HttpGet]
        public async Task<Response<IEnumerable<Writer>>> Get(string country, string name)
        {
            return new OkResult<IEnumerable<Writer>>(await db.GetWriters(country, name));
        }

        // GET: api/Writer/id
        [HttpGet("{id}")]
        public Task<Writer> Get(string id)
        {
            return db.GetWriter(id);
        }

        // POST: api/Writer
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Writer writer)
        {
            if (writer == null)
            {
                throw new Exception("Value cannot be null");
            }
            await db.Create(writer);
            return Ok(writer);
        }

        // PUT: api/Writer/5
        [HttpPut("{id}")]
        public async Task Put([FromBody]Writer writer)
        {
            await db.Update(writer);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await db.Remove(id);
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
