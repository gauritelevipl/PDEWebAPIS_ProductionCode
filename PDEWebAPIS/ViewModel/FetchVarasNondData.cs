using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    
    public class FetchMayatDetailsData
    {
        public int? mayat_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public UserDTLForMayat? userDetails { get; set; }
        public AddressDTLForMayat? address { get; set; }
        public areaForMutationForMayat? areaForMutation { get; set; }
       // public MrutucertificateDTL? mrutuDetails { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }
    public class FetchMrutuDakhalaDetailsData
    {
        public int? mayat_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public UserDetailsForDeathCert? userDetails { get; set; }
        public docUpload? docUpload { get; set;}
        public docUpload? uploadCorrectNameDoc { set; get; }
        //public AddressDTLForMayat? address { get; set; }
        //public areaForMutationForMayat? areaForMutation { get; set; }
        //public MrutucertificateDTL? mrutuDetails { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }
    public class docUpload
    {
        public string? documentName { get; set; }
        public string? documentSrc { get; set; }
    }

    public class FetchVarasNondDetailsData
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetails? photo { get; set; }
        // public signatureDetails? signature { get; set; }
        public isMHPropertyVaras? IsMHProperty { get; set; }
        public DharakForVaras? dharak { get; set; }
        public AddressDTLForMayat? address { get; set; }

    }
}
