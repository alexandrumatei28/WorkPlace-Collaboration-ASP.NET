using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkplaceCollaboration.Models
{
    public class ApplicationUser : IdentityUser
    {
        //un user posteaza mai multe mesaje
        public virtual ICollection<Message>? Messages { get; set; }

        // un user face parte din mai multe canale
        public virtual ICollection<Channel>? Channels { get; set; }


        // atribute suplimentare adaugate pentru user
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        // variabila in care vom retine rolurile existente in baza de date
        // pentru popularea unui dropdown list
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public ICollection<ChannelUser> ChannelUsers { get; set; } = new List<ChannelUser>();


    }
}
