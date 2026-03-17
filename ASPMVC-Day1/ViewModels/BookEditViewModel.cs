using EF_day3.Entities;
using System.ComponentModel.DataAnnotations;

namespace ASPMVC_Day1.ViewModels
{
    public class BookEditViewModel
    {
        public int Id { get; set; }

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

        public byte[] Version { get; set; }

        public List<IFormFile> NewFiles { get; set; }

        public List<BookAttachment> ExistingAttachments { get; set; } = new List<BookAttachment>();
    }
}
