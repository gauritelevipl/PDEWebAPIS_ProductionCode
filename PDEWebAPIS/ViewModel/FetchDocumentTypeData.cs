namespace PDEWebAPIS.ViewModel
{
    public class FetchDocumentTypeData
    {
        public string? mutationType { set; get; }
        public string? mutationTypeHDR { set; get; }
        public List<DocumentTypeData>? documentTypeDataList { set; get; }
    }
    public class DocumentTypeData
    {
        public string? documentTypeCode { set; get; }
        public string? documentType { set; get; }
        //public string? mutation_description { set; get; }
    }
}
