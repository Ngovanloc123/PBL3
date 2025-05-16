using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShippingAddressController : Controller
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingAddressController(IUnitOfWork unitOfWork, IShippingAddressService shippingAddressService)
        {
            _unitOfWork = unitOfWork;
            _shippingAddressService = shippingAddressService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                _shippingAddressService.Add(shippingAddress);
                await _unitOfWork.SaveAsync();
                ViewData["success"] = "Add shipping address successful";

                // Lấy URL trang trước (nếu có)
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                    return Redirect(referer);

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return PartialView("_AddShippingAddressModal", shippingAddress);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(Guid ShippingAddressId)
        {
            // Lấy dữ liệu từ session
            var sessionData = HttpContext.Session.GetString("CheckoutRequest");
            if (string.IsNullOrEmpty(sessionData))
            {
                TempData["Error"] = "No checkout data found.";
                return RedirectToAction("Index", "Cart");
            }

            // Parse session thành CheckoutRequest
            var checkoutRequest = JsonConvert.DeserializeObject<CheckoutRequest>(sessionData);

            // Lấy thông tin địa chỉ từ database theo ID
            var selectedAddress = await _unitOfWork.ShippingAddress.GetAsync(addr => addr.ShippingAddressId == ShippingAddressId);
            if (selectedAddress == null)
            {
                TempData["Error"] = "Shipping address not found.";
                return RedirectToAction("Checkout", "Cart");
            }

            // Cập nhật địa chỉ mặc định
            checkoutRequest.shippingAddressDefault = selectedAddress;

            // Ghi lại vào session
            HttpContext.Session.SetString("CheckoutRequest", JsonConvert.SerializeObject(checkoutRequest));

            // Quay lại trang Checkout
            return RedirectToAction("Checkout", "Cart");
        }






        // GET: Customer/ShippingAddresses
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.ShippingAddresses.Include(s => s.User);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Customer/ShippingAddresses/Details/:id
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var shippingAddress = await _context.ShippingAddresses
        //        .Include(s => s.User)
        //        .FirstOrDefaultAsync(m => m.ShippingAddressId == id);
        //    if (shippingAddress == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(shippingAddress);
        //}

        // GET: Customer/ShippingAddresses/Create
        //public IActionResult Create()
        //{
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
        //    return View();
        //}




        // GET: Customer/ShippingAddresses/Edit/:id
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var shippingAddress = await _context.ShippingAddresses.FindAsync(id);
        //    if (shippingAddress == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", shippingAddress.UserId);
        //    return View(shippingAddress);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, ShippingAddress shippingAddress)
        //{
        //    if (id != shippingAddress.ShippingAddressId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(shippingAddress);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ShippingAddressExists(shippingAddress.ShippingAddressId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", shippingAddress.UserId);
        //    return View(shippingAddress);
        //}

        // GET: Customer/ShippingAddresses/Delete/:id
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var shippingAddress = await _context.ShippingAddresses
        //        .Include(s => s.User)
        //        .FirstOrDefaultAsync(m => m.ShippingAddressId == id);
        //    if (shippingAddress == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(shippingAddress);
        //}

        // POST: Customer/ShippingAddresses/Delete/:id
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var shippingAddress = await _context.ShippingAddresses.FindAsync(id);
        //    if (shippingAddress != null)
        //    {
        //        _context.ShippingAddresses.Remove(shippingAddress);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ShippingAddressExists(Guid id)
        //{
        //    return _context.ShippingAddresses.Any(e => e.ShippingAddressId == id);
        //}
    }
}
