using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementWebApp.Data;
using UserManagementWebApp.Models;
using UserManagementWebApp.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["AgeSortParm"] = sortOrder == "Age" ? "age_desc" : "Age";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["PageNumber"] = pageNumber ?? 1;


            var users = from u in _db.Users select u;

            // 🔎 Поиск
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.Contains(searchString) || u.Email.Contains(searchString));
            }

            // ↕ Сортировка
            users = sortOrder switch
            {
                "name_desc" => users.OrderByDescending(u => u.Name),
                "Email" => users.OrderBy(u => u.Email),
                "email_desc" => users.OrderByDescending(u => u.Email),
                "Age" => users.OrderBy(u => u.Age),
                "age_desc" => users.OrderByDescending(u => u.Age),
                "Date" => users.OrderBy(u => u.CreatedAt),
                "date_desc" => users.OrderByDescending(u => u.CreatedAt),
                _ => users.OrderBy(u => u.Name),
            };

            // 📄 Пагинация (5 элементов на страницу)
            int pageSize = 5;
            return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Users/Create
        public IActionResult Create() => View();

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.UtcNow;
                _db.Add(user);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.Users.Any(e => e.Id == user.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}
