using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("uploaded_documents_dtl")]
    public class UploadedDocumentsDTL
    {
        [Key, Required]
        public int uploaded_doc_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
       // public DocumentTypeMaster? documentType { get; set; }
       public string? document_type_code { set; get; }
        public string? document_type{ set; get; }
        public string? city_servey_no {  get; set; }
        public string? document_name { set; get; }
        public string? document_path { set; get; }
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        // Default value is NA. When Truti docs are uploaded by user then Value will be TRUE.
        // When user reupload the Truti that time we have to make old Truti patra flag to FALSE
        public string? truti_patra_flag { set; get; }
        public DateOnly deleteddate { set; get; }
        public DateOnly nicdate { set; get; }
    }
}
