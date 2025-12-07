using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using StingrayFeeder.Data.Models;

namespace stingray_feeder_app.Pages.Reports
{
    public class StoredProceduresModel : PageModel
    {
        private readonly AppDbContext _db;

        public StoredProceduresModel(AppDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int MinQuantity { get; set; } = 0;

        public IList<FeedSummary> Results { get; set; } = new List<FeedSummary>();

        public async Task OnGetAsync()
        {
            if (StartDate == default && EndDate == default)
            {
                EndDate = DateTime.UtcNow;
                StartDate = EndDate.AddDays(-7);
            }

            // Call stored procedure safely using EF Core interpolation (parameterized)
            Results = await _db.FeedSummaries
                .FromSqlInterpolated($"EXEC dbo.GetFeedSummary {StartDate}, {EndDate}, {MinQuantity}")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}