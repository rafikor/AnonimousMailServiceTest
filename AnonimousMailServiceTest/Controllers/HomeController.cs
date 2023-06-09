﻿using AnonimousMailServiceTest.Data;
using AnonimousMailServiceTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AnonimousMailServiceTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AnonimousMailServiceTestContext _context;

        public HomeController(AnonimousMailServiceTestContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserPage(string userName)
        {
            ViewBag.UserName = userName;
            return View();
        }

        [HttpGet]
        public IActionResult SomePage(string userName)
        {
            ViewBag.UserName = userName;
            return View();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage([FromBody] Message message)
        {
            message.TimeSent= DateTime.Now;
            if (_context.Message == null)
            {
                return Problem("Entity set 'AnonimousMailServiceTestContext.Message'  is null.");
            }
            _context.Message.Add(message);

            if(_context.UserOfMailService.Where(a=>a.Name== message.Author).Count()==0)
            {
                _context.UserOfMailService.Add(new UserOfMailService() { Name=message.Author });
            }

            if (_context.UserOfMailService.Where(a => a.Name == message.Recipient).Count() == 0)
            {
                _context.UserOfMailService.Add(new UserOfMailService() { Name= message.Recipient});
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return (_context.Message?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}