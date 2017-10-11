using BookLibrary.Controllers;
using BookLibrary.Models;
using BookLibrary.Response;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookLibrary.Tests
{
    public class WriterControllerTest
    {
        private Mock<ILibraryContext> dbContext;

        public WriterControllerTest()
        {
            InitMockObjects();
        }

        [Fact]
        public void GetReturnsAResponseWithAListOfWriters()
        {
            var controller = new WriterController(dbContext.Object);

            var country = "Англия";
            var name = "о";
            var page = 1;
            var result = controller.Get(country, name, page).Result;

            // Assert
            var viewResult = Assert.IsType<OkResult<WriterPagination>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Writer>>(viewResult.Result.Writers);
            dbContext.Verify(db => db.GetWriters(It.Is<string>(v => v == country), It.Is<string>(v => v == name), It.Is<int>(v => v == page)));
        }

        private void InitMockObjects()
        {
            dbContext = new Mock<ILibraryContext>();

            dbContext.Setup(dbContext => dbContext.GetWriters(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() =>
                {
                    return Task.Factory.StartNew(
                    () =>
                    {
                        return new WriterPagination()
                        {
                            Writers = new List<Writer>()
                            {
                                new Writer()
                                {
                                    Id ="1", name="Конан Дойл", country="Англия", Books=new List<BaseBook>()
                                },
                                new Writer()
                                {
                                    Id ="1", name="Конан Дойл", country="Англия", Books=new List<BaseBook>()
                                },
                                new Writer()
                                {
                                    Id ="1", name="Конан Дойл", country="Англия", Books=new List<BaseBook>()
                                },
                                new Writer()
                                {
                                    Id ="1", name="Конан Дойл", country="Англия", Books=new List<BaseBook>()
                                }
                            }
                        };
                    });
                });
        }
    }
}
