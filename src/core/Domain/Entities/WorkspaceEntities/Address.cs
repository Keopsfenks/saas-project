using Domain.Abstractions;
using Domain.ValueObject;

namespace Domain.Entities.WorkspaceEntities
{

    public sealed class Location
    {
        public string Name { get; set; } = string.Empty;
        public int    Code { get; set; } = 0;
    }

    public sealed class Residence
    {
        public string CountryCode { get; set; } = string.Empty;
        public string Address     { get; set; } = string.Empty;

        public Location City     { get; set; } = new();
        public Location District { get; set; } = new();

        public string? ZipCode { get; set; } = string.Empty;

    }

    public sealed class Address : WorkspaceEntity
    {
        public string    Name          { get; set; } = string.Empty;
        public string    Surname       { get; set; } = string.Empty;
        public string    FullName      => $"{Name} {Surname}";
        public string    Email         { get; set; } = string.Empty;
        public string    Phone         { get; set; } = string.Empty;
        public Residence Residence     { get; set; } = new();
        public string?   TaxNumber     { get; set; }
        public string?   TaxDepartment { get; set; }
    }
}