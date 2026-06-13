namespace ScrimManagerApplication.Application.Models
{
    public class TournamentInvitation
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int TournamentId { get; set; }
        public int UserId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
