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
using System.Security.Claims;

namespace StackBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewsController(IUnitOfWork unitOfWork, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork;
            _reviewService = reviewService;
        }
    }
}