using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;

namespace PDEWebAPIS.InputDataModel.MrutyuPatra
{
    public class MrutyuPatraDataForGiver : INameWithCodeFormatData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDataForMrutyuPatraGiver? userDetails { set; get; }
        public AreaForMutationForMrutyuPatraGiver? areaForMutation { get; set; }
        public docUpload? docUpload { set; get; }
        public AddressDataForMrutyuPatraGiver? address { set; get; }
        public ProbetData? probet { get; set; }
    }

    public class ProbetData
    {
        public string? isProbet { get; set; }
        public docUpload? docUploadProbet { set; get; }
    }

    public class AddressDataForMrutyuPatraGiver
    {
        public string? addressType { set; get; }
        public AddressForForeignForMrutyuPatraGiver? foreignAddress { set; get; }
        public AddressForIndiaForMrutyuPatraGiver? indiaAddress { set; get; }
    }

    public class AddressForIndiaForMrutyuPatraGiver
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
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class AddressForForeignForMrutyuPatraGiver
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class UserDataForMrutyuPatraGiver
    {
        public string? suffix { set; get; }
        public string? suffixcode { set; get; }
        public string? firstName { set; get; }
        public string? middleName { set; get; }
        public string? lastName { set; get; }
        public string? suffixEng { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? aliceName { set; get; }
        public string? holderType { get; set; }
        public string? dob { set; get; }
        public string? motherName { set; get; }
        public string? motherNameEng { set; get; }
        public string? userName { set; get; }
        public string? nabhu { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? milkat { set; get; }
        public string? namud { set; get; }
        public string? subPropNo { set; get; }
        public deathCertificateOfficeDropdownForMrutyuPatra? deathCertificateIssueOfficeDropdown { get; set; }
        public string? deathCertificateNo { get; set; }
        public string? dateOfDeathCertificate { get; set; }
        public string? dateOfDeath { set; get; }
    }

    public class deathCertificateOfficeDropdownForMrutyuPatra
    {
        public string? certificate_authority_code { set; get; }
        public string? certificate_authority_name { set; get; }
    }

    public class AreaForMutationForMrutyuPatraGiver
    {
        public string? isFullAreaGiven { set; get; }
        public string? actualArea { set; get; }
        public string? mutationArea { set; get; }
        public string? actualDharakArea { set; get; }
    }


    public class EditMrutyuPatraDataForGiver : INameWithCodeFormatData
    {
        public int? MutationId { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public UserDataForMrutyuPatraGiver? userDetails { set; get; }
        public AreaForMutationForMrutyuPatraGiver? areaForMutation { get; set; }
        public AddressDataForMrutyuPatraGiver? address { set; get; }
    }
}
