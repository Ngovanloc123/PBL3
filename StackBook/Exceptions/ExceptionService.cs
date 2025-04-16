using Microsoft.EntityFrameworkCore;
using StackBook.Data;
using StackBook.Models;
// using StackBook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.Exceptions
{
    public class OutOfStockException : Exception
    {
        public OutOfStockException(string message) : base(message) 
        {}
    }
}