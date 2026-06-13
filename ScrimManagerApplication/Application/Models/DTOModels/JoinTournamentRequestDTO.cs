namespace ScrimManagerApplication.Application.Models
{
    public class JoinTournamentRequestDTO
    {
        public int TournamentId { get; set; }

        public string? EntryName { get; set; }

        public int? TeamId { get; set; }

        public List<int> PlayerIds { get; set; } = new();
    }
}