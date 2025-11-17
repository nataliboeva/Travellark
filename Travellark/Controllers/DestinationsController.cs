using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Travellark.Data;
using Travellark.Models;
using Travellark.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Travellark.Controllers
{
    public class DestinationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;


        public DestinationsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Destinations
        public async Task<IActionResult> Index(string? searchString, DestinationStatus? statusFilter, DestinationType? typeFilter)
        {
            var query = _context.Destinations.AsQueryable();

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

            var totalVisited = await _context.Destinations.CountAsync(d => d.Status == DestinationStatus.Visited);
            var totalWishlist = await _context.Destinations.CountAsync(d => d.Status == DestinationStatus.Wishlist);
            var avgRating = await _context.Destinations
                .Where(d => d.Status == DestinationStatus.Visited && d.Rating.HasValue)
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

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Name,Country,Region,Type,Status,Rating,InspirationText,Notes,MapUrl,ImageFile")] Destination destination)
        {
            if (ModelState.IsValid)
            {
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

                destination.CreatedAt = DateTime.Now;

                _context.Add(destination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(destination);
        }


        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Region,Type,Status,Rating,InspirationText,Notes,ImageUrl,ImageFile,MapUrl,CreatedAt,VisitedAt")] Destination destination)
        {
            if (id != destination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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

                try
                {
                    _context.Update(destination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destination.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(destination);
        }


        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var destination = await _context.Destinations.FindAsync(id);
            if (destination != null)
            {
                _context.Destinations.Remove(destination);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.Id == id);
        }
    }
}
