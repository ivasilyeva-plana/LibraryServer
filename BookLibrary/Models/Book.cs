using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class Book : BaseBook
    {
        [BsonElement(elementName: "writerId")]
        public string writerId { get; set; }

        [BsonElement(elementName: "writerName")]
        public string writerName { get; set; }

        public Book()
        {

        }

        public Book(BaseBook b, string wId, string wName)
        {
            bookId = b.bookId;
            title = b.title;
            genre = b.genre;
            published = b.published;
            writerId = wId;
            writerName = wName;
        }
    }
}
