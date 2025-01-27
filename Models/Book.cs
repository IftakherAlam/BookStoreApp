namespace BookStoreApp.Models
{
    public class Book
    {
        public int Index { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public double Likes { get; set; }  // Change to double
        public double Reviews { get; set; } // Change to double
        public string CoverImageUrl { get; set; }
        public int Year { get; set; }  // Add the Year property
        public string Comments { get; set; }  // Add the Comments property
    }
}
