using BookLibrary.Properties;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class MongoLibraryContext : ILibraryContext
    {
        private string connectionString;

        private IMongoDatabase database;
        public IMongoDatabase DB
        {
            get
            {
                if (database == null)
                {
                    var connection = new MongoUrlBuilder(connectionString);
                    // получаем клиента для взаимодействия с базой данных
                    var client = new MongoClient(connectionString);
                    // получаем доступ к самой базе данных
                    database = client.GetDatabase(connection.DatabaseName);
                }
                return database;
            }
        }

        public MongoLibraryContext(string _connectionString)
        {
            connectionString = _connectionString;
        }

        /// <summary>
        /// Get Writers collection
        /// </summary>
        private IMongoCollection<Writer> Writers
        {
            get { return DB.GetCollection<Writer>("writers"); }
        }

        // получаем все документы, используя критерии фальтрации
        public async Task<WriterPagination> GetWriters(string country, string name, int page)
        {

            int pageSize = 10000; //заготовка для порционной выдачиданных клиенту (пока не законченный постраничный вывод)

            var regex = GetSearchByNameTemplate(name);
            var resultList = await Writers.AsQueryable()
                                    .Where(i => ((String.IsNullOrEmpty(country) || country == i.country) &&
                                                 (String.IsNullOrEmpty(name) || regex.IsMatch(i.name))))
                                    .Select(i => new Writer() {Id = i.Id, name = i.name, country = i.country }) //не тянем за собой книги
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            PageInfo pageInfo = new PageInfo()
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = Writers.AsQueryable().Count()
            };
            WriterPagination result = new WriterPagination()
            {
                Writers = resultList,
                PageInfo = pageInfo
            };

            return result;
        }
        

        public async Task <IEnumerable<Book>> GetBooks(string writerId, string genre, string title)
        {
            var regex = GetSearchByNameTemplate(title);

            return await Writers.AsQueryable()
                .Where(i => (String.IsNullOrEmpty(writerId) || writerId == i.Id))
                .SelectMany(i => i.Books, (i, book) => new Book()
                {
                    writerId = i.Id,
                    writerName = i.name,
                    bookId = book.bookId,
                    title = book.title,
                    genre = book.genre,
                    published = book.published
                })
                .Where(i => ((String.IsNullOrEmpty(genre) || genre == i.genre) &&
                             (String.IsNullOrEmpty(title) || regex.IsMatch(i.title))))
                .ToListAsync();
        }


        public async Task<Writer> GetWriter(string id)
        {
            return await Writers.AsQueryable().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Book> GetBook(string writerId, int bookId)
        {
            return await Writers.AsQueryable()
                        .Where(i => i.Id == writerId)
                        .SelectMany(i => i.Books, (i, book) => new Book()
                        {
                            writerId = i.Id,
                            writerName = i.name,
                            bookId = book.bookId,
                            title = book.title,
                            genre = book.genre,
                            published = book.published
                        })
                        .Where(i => i.bookId == bookId)
                        .FirstOrDefaultAsync();
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
            var writerBooks = Writers.AsQueryable()
                .Where(p => p.Id == b.writerId)
                .SelectMany(p => p.Books);

            int i = writerBooks.Count() == 0 ? 1 : writerBooks.Max(p => p.bookId) + 1;

            var filter = Builders<Writer>
             .Filter.Eq(w => w.Id, b.writerId);

            var update = Builders<Writer>.Update
                    .Push<BaseBook>(e => e.Books, new BaseBook()
                    {
                        bookId = 1,
                        title = b.title,
                        genre = b.genre,
                        published = b.published
                    });

            await Writers.FindOneAndUpdateAsync(filter, update);
        }

        // обновление документа
        public async Task UpdateBook(Book b)
        {
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

        public async Task <IEnumerable<Report>> Report(int numReport, int year)
        {
            var res = new List<Report>();
            var listBook = new List<BaseBook>();
            switch (numReport)
            {
                // 1 - отчет книги за год по месяцам
                case 1:
                    DateTime date1 = new DateTime(year, 1, 1);
                    DateTime date2 = new DateTime(year + 1, 1, 1);
                    var months = new string[]{"январь", "февраль", "март",
                                                  "апрель", "май", "июнь",
                                                  "июль", "август", "сентябрь",
                                                  "октябрь", "ноябрь", "декабрь"};
                    var list = await Writers.AsQueryable()
                                    .SelectMany(i => i.Books)
                                    .Where(u => (u.published >= date1 && u.published < date2))
                                    .ToListAsync();

                    res = list.GroupBy(u => u.published.Month)
                                    .Select(g => new Report
                                    {
                                        Str = months[g.Key - 1],
                                        Num = g.Count()
                                    }).ToList();
                    break;
                // 2 - отчет авторы по странам
                case 2:

                    res = await Writers.AsQueryable().GroupBy(u => u.country)
                                .Select(g => new Report
                                {
                                    Str = g.Key,
                                    Num = g.Count()
                                }).ToListAsync();
                    break;
                // 3 - отчет книги по жанрам
                case 3:
                    res = await Writers.AsQueryable()
                        .SelectMany(i => i.Books)
                        .GroupBy(i => i.genre)
                                .Select(g => new Report
                                {
                                    Str = g.Key,
                                    Num = g.Count()
                                }).ToListAsync();
                    break;
                default:
                    break;
            }
            return res;
        }

        private Regex GetSearchByNameTemplate(string name)
        {
            return new Regex($"(\\w*){name}(\\w*)");
        }
    }
}
