using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchKharediNondDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForKharediNond? userDetails { get; set; }
        public areaForMutationDTLKarediNond? areaForMutation { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }    
    
    public class FetchKharediNondDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int usertype_code { set; get; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetails? photo {  get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public dharakDetails? dharak { get; set; } = null;
        public AddressDTLForKharediNond? address { get; set; }
    }

    public class FetchBakshishPatraDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForKharediNond? userDetails { get; set; }
        public areaForMutationDTLBakshishPatra? areaForMutation { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }

    public class FetchBakshishPatraDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int usertype_code { set; get; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public BakshishPatradharakDetails? dharak { get; set; } = null;
        public AddressDTLForKharediNond? address { get; set; }
    }
}
