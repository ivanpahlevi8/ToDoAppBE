namespace WebApplication1.Models.Dtos
{
    public class TeamSummaryDto
    {
        public int TeamNumberInvolved { get; set; }
        public int TeamNumberLead {  get; set; }
        public List<TeamDto> TopThreeTeamAsLeader { get; set; }
        public List<TeamDto> TopThreeTeamAsMember { get; set; }
    }
}
