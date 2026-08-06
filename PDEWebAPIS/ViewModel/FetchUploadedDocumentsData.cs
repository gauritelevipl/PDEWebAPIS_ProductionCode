namespace PDEWebAPIS.ViewModel
{
    public class FetchUploadedDocumentsData
    {
        public string? applicationID { set; get; }  
        public string? documentID { set; get; }
        public int? documentTypeCode { set; get; }
        public string? documentType { set; get; }
        public string? city { set; get; }
        public string? nabhuno {  set; get; }   
        public string? docName {  set; get; }
        public string? docSrc { set; get; }
    }
    public class FetchUploadedDocDataDocTypeWise
    {
        public string? applicationID { set; get; }
        public string? documentID { set; get; }
        public int? documentTypeCode { set; get; }
        public string? documentType { set; get; }
        public string? nabhuno { set; get; }
        public List<DocumentData>? doc { set; get; }
    }
    public class DocumentData
    {
        public string? docName { set; get; }
        public string? docSrc { set; get; }
    }
}
