namespace PDEWebAPIS.InputDataModel
{
    public class HakkaSodTakerData
    {
        public string? applicationid { get; set; }
        public string? ctsNo { set; get; }
        public List<PropertyDataForHakkaSodTaker>? giver { get; set; }
        public string? village_code { set; get; }
        
        public string? mutationSroNo { set; get; }
        public string? ownerNo { set; get; }
        public int userid { get; set; }
        public userDetailsHakkaSod? userDetails { get; set; }
        public AddressDTLForKharediNond?  address { get; set; }  
    }
    public class PropertyDataForHakkaSodTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }
    public class userDetailsHakkaSod
    {
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? aliceName { get; set; }
        public Holdertype? holderType { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? mutationArea { get; set; }
        public string? benefitAmount { get; set; }
        public string? userName { get; set; }
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get;set; }
        public string? milkat { get; set; }
        public string? namud { get;set; }
        public string? subPropNo { set; get; }
    }

    public class EditHakkaSodTakerData
    {
        public int? MutationId { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public userDetailsHakkaSod? userDetails { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
    }
}
