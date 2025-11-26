using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using StingrayFeeder.Data.Models;

namespace stingray_feeder_app.Pages.Caretakers
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db) => _db = db;

        public class InputModel
        {
            public int Id { get; set; }

            [Required]
            [StringLength(120)]
            public string Name { get; set; } = string.Empty;

            [EmailAddress]
            [StringLength(200)]
            public string? Email { get; set; }

            [StringLength(50)]
            public string? Phone { get; set; }

            [StringLength(1000)]
            public string? Notes { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _db.Caretakers.FindAsync(id.Value);
            if (entity == null) return NotFound();

            Input = new InputModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                Notes = entity.Notes
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var entity = await _db.Caretakers.FindAsync(Input.Id);
            if (entity == null) return NotFound();

            entity.Name = Input.Name.Trim();
            entity.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
            entity.Phone = string.IsNullOrWhiteSpace(Input.Phone) ? null : Input.Phone.Trim();
            entity.Notes = Input.Notes;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Caretakers.AnyAsync(c => c.Id == Input.Id))
                {
                    return NotFound();
                }
                ModelState.AddModelError(string.Empty, "The record was changed by another user. Please reload and try again.");
                return Page();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}