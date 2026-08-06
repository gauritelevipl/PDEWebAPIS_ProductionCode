using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
namespace PDEWebAPIS.InputDataModel
{
    public class DastInformationData : INameWithCodeFormatData
    {
        public int dast_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? dastNabhu { get; set; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public string? dastType { get; set; }
        public Division? division { get; set; }
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
        public string? remarks { set; get; }
        public bool? isDastVarified { set; get; }
        public string? verifiedDastData { get; set; }
    }
    public class Division
    {
        public int? digcode { get; set; }
        public string? dig { get; set; }
    }
    public class DastDistrict
    {
        public int? jdrcode { get; set; }
        public string? jdr { get; set; }
    }
    public class Dastregsiter
    {
        public int? srocode { get; set; }
        public string? sro { get; set; }
    }
    public class EditDastInformationData
    {
        public int dastid { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? dastNabhu { get; set; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public string? dastType { get; set; }
        public Division? division { get; set; }
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
        public string? remarks { set; get; }
        public bool? isDastVarified { set; get; }
        public string? verifiedDastData { get; set; }
    }

    public class DeleteDastInformationData
    {
        public int dastid { get; set; }
        public string? applicationid { get; set; }
    }
}
