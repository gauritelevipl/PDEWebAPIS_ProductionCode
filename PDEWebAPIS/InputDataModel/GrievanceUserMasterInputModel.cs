using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.InputDataModel
{
    public class GrievanceUserMasterInputModel
    {
        public string? usertype { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? division { get; set; }
        public string? mobileno { get; set; }
        public string? emailid { get; set; }
        public string? password { get; set; }
    }

}