using System.ComponentModel.DataAnnotations;

namespace ASPMVC_Day1.ViewModels
{
    public class BookCreateViewModel
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int PublishYear { get; set; }

        public decimal Price { get; set; }

        public List<IFormFile> Files { get; set; }
    }
}
