namespace PDEWebAPIS.InputDataModel
{
    public class AddTrutiUploadedDocumentsDTLData
    {
        public int userid { set; get; }
        public string? applicationid { set; get; }
        public string? inwardNo { set; get; }
        public UploadedDoc? docType { set; get; }
        public docUpload? docUpload { set; get; }
    }

    public class DeleteTrutiPatraUploadedDocumentsDTLData
    {
        public string? applicationid { set; get; }
        public int uploaded_doc_id { set; get; }
    }
}
