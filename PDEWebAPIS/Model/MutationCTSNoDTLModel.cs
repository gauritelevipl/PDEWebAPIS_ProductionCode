using PDEWebAPIS.Repository;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Model
{
    public class MutationCTSNoDTLModel
    {
        public int? mutation_cts_no_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? what_is_mentioned_in_the_doc { set; get; }
        public string? village_or_peth_code { set; get; }
        public string? village_or_peth_name { set; get; }
        public string? village_lgd_code { get; set; }
        public string? village_english_name { set; get; }
        public string? zone_code { set; get; }
        public string? amount { get; set; }
        public string? mutation_modification_type { set; get; }
        public string? city_servey_no_mentioned_in_application { set; get; }
        public string? servey_no { set; get; }
        public string? selected_city_servey_no { set; get; }
        public string? lr_property_uid { set; get; }
        public string? application_income_type { set; get; }
        public string? city_servey_area_in_sq_m { set; get; }
        public string? building_name { set; get; }
        public int? floor_type { set; get; }
        public string? floor_desc { set; get; }
        public int? floor_order_by { set; get; }    

        public string? floor_no { set; get; }
        public int? unit_code_156 { set; get; }
        public string? unit_name_156 { set; get; }
        public string? unit_no { set; get; }
        public string? buildup_area_in_sq_m { set; get; }
        public string? carpet_area_in_sq_m { set; get; }
        public string? terrace_area_in_sq_m { set; get; }
        public string? parking_no { set; get; }
        public string? parking_area_in_sq_m { set; get; }
        public string? shares_in_percent { set; get; }
        public string? nic_flat_details { set; get; }
        public string? flat_bulit_up_area {  set; get; }
        public string? sub_property_no { set; get; }
        public DateTime createddatetime { set; get; }
    }
}
