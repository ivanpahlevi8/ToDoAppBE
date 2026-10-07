namespace WebApplication1.Models.Dtos
{
    public class ProjectSummaryDto
    {
        public int ProjectInvolvedNumber { get; set; }
        public int ProjectLeadNumber { get; set; }
        public int ProjectMemberNumber { get; set; }
        public List<ProjectDto> TopThreeProjectAsMember { get; set; }
        public List<ProjectDto> TopThreeProjectAsLeader {  get; set; }
    }
}
