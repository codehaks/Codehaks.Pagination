using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeopleApp.Data;

namespace Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PeopleDbContext _db;
        public IndexModel(PeopleDbContext db)
        {
            _db = db;
            PageSize = 10;
        }
        public IList<User> UserList { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        [ViewData]
        public int PageNumber { get; set; }

        public void OnGet(int number = 1)
        {
            Expression<Func<User, User>> expSelect = u =>
            new User
            {
                Givenname = u.Givenname,
                Maidenname = u.Maidenname,
                Age = u.Age
            };

            var skip = PageSize * (number - 1);

            TotalPages = _db.Users.Count() / PageSize;

            UserList = _db.Users
                .Select(expSelect)
                .Skip(skip)
                .Take(PageSize)
                .ToList();

            PageNumber = number;
        }
    }
}