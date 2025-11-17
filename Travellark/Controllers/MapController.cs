using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travellark.Data;
using Travellark.Models.Enums;

namespace Travellark.Controllers
{
    [Authorize]
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MapController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var destinations = await _context.Destinations
                .Where(d => d.UserId == userId && d.Latitude.HasValue && d.Longitude.HasValue)
                .ToListAsync();

            var destinationsData = destinations.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Country = d.Country,
                Region = d.Region,
                Latitude = d.Latitude.Value,
                Longitude = d.Longitude.Value,
                Status = d.Status.ToString(),
                ImageUrl = d.ImageUrl,
                Rating = d.Rating
            }).ToList();

            var visitedCount = destinations.Count(d => d.Status == DestinationStatus.Visited);
            var wishlistCount = destinations.Count(d => d.Status == DestinationStatus.Wishlist);

            ViewBag.Destinations = destinationsData;
            ViewBag.VisitedCount = visitedCount;
            ViewBag.WishlistCount = wishlistCount;

            return View();
        }
    }
}

