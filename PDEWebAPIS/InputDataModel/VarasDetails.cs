using PDEWebAPIS.Repository;

namespace PDEWebAPIS.InputDataModel
{
    public class VarasDetails
    {
        // public int? mayat_id { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        // public signatureDetails? signature { get; set; }
        public isMHPropertyVaras? IsMHProperty { get; set; }
        public DharakForVaras? dharak { get; set; }
        public AddressDTLForMayat? address { get; set; }
    }

    public class isMHPropertyVaras {
        public string? hasProperty {get;set;}
        public string? propType { get; set; }
        public UserDTLForVaras? userDetails { get; set; }

    }
    public class HolderTypeVaras
    {
        public string? account_type_code { get; set; }
        public string? account_type_description { get; set; }
    }
    public class DharakForVaras
    {
        public string? aliceName { get; set; }
        public Gender? gender { get; set; }
        public HolderTypeVaras? holderType { get; set; } = null;
        public aapakDropdown? aapakDropdown { get; set; }
        public deadRelation? deadRelation { get; set; }
        public string? aapak {  get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? landBuyArea { get; set; }
        public aapakRelation? aapakRelation { get; set; }
    }
    public class deadRelation
    {
        public int relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class UserDTLForVaras
    {
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }    
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? khataNo { set; get; }
        public string? naBhu { set; get; }
        public string? ulpin { set; get; }
        public string? userName { get; set; }
        public District? district { set; get; }
        public Taluka? taluka { set; get; }
        public VillageforKharediNond? village { set; get; }
       
        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
    }

    public class EditVarasDetails
    {
        public int? MutationId { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        // public signatureDetails? signature { get; set; }
        public isMHPropertyVaras? IsMHProperty { get; set; }
        public DharakForVaras? dharak { get; set; }
        public AddressDTLForMayat? address { get; set; }
    }
}

