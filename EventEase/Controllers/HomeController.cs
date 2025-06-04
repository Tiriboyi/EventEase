using System.Diagnostics; // Importing the System.Diagnostics namespace for debugging and tracing
using Microsoft.AspNetCore.Mvc; // Importing ASP.NET Core MVC functionalities
using EventEase.Models; // Importing the models namespace for the application

namespace EventEase.Controllers; // Declaring the namespace for the controller

public class HomeController : Controller // Defining the HomeController class, which inherits from Controller
{
    private readonly ILogger<HomeController> _logger; // Declaring a logger to log information for debugging

    public HomeController(ILogger<HomeController> logger) // Constructor to initialize the logger
    {
        _logger = logger;
    }

    public IActionResult Index() // Action method for the homepage
    {
        return View(); // Returns the corresponding Index view
    }

    public IActionResult Privacy() // Action method for the Privacy page
    {
        return View(); // Returns the corresponding Privacy view
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // Disables caching for the Error action to ensure fresh error messages
    public IActionResult Error() // Action method for handling errors
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // Returns the Error view with a model containing the request ID
    }
}
