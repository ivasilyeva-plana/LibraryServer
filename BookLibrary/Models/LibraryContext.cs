using BookLibrary.Properties;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class LibraryContext
    {
        IMongoDatabase database; // база данных

        public LibraryContext()
        {
            // строка подключения
            var connectionString = Resources.ResourceManager.GetString("MONGODB_CONNECTION_STRING");
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            var client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            database = client.GetDatabase(connection.DatabaseName);
        }

        // обращаемся к коллекции Writers
        private IMongoCollection<Writer> Writers
        {
            get { return database.GetCollection<Writer>("writers"); }
        }

        // получаем все документы, используя критерии фальтрации
        public async Task<IEnumerable<Writer>> GetWriters(string country, string name)
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Writer>();

            var filter = builder.Empty; // фильтр для выборки всех документов
            // фильтр по имени
            if (!String.IsNullOrEmpty(country))
            {
                filter = filter & builder.Eq("country", country);
            }
            if (!String.IsNullOrEmpty(name))
            {
                filter = filter & builder.Regex("name", new BsonRegularExpression(name));
            }

            var options = new FindOptions<Writer>()
            {
                Projection = Builders<Writer>.Projection.Exclude(w => w.Books)
            };

            return await Writers.Find(filter)
                 .Project<Writer>(options.Projection).ToListAsync();
        }
        
        public async Task <IEnumerable<Book>> GetBooks(string writerId, string genre, string title)
        {

            var builder = new FilterDefinitionBuilder<Writer>();

            var filter = builder.Empty;

            if (!String.IsNullOrEmpty(writerId))
            {
                filter = filter & builder.Eq(w => w.Id, writerId);
            }

            var options = new FindOptions<Writer>()
            {
                Projection = Builders<Writer>.Projection.Include(w => w.Id).Include(w => w.name).Include(w => w.Books)
            };

            var listWriter = await Writers.Find(filter).Project<Writer>(options.Projection).ToListAsync();

            List<Book> result = new List<Book>();

            Regex regex = new Regex(@"(\w*)"+ title+ @"(\w*)");

            foreach (var writerItem in listWriter)
            {
                foreach (var bookItem in writerItem.Books)
                {
                    if (( String.IsNullOrEmpty(genre) || genre==bookItem.genre) &&
                        ( String.IsNullOrEmpty(title) || regex.IsMatch(bookItem.title)))
                    {
                        result.Add(new Book()
                        {
                            writerId = writerItem.Id,
                            writerName = writerItem.name,
                            bookId = bookItem.bookId,
                            title = bookItem.title,
                            genre = bookItem.genre,
                            published = bookItem.published
                        });
                    }
                }
            }

            return result;
            

           // return await Writers.Find(builder.Empty).Project<Writer>(options.Projection).ToListAsync(); 

            /*
            return await Writers.Aggregate()
                            .Match(filter)
                            .Project(w => new { w.Books })
                            .Unwind(w => w.Books).ToListAsync();*/

        }


        public async Task<Writer> GetWriter(string id)
        {
            return await Writers.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task<Book> GetBook(string writerId, int bookId)
        {
            var wr = await Writers.Find(new BsonDocument("_id", new ObjectId(writerId))).FirstOrDefaultAsync();

            return new Book(wr.Books.Where(i => i.bookId == bookId).First<BaseBook>(), wr.Id, wr.name);
            
        }

        public async Task Create(Writer w)
        {
            await Writers.InsertOneAsync(w);
        }

        public async Task Update(Writer w)
        {
            await Writers.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(w.Id)), w);
        }

        public async Task Remove(string id)
        {
            await Writers.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }


        public async Task CreateBook(Book b)
        {
            var filter = Builders<Writer>
             .Filter.Eq(w => w.Id, b.writerId);

            List<Writer> theWriter =  Writers.Find(Builders<Writer>.Filter.Eq("_id", new ObjectId(b.writerId))).ToList();
            List<BaseBook> bookMaxIndex = theWriter[0].Books.OrderByDescending(p => p.bookId).Take(1).ToList();
            int i = bookMaxIndex.Count() == 0? 1 : bookMaxIndex[0].bookId + 1;

            var update = Builders<Writer>.Update
                    .Push<BaseBook>(e => e.Books, new BaseBook()
                    {
                        bookId = i,
                        title = b.title,
                        genre = b.genre,
                        published = b.published
                    });

            await Writers.FindOneAndUpdateAsync(filter, update);
        }

        // обновление документа
        public async Task UpdateBook(Book b)
        {/*
            var filter = Builders<Writer>.Filter.And(
                Builders<Writer>.Filter.Where(x => x.Id == b.writerId),
                Builders<Writer>.Filter.ElemMatch(x => x.Books, x => x.bookId == b.bookId));

            var update = Builders<Writer>.Update.Set(x => x.Books[-1].title, b.title);

            await Writers.UpdateOneAsync(filter, update);*/

            var filter = Builders<Writer>
             .Filter.Eq(w => w.Id, b.writerId);

            var update = Builders<Writer>.Update.PullFilter(w => w.Books, f => f.bookId == b.bookId);
            await Writers.FindOneAndUpdateAsync(filter, update);

            update = Builders<Writer>.Update
                    .Push<BaseBook>(e => e.Books, new BaseBook()
                    {
                        bookId = b.bookId,
                        title = b.title,
                        genre = b.genre,
                        published = b.published
                    });

            await Writers.FindOneAndUpdateAsync(filter, update);
        }

        public async Task RemoveBook(string writerId, int bookId)
        {

            var filter = Builders<Writer>
             .Filter.Eq(w => w.Id, writerId);

            var update = Builders<Writer>.Update.PullFilter(w => w.Books, f => f.bookId == bookId);
            await Writers.FindOneAndUpdateAsync(filter, update);

        }

    }
}
