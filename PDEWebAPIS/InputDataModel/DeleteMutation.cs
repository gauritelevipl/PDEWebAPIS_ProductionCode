namespace PDEWebAPIS.InputDataModel
{
    public class DeleteMutation
    {
        public int? MutationId { get; set; }
        public string? applicationid { set; get; }
    }

    public class DeleteMayat
    {
        public int? MayatId { get; set; }
        public string? applicationid { get; set; }
    }

    public class DeleteBhadepattaInfo
    {
        public string? applicationid { get; set; }
        public int info_id { set; get; }
    }
}

