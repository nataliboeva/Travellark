using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Travellark.Data;
using Travellark.Models;
using Travellark.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Travellark.Controllers
{
    [Authorize]
    public class DestinationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;


        public DestinationsController(ApplicationDbContext context,IWebHostEnvironment environment,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // GET: Destinations
        public async Task<IActionResult> Index(string? searchString, DestinationStatus? statusFilter, DestinationType? typeFilter)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Destinations
            .Where(d => d.UserId == userId)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            if (statusFilter.HasValue)
            {
                query = query.Where(d => d.Status == statusFilter);
            }

            if (typeFilter.HasValue)
            {
                query = query.Where(d => d.Type == typeFilter);
            }

            var destinations = await query.ToListAsync();

            var totalVisited = await _context.Destinations
                .CountAsync(d => d.UserId == userId && d.Status == DestinationStatus.Visited);

            var totalWishlist = await _context.Destinations
                .CountAsync(d => d.UserId == userId && d.Status == DestinationStatus.Wishlist);

            var avgRating = await _context.Destinations
                .Where(d => d.UserId == userId
                            && d.Status == DestinationStatus.Visited
                            && d.Rating.HasValue)
                .AverageAsync(d => (double?)d.Rating) ?? 0.0;

            ViewBag.TotalVisited = totalVisited;
            ViewBag.TotalWishlist = totalWishlist;
            ViewBag.AvgRating = avgRating;

            return View(destinations);
        }


        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // GET: Destinations/Create
        public IActionResult Create()
        {
            var destination = new Destination
            {
                Status = DestinationStatus.Wishlist,
                Type = DestinationType.City
            };

            return View(destination);
        }

        // POST: Destinations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return View(destination);
            }

            var userId = _userManager.GetUserId(User);
            destination.UserId = userId;
            destination.CreatedAt = DateTime.Now;

            if (destination.ImageFile != null && destination.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(destination.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await destination.ImageFile.CopyToAsync(stream);
                }

                destination.ImageUrl = "/uploads/" + fileName;
            }

            if (destination.Status == DestinationStatus.Visited && destination.VisitedAt == null)
            {
                destination.VisitedAt = DateTime.Now;
            }

            _context.Add(destination);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Destination destination)
        {
            if (id != destination.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(destination);
            }

            var userId = _userManager.GetUserId(User);

            var existing = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (existing == null)
            {
                return NotFound();
            }

            // Обновяваме полетата едно по едно
            existing.Name = destination.Name;
            existing.Country = destination.Country;
            existing.Region = destination.Region;
            existing.Type = destination.Type;
            existing.Status = destination.Status;
            existing.Rating = destination.Rating;
            existing.InspirationText = destination.InspirationText;
            existing.Notes = destination.Notes;
            existing.MapUrl = destination.MapUrl;
            existing.VisitedAt = destination.VisitedAt;

            // CreatedAt и UserId НЕ пипаме
            // existing.CreatedAt си остава, existing.UserId си остава

            // обработка на снимка, ако има нова
            if (destination.ImageFile != null && destination.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(destination.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await destination.ImageFile.CopyToAsync(stream);
                }

                existing.ImageUrl = "/uploads/" + fileName;
            }

            // ако статусът е Visited и няма дата
            if (existing.Status == DestinationStatus.Visited && existing.VisitedAt == null)
            {
                existing.VisitedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsVisited(int id)
        {
            var userId = _userManager.GetUserId(User);

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (destination == null)
            {
                return NotFound();
            }

            if (destination.Status == DestinationStatus.Visited)
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }

            destination.Status = DestinationStatus.Visited;

            if (!destination.VisitedAt.HasValue)
            {
                destination.VisitedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = id });
        }


        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (destination == null)
            {
                return NotFound();
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.Id == id);
        }
    }
}
