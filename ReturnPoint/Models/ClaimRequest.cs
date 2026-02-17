namespace ReturnPoint.Models
{
    public class ClaimRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ClaimantEmail { get; set; }
        public string ClaimantName { get; set; }
        public string ClaimantGradeSection { get; set; }
        public string ImagePath { get; set; }
        public string ImageFileName { get; set; }
        public DateTime DateClaimed { get; set; } = DateTime.Now;
        public string Status { get; set; } = "pending"; // pending, confirmed, rejected
        public string ConfirmedBy { get; set; } = "";
        public DateTime DateConfirmed { get; set; } = DateTime.MinValue;
        public string Notes { get; set; } = "";
    }
}
