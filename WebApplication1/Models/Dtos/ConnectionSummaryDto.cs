namespace WebApplication1.Models.Dtos
{
    public class ConnectionSummaryDto
    {
        public int NumberOfRequestedConnection {  get; set; }
        public int NumberOfRequestConnection { get; set; }
        public int NumberConnectionStatusUserFollow {  get; set; }
        public int NumberConnectionStatusUserFollowed { get; set; }
        public int NumberConnectionStatusConnect { get; set; }
        public int NumberConnectionStatusUserReject { get; set; }
        public int NumberConnectionStatusUserRejected { get; set; }
        public int NumberConnectionStatusDisconnect { get; set; }
        public List<ConnectionDto> TopThreeConnection { get; set; }
    }
}
