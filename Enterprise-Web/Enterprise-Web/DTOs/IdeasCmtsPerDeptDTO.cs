namespace Enterprise_Web.DTOs
{
    public class IdeasCmtsPerDeptDTO
    {
        public string? Name { get; set; }
        public int NumOfIdeas { get; set; }
        public int NumOfCmts { get; set; }
    }
    public class IdeasPerDeptDTO
    {
        public string? Name { get; set; }
        public int NumOfIdeas { get; set; }
    }
    public class CmtsPerDeptDTO
    {
        public string? Name { get; set; }
        public int NumOfCmts { get; set; }
    }
}
