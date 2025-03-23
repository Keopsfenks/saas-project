using Domain.Entities.WorkspaceEntities;

namespace Domain.ValueObject
{
    public sealed class Member
    {
        public string?   Id            { get; set; }
        public string    Name          { get; set; } = string.Empty;
        public string    Surname       { get; set; } = string.Empty;
        public string    Email         { get; set; } = string.Empty;
        public string    Phone         { get; set; } = string.Empty;
        public Residence Residence     { get; set; } = new Residence();
        public string?   TaxNumber     { get; set; }
        public string?   TaxDepartment { get; set; }
        public bool      IsSender      { get; set; } = false;
    }
}