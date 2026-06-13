namespace ScrimManagerApplication.Application.Models.DTOModels
{
    public class CreateTournamentDTO
    {
        public string Naam { get; set; } = string.Empty;

        public string Organisator { get; set; } = string.Empty;

        public DateTime? SelectedDate { get; set; }

        public TimeSpan? SelectedTime { get; set; }

        public string Format { get; set; } = string.Empty;

        public int MaxTeams { get; set; }

        public decimal? PrizeMoney { get; set; }

        public string? Description { get; set; }
    }
}
