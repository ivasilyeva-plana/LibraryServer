using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookLibrary.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace BookLibrary.Controllers
{
    [Produces("application/json")]
    [Route("api/Report")]
    public class ReportController : Controller
    {

        private readonly LibraryContext db;
        public ReportController(LibraryContext context)
        {
            db = context;
        }

        // GET: api/Report
        [HttpGet]
        public  Task<IEnumerable<Report>> Get(int numReport, int year)
        {
            return db.Report(numReport, year);
        }

        // GET: api/Report/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Report
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Report/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
