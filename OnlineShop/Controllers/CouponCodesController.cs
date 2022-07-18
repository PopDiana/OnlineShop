using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Data;
using IdentityCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityCore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static Random random = new Random();


        public CouponCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CouponCodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.CouponCodes.ToListAsync());
        }

        // GET: CouponCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponCode = await _context.CouponCodes
                .FirstOrDefaultAsync(m => m.CouponCodeId == id);
            if (couponCode == null)
            {
                return NotFound();
            }

            return View(couponCode);
        }

        private static string GenerateVoucher(char[] keys, int lengthOfVoucher)
        {
            return Enumerable
                .Range(1, lengthOfVoucher) 
                .Select(k => keys[random.Next(0, keys.Length - 1)])  
                .Aggregate("", (e, c) => e + c); 
        }

        // GET: CouponCodes/Create
        public IActionResult Create()
        {
            int lengthOfVoucher = 10;
            char[] keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890".ToCharArray();

            var voucher = GenerateVoucher(keys, lengthOfVoucher);

            CouponCode couponCode = new CouponCode();
            couponCode.CouponName = voucher;

            return View(couponCode);
        }

        // POST: CouponCodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CouponCodeId,CouponName,CouponPercentage,isAvailable")] CouponCode couponCode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(couponCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(couponCode);
        }

        // GET: CouponCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponCode = await _context.CouponCodes.FindAsync(id);
            if (couponCode == null)
            {
                return NotFound();
            }
            return View(couponCode);
        }

        // POST: CouponCodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CouponCodeId,CouponName,CouponPercentage,isAvailable")] CouponCode couponCode)
        {
            if (id != couponCode.CouponCodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(couponCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponCodeExists(couponCode.CouponCodeId))
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
            return View(couponCode);
        }

        // GET: CouponCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var couponCode = await _context.CouponCodes
                .FirstOrDefaultAsync(m => m.CouponCodeId == id);
            if (couponCode == null)
            {
                return NotFound();
            }

            return View(couponCode);
        }

        // POST: CouponCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var couponCode = await _context.CouponCodes.FindAsync(id);
            _context.CouponCodes.Remove(couponCode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponCodeExists(int id)
        {
            return _context.CouponCodes.Any(e => e.CouponCodeId == id);
        }
    }
}
