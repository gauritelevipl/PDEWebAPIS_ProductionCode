using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchGenericDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? entry_date { get; set; }
        public string? entry_bracketed { get; set; }
        public string? owner_number { get; set; }
        public string? owner_name { get; set; }
        public string? owner_bracketed { get; set; }
        public string? nabhu { get; set; }
        public string? suffixEng { get; set; }
        public string? suffixCodeEng { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? suffixcode { get; set; }
        public string? suffix { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? actualArea { get; set; }
        public string? fullNameInMarathi { get; set; }
        public string? fullNameInEng { get; set; }
        public string? mobileNo { get; set; }
        public string? subPropNo { get; set; }

        //// Below fields are for Additional data
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForGenericGiver? userDetails { get; set; }
        public areaForMutationDTLGenericGiver? areaForMutation { get; set; }
        public AddressDTLForGenericGiver? address { get; set; }
    }
}
