using static PDEWebAPIS.InputDataModel.IUserData;
namespace PDEWebAPIS.InputDataModel
{
    public class CreateUserData : IUserData
    {
        public string? usertype { set; get; }
        public int usertype_code { set; get; }
        public PhotoData? photo { set; get; }
        public IsMHPropertyData? isMHProperty { set; get; }
        public PersonAddressData? address { set; get; }
    }

}
