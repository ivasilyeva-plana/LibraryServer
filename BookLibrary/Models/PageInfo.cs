using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; } // номер текущей страницы

        public int PageSize { get; set; } // количество объектов на странице

        public int TotalItems { get; set; } // всего записей

        public int TotalPages {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize);  }
        }
    }
}
