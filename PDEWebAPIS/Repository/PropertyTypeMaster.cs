using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace PDEWebAPIS.Repository
{
    [Table("propertytypemaster")]
    public class PropertyTypeMaster
    {
        //data annotations.
        [Key, Required]
        public int propertytypeid { get; set; }
        [Required]
        public string propertytype { set; get; }=string.Empty;
    }
}
