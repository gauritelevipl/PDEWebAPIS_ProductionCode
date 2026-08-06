namespace PDEWebAPIS.InputDataModel
{
    public class BakshishPatraGiverData
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForKharediNond? userDetails { get; set; }
        public areaForMutationDTLBakshishPatra? areaForMutation { get; set; }
        public AddressDTLForKharediNond? address { get; set; }

    }


    public class areaForMutationDTLBakshishPatra
    {
        public string? isFullAreaGiven { set; get; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }

    }

    public class EditBakshishPatraGiverData
    {
        public int? MutationId { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public UserDTLForKharediNond? userDetails { get; set; }
        public areaForMutationDTLBakshishPatra? areaForMutation { get; set; }
        public AddressDTLForKharediNond? address { get; set; }

    }


}
