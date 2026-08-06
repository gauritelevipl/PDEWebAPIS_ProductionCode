namespace PDEWebAPIS.ViewModel
{
    public class FetchMutationName
    {
        public int? mutation_dtl_id { get; set; }
        public string? fullNameinMarathi { get; set; }
        public string? fullNameinEng { get; set; }
        /*public string? Middlename { get; set; }
        public string? Lastname { get; set; }

        // Fullname property combines Firstname, Middlename, and Lastname
        public string Fullname
        {
            get
            {
                // Combine parts, skipping empty/null names
                return string.Join(" ", new[] { Firstname, Middlename, Lastname }
                                        .Where(name => !string.IsNullOrWhiteSpace(name)));
            }
        }*/
    }
}
