using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchNavatBadalData
    {
        public int name_change_id { get; set; }
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public int userid { get; set; }
        public UserDetailsForNameChange? userDetails { get; set; }
        public NameChangeDetails? nameChange { get; set; }
        public List<SelectedUserDTLsForNameChange>? selectedUserDetails { get; set; }
        public UpdatedUserDetailsForNameChange? updatedUserDetails { get; set; }
        public AddressDTLForNameChange? address { get; set; }
    }
}
