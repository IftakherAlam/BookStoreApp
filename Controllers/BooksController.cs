using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bogus;
using System.Collections.Generic;
using BookStoreApp.Models;
using System.Collections;

namespace BookStoreApp.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly Dictionary<string, BookDataset> _bookDatasets;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
            _bookDatasets = InitializeBookDatasets();
        }

        private class BookDataset
        {
            public required string[] Genres { get; init; }
            public required string[] TitlePrefixes { get; init; }
            public required string[] TitleWords { get; init; }
            public required string[] Comments { get; init; }
        }
        private Dictionary<string, BookDataset> InitializeBookDatasets()
        {
            return new Dictionary<string, BookDataset>
            {
                ["en"] = new BookDataset
                {
                    Genres = new[] { "Mystery", "Romance", "Science Fiction", "Fantasy", "Thriller", "Historical Fiction" },
                    TitlePrefixes = new[] { "The", "A", "My", "Our" },
                    TitleWords = new[] { "Secret", "Mystery", "Journey", "Love", "Dream", "Shadow", "Light", "Heart", "Storm", "Legacy" },
                    Comments = new[] {
                        "A captivating read!",
                        "Couldn't put it down.",
                        "Brilliantly written.",
                        "A masterpiece of storytelling.",
                        "Highly recommended!"
                    }
                },
                ["de"] = new BookDataset
                {
                    Genres = new[] { "Krimi", "Liebesroman", "Science-Fiction", "Fantasy", "Thriller", "Historischer Roman" },
                    TitlePrefixes = new[] { "Der", "Die", "Das", "Ein", "Eine" },
                    TitleWords = new[] { "Geheimnis", "Traum", "Schatten", "Licht", "Herz", "Sturm", "Weg", "Zeit", "Leben", "Geschichte" },
                    Comments = new[] {
                        "Fantastisch geschrieben!",
                        "Ein tolles Buch!",
                        "Sehr empfehlenswert.",
                        "Eine fesselnde Geschichte.",
                        "Absolut lesenswert!"
                    }
                },
                ["fr"] = new BookDataset
                {
                    Genres = new[] { "Mystère", "Romance", "Science-Fiction", "Fantaisie", "Thriller", "Fiction Historique" },
                    TitlePrefixes = new[] { "Le", "La", "Les", "Un", "Une" },
                    TitleWords = new[] { "Secret", "Mystère", "Voyage", "Amour", "Rêve", "Ombre", "Lumière", "Coeur", "Tempête", "Destin" },
                    Comments = new[] {
                        "Une lecture fascinante!",
                        "Impossible de le poser!",
                        "Magnifiquement écrit.",
                        "Un chef-d'œuvre littéraire.",
                        "Fortement recommandé!"
                    }
                },
                ["es"] = new BookDataset
                {
                    Genres = new[] { "Misterio", "Romance", "Ciencia Ficción", "Fantasía", "Suspense", "Ficción Histórica" },
                    TitlePrefixes = new[] { "El", "La", "Los", "Un", "Una" },
                    TitleWords = new[] { "Secreto", "Misterio", "Viaje", "Amor", "Sueño", "Sombra", "Luz", "Corazón", "Tormenta", "Destino" },
                    Comments = new[] {
                        "¡Una lectura cautivadora!",
                        "¡No pude dejarlo!",
                        "Maravillosamente escrito.",
                        "Una obra maestra literaria.",
                        "¡Altamente recomendado!"
                    }
                },
                ["it"] = new BookDataset
                {
                    Genres = new[] { "Mistero", "Romanzo d'amore", "Fantascienza", "Fantasy", "Thriller", "Romanzo Storico" },
                    TitlePrefixes = new[] { "Il", "La", "I", "Un", "Una" },
                    TitleWords = new[] { "Segreto", "Mistero", "Viaggio", "Amore", "Sogno", "Ombra", "Luce", "Cuore", "Tempesta", "Destino" },
                    Comments = new[] {
                        "Una lettura affascinante!",
                        "Impossibile smettere di leggere!",
                        "Scritto magnificamente.",
                        "Un capolavoro letterario.",
                        "Altamente consigliato!"
                    }
                }
            };
        }

       [HttpGet("generate")]
public IActionResult GetBooks([FromQuery] int seed, [FromQuery] string language, [FromQuery] double avgLikes, [FromQuery] double avgReviews, [FromQuery] int start = 0)
{
    var validLanguages = new HashSet<string> { "en", "de", "fr", "es", "it" };
    if (!validLanguages.Contains(language))
    {
        language = "en";
    }

    // Create a deterministic random generator with the seed
    var rand = new Random(seed);

    // Skip the first 'start' numbers to get to the right position in the sequence
    for (int i = 0; i < start; i++)
    {
        rand.Next();
        rand.NextDouble();  // Skip for both Next() and NextDouble() to maintain consistency
    }

    var dataset = _bookDatasets[language];
    var books = new List<Book>();

    var faker = new Faker(language)
    {
        Random = new Randomizer(seed) // Use the same Random object across Faker's randomizer
    };

    for (int i = 0; i < 10; i++)
    {
        var likes = faker.Random.Double(Math.Max(avgLikes - 5, 0), avgLikes + 5);
        var reviews = faker.Random.Double(Math.Max(avgReviews - 3, 0), avgReviews + 3);
        var publisher = faker.Company.CompanyName();
        var coverImageUrl = faker.Image.PicsumUrl(200, 300);
        var year = faker.Date.Past(100).Year;

        // Generate title using language-specific patterns
        var title = GenerateBookTitle(faker, dataset);
        var comment = faker.Random.ArrayElement(dataset.Comments);
        var genre = faker.Random.ArrayElement(dataset.Genres);

        books.Add(new Book
        {
            Index = i + 1,
            ISBN = faker.Random.Replace("###-###-####"),
            Title = title,
            Author = faker.Name.FullName(),
            Publisher = publisher,
            Genre = genre,
            Likes = likes,
            Reviews = reviews,
            CoverImageUrl = coverImageUrl,
            Year = year,
            Comments = comment
        });
    }

    return Ok(books);
}

        private string GenerateBookTitle(Faker faker, BookDataset dataset)
        {
            var usePrefix = faker.Random.Bool(0.7f); // 70% chance to use prefix
            var prefix = usePrefix ? faker.Random.ArrayElement(dataset.TitlePrefixes) + " " : "";
            var words = faker.Random.ArrayElements(dataset.TitleWords, faker.Random.Number(1, 2));
            
            return $"{prefix}{string.Join(" ", words)}";
        }
    }
}