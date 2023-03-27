using AnonimousMailServiceTest.Data;
using AnonimousMailServiceTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage([FromHeader] string Author, [FromHeader] string Recipient, [FromHeader] string Title, [FromHeader] string Body)
        {
            Message message = new Message()
            {
                Author = Author,
                Recipient = Recipient,
                Title = Title,
                Body = Body,
                TimeSent = DateTime.UtcNow
            };
            if (_context.Message == null)
            {
                return Problem("Entity set 'AnonimousMailServiceTestContext.Message'  is null.");
            }
            _context.Message.Add(message);

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