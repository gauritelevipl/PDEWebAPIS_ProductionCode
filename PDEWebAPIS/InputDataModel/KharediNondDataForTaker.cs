namespace PDEWebAPIS.InputDataModel
{
    public class KharediNondDataForTaker
    {
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public List<PropertyDataForKharediNondTaker>? giver { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public dharakDetails? dharak { get; set; }
        public AddressDTLForKharediNond? address { get; set; }

    }
    public class PropertyDataForKharediNondTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }
    public class photoDetails
    {
        public string? passportName { get; set; }
        public string? passportSrc { get; set; }

    }

    public class isMHProperty
    {
        public string? hasProperty { get; set; }
        public string? propType { get; set; }
        public TakeruserDetails? userDetails { get; set; }
    }
    public class TakeruserDetails
    {
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; } = null;
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
        public string? khataNo { get; set; }
        public string? naBhu { get; set; }
        public string? userName { get; set; }
        public string? ulpin { get; set; }
        public District? district { get; set; }
        public Taluka? taluka { get; set; }
        public VillageforKharediNond? village { get; set; }
    }
    public class District
    {
        public string? district_code { get; set; }
        public string? district_name { get; set; }
        public string? district_english_name { get; set; }
    }
    public class Taluka
    {
        public string? office_code { get; set; }
        public string? office_name { get; set; }
    }
    public class VillageforKharediNond
    {
        public string? village_code { get; set; }
        public string? village_name { get; set; }
    }
    public class Holdertype
    {
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
    }
    public class KhataType
    {
        public string? khataCode { get; set; }
        public string? khataLabel { get; set; }
    }
    public class aapakDropdown
    {
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
    }
    public class dharakDetails
    {
        public userdharakDetails? userdharak { get; set; }
        public companydharakDetails? companydharak { get; set; }
    }

    public class userdharakDetails
    {
        public string? aliceName { get; set; }
        public aapakDropdown? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public aapakRelation? aapakRelation { get; set; }
        public Gender? gender { get; set; }
        //public KhataType? khataType { get; set; }
        public Holdertype? holderType { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? landBuyArea { get; set; }
    }

    public class aapakRelation
    {
        public string? relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class Gender
    {
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
    }



    public class companydharakDetails
    {
        public Holdertype? holderType { get; set; }
        // public KhataType? khataType { get; set; }
        //public aapakDropdown? aapakDropdown { get; set; }
        //public string? aapak { get; set; }
        public string? landBuyArea { get; set; }
    }

    public class EditKharediNondDataForTaker
    {
        public int? MutationId { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public dharakDetails? dharak { get; set; }
        public AddressDTLForKharediNond? address { get; set; }

    }

}
