namespace PDEWebAPIS.InputDataModel
{
    public class AddLabelData
    {
        public int screenid { set; get; }
        public int mutationtypeid { set; get; }

        //public string? mutationtype { set; get; }
        //public string? mutationname { set; get; }
        public string? englishname { set; get; }
        public string? marathiname { set; get; }
        public string? createdby { set; get; }
    }
}
