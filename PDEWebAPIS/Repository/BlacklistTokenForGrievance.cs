using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("blacklisttokenforgrievance")]
    public class BlacklistTokenForGrievance
    {
        [Key, Required]
        public int blacklistId { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required]
        public DateTime ExpirationTime { get; set; }

        [Required]
        public DateTime createdDateTime { get; set; }
    }
}