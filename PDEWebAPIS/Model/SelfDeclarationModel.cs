using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class SelfDeclarationModel
    {
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? self_declaration_doc_name { set; get; }
        public string? self_declaration_doc_path { set; get; }
    }
}
