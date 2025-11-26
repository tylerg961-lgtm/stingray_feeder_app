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
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;

        public CreateModel(AppDbContext db) => _db = db;

        public class InputModel
        {
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

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var entity = new Caretaker
            {
                Name = Input.Name.Trim(),
                Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(Input.Phone) ? null : Input.Phone.Trim(),
                Notes = Input.Notes
            };

            try
            {
                _db.Caretakers.Add(entity);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again.");
                // Optionally log ex
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}