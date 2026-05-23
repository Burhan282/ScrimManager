namespace ScrimManagerApplication.Application.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string Organisator { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public string Format { get; set; } = string.Empty;
        public int MaxTeams { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? PrizeMoney { get; set; }
        public int ParticipatingTeams { get; set; }
    }
}