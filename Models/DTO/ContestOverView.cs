namespace Entities.DTO
{
    public class ContestOverView
    {
        public string ContestUniqueCode { get; set; } = null!;
        public string NameContest { get; set; } = null!;
        public string Keyword { get; set; } = null!;
        public DateTime TestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TerminationDate { get; set; }
    }
}
