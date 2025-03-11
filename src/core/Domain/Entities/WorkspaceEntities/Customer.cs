using Domain.Abstractions;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Customer : WorkspaceEntity
    {
        public string Name     { get; set; } = string.Empty;
        public string Surname  { get; set; } = string.Empty;
        public string FullName => $"{Name} {Surname}";
        public string Email    { get; set; } = string.Empty;
        public string Phone    { get; set; } = string.Empty;

        public string CountryCode  { get; set; } = string.Empty;
        public string Address      { get; set; } = string.Empty;
        public string City         { get; set; } = string.Empty;
        public string CityCode     { get; set; } = string.Empty;
        public string District     { get; set; } = string.Empty;
        public string DistrictCode { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string? TaxNumber     { get; set; } = null;
        public string? TaxDepartment { get; set; } = null;
    }
}