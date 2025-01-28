namespace BookStoreApp.Models
{
 public class Book
{
    public int Index { get; set; }
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Publisher { get; set; }
    public double Likes { get; set; }  
    public double Reviews { get; set; } 
    public required string CoverImageUrl { get; set; }
    public int Year { get; set; }  
    public required string Comments { get; set; }
    public required string Genre { get; set; }
}

}
