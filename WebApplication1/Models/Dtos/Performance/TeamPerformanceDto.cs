namespace WebApplication1.Models.Dtos.Performance
{
    public class TeamPerformanceDto
    {
        public int NumberTeamLeadsCurrMonth { get; set; }
        public int NumberTeamLeadsLastMonth { get; set; }
        public int NumberTeamLeadsPercentage { get; set; }
        public int NumberTeamInvolvedCurrMonth { get; set; }
        public int NumberTeamInvolvedPercentage { get; set; }
        public int NumberTeamInvolvedLastMonth { get; set; }
    }
}
