namespace PDEWebAPIS.InputDataModel
{
    public class AddUploadedDocumentsDTLData
    {
        public int userid { set; get; }
        public string? applicationid { set; get; }
        //public string? nabhu { set; get; }
        public UploadedDoc? docType {  set; get; }
        public docUpload? docUpload {  set; get; }
    }
    public class UploadedDoc
    {
        public string? document_code { set; get; }   
        public string? document_name {  set; get; }
    }
    public class EditAddUploadedDocumentsDTLData
    {
        public int? userid { set; get; }
        public string? applicationid { set; get; }
        public int uploaded_doc_id { set; get; }
        //public string? nabhu { set; get; }
        public UploadedDoc? docType { set; get; }
        public docUpload? docUpload { set; get; }
    }

    public class DeleteAddUploadedDocumentsDTLData
    {
        public string? applicationid { set; get; }
        public int uploaded_doc_id { set; get; }
    }

    public class docUpload
    {
        public string? docName { set; get; }
        public string? docSrc { set; get; }
    }

}
