using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Model
{
    public class BlacklistTokenForGrievanceModel
    {
        public int blacklistId { get; set; }
        public string? Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}