using Fileshard.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Fileshard.Service.Repository;
using Fileshard.Service.Database;

namespace Fileshard.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ICollectionRepository _collectionRepository;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _collectionRepository = new CollectionRepository();
        }

        public IActionResult Index()
        {
            var collections = _collectionRepository.GetAll().Result;

            var objects = _collectionRepository.GetObjects(collections.First().Id).Result;

            return View(objects);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
