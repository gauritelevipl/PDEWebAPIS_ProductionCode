namespace PDEWebAPIS.InputDataModel
{
    public class BakshishPatraTakerData
    {
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public List<PropertyDataForBakshishPatraTaker>? giver { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public BakshishPatradharakDetails? dharak { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
    }

    public class PropertyDataForBakshishPatraTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }
    public class BakshishPatradharakDetails
    {
        public BakshishPatrauserdharakDetails? userdharak { get; set; }
        public BakshishPatracompanydharakDetails? companydharak { get; set; }
    }
    public class BakshishPatrauserdharakDetails
    {
        public string? aliceName { get; set; }
        public Gender? gender { get; set; }
       // public KhataType? khataType { get; set; }
        public Holdertype? holderType { get; set; }
        public aapakDropdown? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public aapakRelation? aapakRelation { get; set; }
        public string? dob { get; set; }
       // public string? actualArea { get; set; }
        public string? giftArea { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
    }

    public class BakshishPatracompanydharakDetails
    {
        public Holdertype? holderType { get; set; }
        //public KhataType? khataType { get; set; }
       // public aapakDropdown? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public string? landBuyArea { get; set; }
        public string? giftArea { get; set; }
    }

    public class EditBakshishPatraTakerData
    {
        public int? MutationId { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public BakshishPatradharakDetails? dharak { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
    }
}
