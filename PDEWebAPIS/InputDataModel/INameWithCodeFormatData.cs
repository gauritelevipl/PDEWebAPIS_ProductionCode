using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;

namespace PDEWebAPIS.InputDataModel
{
    public interface INameWithCodeFormatData
    {
        public class DistrictData
        {
            //public string? districtCode { set; get; }
            //public string? districtName { set; get; }
            public string? district_code { set; get; }
            public string? district_name { set; get; }
            public string? district_english_name { set; get; }
        }

        public class TalukaData
        {
            public string? office_code { get; set; }
            public string? office_name { get; set; }
        }

        public class VillageData
        {
            public string? village_code { get; set; }
            public string? village_name { get; set; }
        }

        public class Registrar
        {
            //public string? registrarCode { set; get; }
            //public string? registrarName { set; get; }
            public int sro_office_code { set; get; }
            public string? sro_office_name { set; get; }
        }

        public class HoldertypeData
        {
            public string? owner_status_code { get; set; }
            public string? owner_status_description { get; set; }
        }

        public class KhataTypeData
        {
            public string? khataCode { get; set; }
            public string? khataLabel { get; set; }
        }

        public class aapakDropdownData
        {
            public int? apk_code { get; set; }
            public string? apk_description { get; set; }
        }

        public class DistrictDataForPOATaker
        {
            public string? district_code { set; get; }
            public string? district_name { set; get; }
            public string? district_english_name { set; get; }
        }

        public class RegistrarForPOATaker
        {
            public int sro_office_code { set; get; }
            public string? sro_office_name { set; get; }
        }

    }
}
