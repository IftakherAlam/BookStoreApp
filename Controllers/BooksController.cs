using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private static readonly HashSet<string> _usedTitles = new HashSet<string>();
        private static readonly HashSet<string> _usedISBNs = new HashSet<string>();

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet("generate")]
        public IActionResult GetBooks(
            [FromQuery] int seed,
            [FromQuery] string language,
            [FromQuery] double avgLikes,
            [FromQuery] double avgReviews,
            [FromQuery] int start = 0,
            [FromQuery] int count = 10)
        {
            // Set the seed for deterministic behavior
            Randomizer.Seed = new Random(seed);

            // Set up language-specific faker
            var faker = language.ToLower() switch
            {
                "ru" => new Faker<Book>("ru"),
                "fr" => new Faker<Book>("fr"),
                "de" => new Faker<Book>("de"),
                "es" => new Faker<Book>("es"),
                "it" => new Faker<Book>("it"),
                _ => new Faker<Book>("en")
            };

            var books = new List<Book>();
            var baseIndex = start + 1;

            // Configure the faker with rules for book generation
            faker
                .RuleFor(b => b.Index, f => baseIndex++)
                .RuleFor(b => b.Title, (f, b) => GenerateUniqueTitle(f, seed + b.Index)) // Pass seed + index for unique titles
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.ISBN, (f, b) => GenerateUniqueISBN(f, seed + b.Index)) // Pass seed + index for unique ISBNs
                .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
                .RuleFor(b => b.Year, f => f.Date.Past(30).Year)
                .RuleFor(b => b.Likes, f => Math.Max(0, avgLikes - 5 + (f.Random.Double() * 10)))
                .RuleFor(b => b.Reviews, f => Math.Max(0, avgReviews - 3 + (f.Random.Double() * 6)))
                .RuleFor(b => b.Comments, f => f.Lorem.Sentence())
                .RuleFor(b => b.CoverImageUrl, (f, b) => $"https://picsum.photos/seed/{seed + b.Index}/200/300"); // Use seed + index for deterministic cover image

            // Skip records if needed
            for (int i = 0; i < start; i++)
            {
                faker.Generate();
            }

            // Generate the requested books
            books = faker.Generate(count);

            return Ok(books);
        }

        private string GenerateUniqueTitle(Faker faker, int seed)
        {
            // Use the seed to ensure deterministic behavior
            var random = new Random(seed);
            string title;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                var words = faker.Lorem.Words(random.Next(2, 5)); // Use seeded random
                title = System.Globalization.CultureInfo.CurrentCulture.TextInfo
                    .ToTitleCase(string.Join(" ", words));

                if (random.NextDouble() < 0.4) // Use seeded random
                {
                    var prefixes = new[] { "The", "A", "My", "Our" };
                    title = $"{prefixes[random.Next(prefixes.Length)]} {title}";
                }

                attempts++;
                if (attempts >= maxAttempts)
                {
                    title = $"{title} {Guid.NewGuid().ToString().Substring(0, 4)}";
                    break;
                }
            }
            while (_usedTitles.Contains(title));

            _usedTitles.Add(title);
            return title;
        }

        private string GenerateUniqueISBN(Faker faker, int seed)
        {
            // Use the seed to ensure deterministic behavior
            var random = new Random(seed);
            string isbn;
            do
            {
                var group = random.Next(0, 10); // Use seeded random
                var publisher = random.Next(100000, 1000000); // Use seeded random
                var title = random.Next(100, 1000); // Use seeded random
                var checkDigit = random.Next(0, 10); // Use seeded random

                isbn = $"978-{group}-{publisher}-{title}-{checkDigit}";
            }
            while (_usedISBNs.Contains(isbn));

            _usedISBNs.Add(isbn);
            return isbn;
        }
    }
}