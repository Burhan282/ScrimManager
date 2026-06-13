namespace ScrimManagerApplication.Application.Models
{
    public class UserTournament
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string TournamentStatus { get; set; } = string.Empty;
        public int ParticipatingTeams { get; set; }
        public int MaxTeams { get; set; }
    }
}
