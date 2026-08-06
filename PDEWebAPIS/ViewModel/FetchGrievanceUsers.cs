using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchGrievanceUsers
    {
        public int guserid { get; set; }
        public string? usertype { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? division { get; set; }
        public string? mobileno { get; set; }
        public string? emailid { get; set; }
        public string? password { get; set; }
        public DateTime registerdatetime { get; set; }
        public string? webtoken { get; set; }
        public string? moiletoken { get; set; }
    }
}
