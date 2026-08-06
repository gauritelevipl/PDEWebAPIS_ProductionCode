using PDEWebAPIS.Repository;
using static PDEWebAPIS.InputDataModel.IUserData;

namespace PDEWebAPIS.InputDataModel
{
    public class CreateApplicantData : IUserData
    {
        public int usertype_code { set; get; }
        public string? usertype { set; get; }
        public PhotoData? photo { set; get; }
        public IsMHPropertyData? isMHProperty { set; get; }
        public PersonAddressData? address { set; get; }
        public string? applicationId { set; get; }
        public int userId { set; get; }
    }

    public class EditApplicantData : IUserData
    {
        public int applicantid { set; get; }
        public int usertype_code { set; get; }
        public string? usertype { set; get; }
        public PhotoData? photo { set; get; }
        public IsMHPropertyData? isMHProperty { set; get; }
        public PersonAddressData? address { set; get; }
        public string? applicationId { set; get; }
        public int userId { set; get; }
    }

    public class DeleteApplicant
    {
        public string? applicationid { set; get; }
        public int applicantid { get; set; }
    }
}
