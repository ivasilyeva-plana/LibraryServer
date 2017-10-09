using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    
    public class Writer 
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public virtual IEnumerable<BaseBook> Books { get; set; }
        public Writer()
        {
            Books = new List<BaseBook>();
        }
    }

    public class WriterPagination
    {
        public IEnumerable<Writer> Writers { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}
