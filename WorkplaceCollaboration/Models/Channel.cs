using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WorkplaceCollaboration.Models
{
    public class Channel
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea este obligatorie")]
        [StringLength(100, ErrorMessage = "Denumirea nu poate avea mai mult de 20 de caractere")]
        [MinLength(5, ErrorMessage = "Denumirea trebuie sa aiba mai mult de 5 caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        // un canal are asociata o categorie
        public int? CategoryId { get; set; }

        // un canal este creat de catre un user - FK
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Category? Category { get; set; }

        // un canal poate avea o colectie de mesaje
        public virtual ICollection<Message>? Messages { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

        public ICollection<ChannelUser> ChannelUsers { get; set; } = new List<ChannelUser>();

    }
}
