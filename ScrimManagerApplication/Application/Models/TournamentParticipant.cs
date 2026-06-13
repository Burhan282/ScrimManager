public class TournamentParticipant
{
    public int Id { get; set; }
    public int TournamentId { get; set; }

    public int? UserId { get; set; }
    public int? TeamId { get; set; }

    public string? EntryName { get; set; }
}