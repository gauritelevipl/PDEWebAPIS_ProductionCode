using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class MutationMasterModel
    {
        public string? mutation_code { get; set; }
        public string? mutation_name { get; set; }
        public ApplicationTypeMaster? applicationType { get; set; }
    }
}
