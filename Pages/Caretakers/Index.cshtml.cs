using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using StingrayFeeder.Data.Models;

namespace stingray_feeder_app.Pages.Caretakers
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db) => _db = db;

        public IList<Caretaker> Caretakers { get; private set; } = new List<Caretaker>();

        public async Task OnGetAsync()
        {
            Caretakers = await _db.Caretakers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}