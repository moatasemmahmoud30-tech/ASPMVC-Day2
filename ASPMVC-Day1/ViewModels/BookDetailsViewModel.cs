namespace ASPMVC_Day1.ViewModels
{
    public class BookDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PublishYear { get; set; }
        public decimal Price { get; set; }

        public string AuthorName { get; set; }
        public string CategoryName { get; set; }

        public List<string> AttachmentUrls { get; set; } = new List<string>();
    }
}
