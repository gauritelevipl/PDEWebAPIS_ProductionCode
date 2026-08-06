using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class UploadedDocumentsDTLModel
    {
        public int uploaded_doc_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
       // public DocumentTypeMaster? documentType { get; set; }
       public string? documentTypeCode { set; get; }
        public string? documentType {  set; get; }
        public string? city_servey_no { get; set; }
        public string? document_name { set; get; }
        public string? document_path { set; get; }
    }
}
