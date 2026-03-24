namespace ASPMVC_Day1.ViewModels
{
    public class BookCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }

        public string CoverImageUrl { get; set; }
    }
}
