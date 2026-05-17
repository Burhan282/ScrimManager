using System.ComponentModel.DataAnnotations.Schema;

namespace ScrimManager.Domain
{
    [Table("tournament")]
    public class Tournament
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Naam { get; set; } = string.Empty;

        [Column("organizer")]
        public string Organisator { get; set; } = string.Empty;

        [Column("date")]
        public DateTime Datum { get; set; }

        [Column("game_format")]
        public string Format { get; set; } = string.Empty;

        [Column("max_teams")]
        public int MaxTeams { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("prize_money")]
        public decimal? PrizeMoney { get; set; }

        [Column("participating_teams")]
        public int ParticipatingTeams { get; set; }
    }
}