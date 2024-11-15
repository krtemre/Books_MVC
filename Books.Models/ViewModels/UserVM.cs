using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.Models.ViewModels
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; } //Roles
        public IEnumerable<SelectListItem>? Companies { get; set; }
    }
}
