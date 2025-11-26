using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using StingrayFeeder.Data.Models;

namespace stingray_feeder_app.Pages.Caretakers
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _db;

        public DetailsModel(AppDbContext db) => _db = db;

        public Caretaker? Caretaker { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Caretaker = await _db.Caretakers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (Caretaker == null) return NotFound();

            return Page();
        }
    }
}