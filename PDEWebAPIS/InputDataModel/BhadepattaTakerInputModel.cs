namespace PDEWebAPIS.InputDataModel
{
    public class BhadepattaTakerInputModel
    {
        public string? usertype { get; set; }
        public int usertype_code { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public dharakDetails? dharak { get; set; }
        public Address? address { get; set; }
        public List<PropertyDataForBhadepattaTaker>? giver { get; set; }

    }
    public class PropertyDataForBhadepattaTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }
}
