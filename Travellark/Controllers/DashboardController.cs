using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travellark.Data;
using Travellark.Models.Enums;

namespace Travellark.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Overall Statistics
            var totalDestinations = await _context.Destinations
                .CountAsync(d => d.UserId == userId);

            var totalVisited = await _context.Destinations
                .CountAsync(d => d.UserId == userId && d.Status == DestinationStatus.Visited);

            var totalWishlist = await _context.Destinations
                .CountAsync(d => d.UserId == userId && d.Status == DestinationStatus.Wishlist);

            var totalFavorites = await _context.Favorites
                .CountAsync(f => f.UserId == userId);

            // Rating Statistics
            var avgRating = await _context.Destinations
                .Where(d => d.UserId == userId
                            && d.Status == DestinationStatus.Visited
                            && d.Rating.HasValue)
                .AverageAsync(d => (double?)d.Rating) ?? 0.0;

            var ratedDestinations = await _context.Destinations
                .CountAsync(d => d.UserId == userId
                            && d.Status == DestinationStatus.Visited
                            && d.Rating.HasValue);

            // Top Visited Countries
            var topVisitedCountries = await _context.Destinations
                .Where(d => d.UserId == userId && d.Status == DestinationStatus.Visited)
                .GroupBy(d => d.Country)
                .Select(g => new { Country = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Top Rated Destinations
            var topRatedDestinations = await _context.Destinations
                .Where(d => d.UserId == userId
                            && d.Status == DestinationStatus.Visited
                            && d.Rating.HasValue)
                .OrderByDescending(d => d.Rating)
                .ThenByDescending(d => d.VisitedAt)
                .Take(10)
                .ToListAsync();

            // Destinations by Type
            var byType = await _context.Destinations
                .Where(d => d.UserId == userId)
                .GroupBy(d => d.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            // Destinations by Status
            var byStatus = await _context.Destinations
                .Where(d => d.UserId == userId)
                .GroupBy(d => d.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            // Top Countries
            var topCountries = await _context.Destinations
                .Where(d => d.UserId == userId)
                .GroupBy(d => d.Country)
                .Select(g => new { Country = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Rating Distribution
            var ratingDistribution = await _context.Destinations
                .Where(d => d.UserId == userId
                            && d.Status == DestinationStatus.Visited
                            && d.Rating.HasValue)
                .GroupBy(d => d.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .OrderBy(x => x.Rating)
                .ToListAsync();

            // Recent Activity
            var recentActivity = await _context.Destinations
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(10)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Country,
                    d.Status,
                    d.CreatedAt,
                    Action = "Added"
                })
                .ToListAsync();

            var recentVisits = await _context.Destinations
                .Where(d => d.UserId == userId
                            && d.VisitedAt.HasValue)
                .OrderByDescending(d => d.VisitedAt)
                .Take(10)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Country,
                    d.Status,
                    VisitedAt = d.VisitedAt.Value,
                    Action = "Visited"
                })
                .ToListAsync();

            // Additional Statistics
            var totalCountriesVisited = await _context.Destinations
                .Where(d => d.UserId == userId && d.Status == DestinationStatus.Visited)
                .Select(d => d.Country)
                .Distinct()
                .CountAsync();

            var mostVisitedType = await _context.Destinations
                .Where(d => d.UserId == userId && d.Status == DestinationStatus.Visited)
                .GroupBy(d => d.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();


            var favoriteIds = await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .Select(f => f.DestinationId)
                .ToListAsync();

            var favoriteDestinations = await _context.Destinations
                .Where(d => favoriteIds.Contains(d.Id))
                .ToListAsync();

            ViewBag.TotalDestinations = totalDestinations;
            ViewBag.TotalVisited = totalVisited;
            ViewBag.TotalWishlist = totalWishlist;
            ViewBag.TotalFavorites = totalFavorites;
            ViewBag.AvgRating = avgRating;
            ViewBag.RatedDestinations = ratedDestinations;
            ViewBag.ByType = byType;
            ViewBag.ByStatus = byStatus;
            ViewBag.TopCountries = topCountries;
            ViewBag.TopVisitedCountries = topVisitedCountries;
            ViewBag.TopRatedDestinations = topRatedDestinations;
            ViewBag.RatingDistribution = ratingDistribution;
            ViewBag.RecentActivity = recentActivity;
            ViewBag.RecentVisits = recentVisits;
            ViewBag.TotalCountriesVisited = totalCountriesVisited;
            ViewBag.MostVisitedType = mostVisitedType;
            ViewBag.FavoriteDestinations = favoriteDestinations;

            return View();
        }
    }
}

