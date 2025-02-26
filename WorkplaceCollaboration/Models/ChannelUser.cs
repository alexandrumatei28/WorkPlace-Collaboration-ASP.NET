using Microsoft.AspNetCore.Identity;

namespace WorkplaceCollaboration.Models
{
    public class ChannelUser
    {
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Status { get; set; }
    }

}
