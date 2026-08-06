namespace PDEWebAPIS.InputDataModel
{
    public class HibanamaWitnessInfoInputModel
    {
        public string? applicationid { get; set; }
        public string? permissionNo { get; set; }
        public string? permissionDate { get; set; }
        public int userid { get; set; }
        public HibanamaWitnessDetails? witnessDetails { get; set; }
        public AddressForHibanamaWitness? address { get; set; }
    }
    public class HibanamaWitnessDetails
    {
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? suffixcode { get; set; }
        public string? suffixCodeEng { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? aliceName { get; set; }
    }

    public class AddressForHibanamaWitness
    {
        public string? addressType { get; set; }
        public IndianAddressForHibanamaWitness? indiaAddress { get; set; }
        public ForeignAddressForHibanamaWitness? foreignAddress { get; set; }
    }
    public class IndianAddressForHibanamaWitness
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
    }
    public class ForeignAddressForHibanamaWitness
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
    }
}
