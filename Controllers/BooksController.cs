using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bogus;
using System.Collections.Generic;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet("generate")]
        public IActionResult GetBooks([FromQuery] int seed, [FromQuery] string language, [FromQuery] double avgLikes, [FromQuery] double avgReviews)
        {
            // Ensure valid language codes
            var validLanguages = new HashSet<string> { "en", "de", "fr", "es", "it" };
            if (!validLanguages.Contains(language))
            {
                language = "en"; // Default to English
            }

            var faker = new Faker(language) { Random = new Randomizer(seed) }; // Use Faker with the selected language
            var books = new List<Book>();

            for (int i = 0; i < 10; i++)
            {
                var likes = faker.Random.Double(Math.Max(avgLikes - 5, 0), avgLikes + 5);
                var reviews = faker.Random.Double(Math.Max(avgReviews - 3, 0), avgReviews + 3);
                var publisher = faker.Company.CompanyName();
                var coverImageUrl = faker.Image.PicsumUrl(200, 300);
                var year = faker.Date.Past(100).Year;

                // Generate random comment
                var comment = faker.Lorem.Sentence();  // This will generate the comment in the selected language

                _logger.LogInformation($"Generated Book {i + 1}: Likes = {likes}, Reviews = {reviews}, Publisher = {publisher}");

                books.Add(new Book
                {
                    Index = i + 1,
                    ISBN = faker.Random.Replace("###-###-####"),
                    Title = faker.Lorem.Sentence(3), // Generate title in selected language
                    Author = faker.Name.FullName(), // Author's name in selected language
                    Publisher = publisher,
                    Likes = likes,
                    Reviews = reviews,
                    CoverImageUrl = coverImageUrl,
                    Year = year,
                    Comments = comment // Comments in the selected language
                });
            }

            return Ok(books);
        }
    }
}
