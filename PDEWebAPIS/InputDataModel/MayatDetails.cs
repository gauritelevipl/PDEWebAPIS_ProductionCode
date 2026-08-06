using PDEWebAPIS.Repository;

namespace PDEWebAPIS.InputDataModel
{
    public class MayatDetails
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForMayat? userDetails { get; set; }
        public AddressDTLForMayat? address { get; set; }
        public areaForMutationForMayat? areaForMutation { get; set; }
        //  public MrutucertificateDTL? mrutuDetails { get; set; }
    }

    public class MrutuDakhalaDetails
    {
        public int? mayat_id { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public UserDetailsForDeathCert? userDetails { get; set; }
        public docUpload? docUpload { set; get; }
        public docUpload? uploadCorrectNameDoc { set; get; }
    }

    public class UserDTLForMayat
    {
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? aliceName { get; set; }
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? subPropNo { get; set; }
        //public string? namud_milkat { get; set; }

    }

    public class deathCertificateOfficeDropdown
    {
        public string? certificate_authority_code { set; get; }
        public string? certificate_authority_name { set; get; }

    }

    public class areaForMutationForMayat
    {
        public string? actualArea { get; set; }
    }
    public class AddressDTLForMayat
    {
        public string? addressType { set; get; }
        public IndiaAddressForKharediNond? indiaAddress { get; set; }
        public AddressForForeign? foreignAddress { set; get; }
    }

    public class UserDetailsForDeathCert
    {
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        //public string? aliceName { get; set; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }
        public string? dateOfDeath { get; set; }
        //public string? deathCertificateOfficeName { get; set; }
        public deathCertificateOfficeDropdown? deathCertificateIssueOfficeDropdown { get; set; }
        //public string? deathCertificateTaker { get; set; }
        public string? deathCertificateNo { get; set; }
        public string? dateOfDeathCertificate { get; set; }
        public string? isNameSame { get; set; }
        public string? reason { get; set; }
    }


    public class MrutucertificateDTL
    {
        public string? dateOfDeath { get; set; }
        public string? mrutucertificate_issued_ofc { get; set; }
        public string? ofc_name { get; set; }
        public string? deathCertificateTaker { get; set; }
        public string? deathCertificateNo { get; set; }
        public string? dateOfDeathCertificate { get; set; }
        public string? mruticertificatename { get; set; }
        public string? mrutucertificateSrc { get; set; }
    }

    public class EditMayatDetails
    {
        public int? MayatId { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForMayat? userDetails { get; set; }
        public AddressDTLForMayat? address { get; set; }
        public areaForMutationForMayat? areaForMutation { get; set; }
        //  public MrutucertificateDTL? mrutuDetails { get; set; }

    }
}
