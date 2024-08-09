using System.ComponentModel.DataAnnotations;

namespace BooksWeb.Models
{
    public class Category
    {
        [Key] public int Id { get; set; } //Primary key
        [Required] public string Name { get; set; } //Cannot be null in db (not null)
        public int DisplayOrder { get; set; }
    }
}
