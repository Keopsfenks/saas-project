using Domain.Entities.WorkspaceEntities;

namespace Application.Dtos
{
    public sealed class CustomerDto
    {
        public CustomerDto(Customer customer)
        {
            Id           = customer.Id;
            Name         = customer.Name;
            Surname      = customer.Surname;
            Email        = customer.Email;
            Phone        = customer.Phone;
            CountryCode  = customer.CountryCode;
            Address      = customer.Address;
            City         = customer.City;
            CityCode     = customer.CityCode;
            District     = customer.District;
            DistrictCode = customer.DistrictCode;
            ZipCode      = customer.ZipCode;
            TaxNumber    = customer.TaxNumber;
            TaxDepartment= customer.TaxDepartment;
            CreatedAt    = customer.CreateAt;
            UpdatedAt    = customer.UpdateAt;
        }

        public string Id       { get; set; }
        public string Name     { get; set; }
        public string Surname  { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string Email    { get; set; }
        public string Phone    { get; set; }

        public string CountryCode  { get; set; }
        public string Address      { get; set; }
        public string City         { get; set; }
        public string CityCode     { get; set; }
        public string District     { get; set; }
        public string DistrictCode { get; set; }

        public string ZipCode { get; set; }

        public string? TaxNumber     { get; set; }
        public string? TaxDepartment { get; set; }


        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}