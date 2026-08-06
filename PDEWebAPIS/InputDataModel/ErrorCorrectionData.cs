namespace PDEWebAPIS.InputDataModel
{
    public class ErrorCorrectionData
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? village_code { get; set; }
        public UserDetailsForErrorCorrection? userDetails { get; set; }
        public AddressDTLForErrorCorrection? address { get; set; }
    }

    public class UserDetailsForErrorCorrection
    {
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? subPropNo { get; set; }
        //public selectedMutationDataForErrorCorrection? selectedMutation { get; set; }
        public string? reason { get; set; }
    }

    public class selectedMutationDataForErrorCorrection
    {
        public string? var_village_code { get; set; }
        public string? var_cts_number { get; set; }
        public string? var_cts_puid { get; set; }
        public string? var_mutation_srno { get; set; }
        public string? var_entry_date { get; set; }
        public string? var_mutation_number { get; set; }
        public string? var_mutation_date { get; set; }
        public string? var_sro_office_name_marathi { get; set; }
        public string? var_sro_office_name_english { get; set; }
        public string? var_document_number { get; set; }
        public string? var_document_year { get; set; }
        public string? var_document_date { get; set; }
        public string? var_entry_details { get; set; }
        public string? var_owner_details { get; set; }
    }

    public class AddressDTLForErrorCorrection
    {
        public string? addressType { set; get; }
        public IndiaAddressForErrorCorrection? indiaAddress { get; set; }
        public ForeignAddressForErrorCorrection? foreignAddress { set; get; }
    }

    public class IndiaAddressForErrorCorrection
    {
        public string? plotNo { set; get; }
        public string? building { get; set; }
        public string? mainRoad { get; set; }
        public string? impSymbol { get; set; }
        public string? area { get; set; }
        public string? mobile { get; set; }
        public string? mobileOTP { get; set; }
        public string? pincode { get; set; }
        public string? postOfficeName { get; set; }
        public string? city { get; set; }
        public string? taluka { get; set; }
        public string? district { get; set; }
        public string? state { get; set; }
        public string? addressProofName { get; set; }
        public string? addressProofSrc { get; set; }
        //public string? signatureName { get; set; }
        //public string? signatureSrc { get; set; }
    }

    public class ForeignAddressForErrorCorrection
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }
}