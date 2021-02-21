using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codehaks.Pagination
{
    public class PaginationService<T>
    {
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }

        public IQueryable<T> Config(IQueryable<T> dbset,int number=1)
        {
            var skip = PageSize * (number - 1);

            return dbset.Skip(skip)
                .Take(PageSize);

        }
    }
}
