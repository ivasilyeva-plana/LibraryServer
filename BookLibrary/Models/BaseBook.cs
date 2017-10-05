using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    [BsonIgnoreExtraElements]
    public class BaseBook
    {
        [BsonElement(elementName: "bookId")]
        public int bookId { get; set; }
 
        [BsonElement(elementName: "title")]
        public string title { get; set; }

        [BsonElement(elementName: "genre")]
        public string genre { get; set; }

        [BsonElement(elementName: "published")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime published { get; set; }
    }
}
