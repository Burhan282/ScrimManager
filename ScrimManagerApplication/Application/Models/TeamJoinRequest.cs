using System;

namespace ScrimManagerApplication.Application.Models
{
    public class TeamJoinRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TeamId { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; } = string.Empty;

        public string TeamName { get; set; } = string.Empty;
        public Rank UserRank { get; set; } 
    }
}