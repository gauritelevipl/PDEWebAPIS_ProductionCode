namespace PDEWebAPIS.Model
{
    public interface IUserData
    {
        public class PhotoData
        {
            public string? passportName { set; get; }
            public string? passportSrc { set; get; }
        }

        public class IsMHPropertyData
        {
            public string? hasProperty { set; get; }
            public string? propType { set; get; }
            public UserDetailsData? userDetails { set; get; }
        }

        public class UserDetailsData
        {
            public string? khataNo { set; get; }
            public string? naBhu { set; get; }
            public string? ulpin { set; get; }
            public string? userName { set; get; }
            public string? district { set; get; }
            public string? taluka { set; get; }
            public string? village { set; get; }
            public string? suffix { set; get; }
            public string? firstName { set; get; }
            public string? middleName { set; get; }
            public string? lastName { set; get; }
            public string? suffixEng { set; get; }
            public string? firstNameEng { set; get; }
            public string? middleNameEng { set; get; }
            public string? lastNameEng { set; get; }
            public string? companyName { set; get; }
            public string? companyNameEng { set; get; }

        }

        public class PersonAddressData
        {
            public string? addressType { set; get; }
            public ForeignAddressData? foreignAddress { set; get; }
            public IndiaAddressData? indiaAddress { set; get; }
        }

        public class ForeignAddressData
        {
            public string? address { set; get; }
            public string? mobile { set; get; }
            public string? email { set; get; }
            public string? emailOTP { set; get; }
            public string? singnatureName { set; get; }
            public string? signatureSrc { set; get; }
        }

        public class IndiaAddressData
        {
            public string? state { set; get; }
            public string? district { set; get; }
            public string? city { set; get; }
            public string? taluka { set; get; }
            public string? plotNo { set; get; }
            public string? building { set; get; }
            public string? mainRoad { set; get; }
            public string? impSymbol { set; get; }
            public string? area { set; get; }
            public string? pincode { set; get; }
            public string? postOfficeName { set; get; }
            public string? addressProofName { set; get; }
            public string? addressProofSrc { set; get; }
            public string? mobile { set; get; }
            public string? mobileOTP { set; get; }
            public string? email { set; get; }
            public string? emailOTP { set; get; }
            public string? securityKey { set; get; }
            public string? singnatureName { set; get; }
            public string? signatureSrc { set; get; }
        }
    }
}
