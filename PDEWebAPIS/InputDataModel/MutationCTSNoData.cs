using PDEWebAPIS.Repository;

namespace PDEWebAPIS.InputDataModel
{
    public class MutationCTSNoData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? fetchedBuildupArea { set; get; }
        public FlatDetails? flatDetails { set; get; }
        public string? inDast { set; get; }
        public string? milkat { get; set; }
        public string? naBhu { set; get; }
        public string? nabhuNo { set; get; }
        public string? namud { set; get; }
        public string? surveyNo { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? cityServeyAreaInSqm { set; get; }
        public Village? village { set; get; }
        public string? nic_flat_details { set; get; }
        public string? flatBuiltUpArea {  set; get; }
        public string? subPropNo { set; get; }
    }
    public class FlatDetails
    {
        public string? buildingName { set; get; }
        public string? buildupArea { set; get; }
        public string? carpetArea { set; get; }
        public string? floorNo { set; get; }
        public FloorType? floorType { set; get; }
        public string? hissa { set; get; }
        public string? parkingArea { set; get; }
        public string? parkingNo { set; get; }
        public string? taraceArea { set; get; }
        public string? unitNo { set; get; }
        public unitType? unitType { set; get; }
    }

    public class FloorType
    {
        public int? floor_type { set; get; }
        public string? floor_desc { get; set; }
        public int? floor_order_by { set; get; }
    }
    public class unitType
    {
        public int? unit_code_156 { set; get; }
        public string? unit_name_156 { get; set; }
    }

    public class Village
    {
        public string? village_lgd_code { get; set; }
        public string? village_name { set; get; }
        public string? village_code { set; get; }
        public string? village_english_name { get; set; }
        public string? zone_code { set; get; }
        public string? amount { set; get; }
    }

    public class EditMutationCTSNoData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int mutation_cts_no_id { get; set; }
        public string? fetchedBuildupArea { set; get; }
        public FlatDetails? flatDetails { set; get; }
        public string? inDast { set; get; }
        public string? milkat { get; set; }
        public string? naBhu { set; get; }
        public string? nabhuNo { set; get; }
        public string? namud { set; get; }
        public string? surveyNo { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? cityServeyAreaInSqm { set; get; }
        public Village? village { set; get; }
        public string? nic_flat_details { set; get; }
        public string? sub_property_no { set; get; }
    }

    public class DeleteMutationCTSNoData
    {
        public string? applicationid { get; set; }
        public int mutation_cts_no_id { get; set; }
    }
}
