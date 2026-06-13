namespace ScrimManagerApplication.Application.Models
{
    public class TournamentParticipationDetails
    {
        public int ParticipantId { get; set; }
        public int? TeamId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public byte[]? TeamLogoData { get; set; }
        public List<TournamentParticipationPlayer> Players { get; set; } = new();
    }

    public class TournamentParticipationPlayer
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
