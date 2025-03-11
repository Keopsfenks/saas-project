using Domain.Entities.WorkspaceEntities;

namespace Application.Dtos
{
    public sealed class ShipperDto
    {
        public ShipperDto(Shipper shipper)
        {
            Id           = shipper.Id;
            Name         = shipper.Name;
            Surname      = shipper.Surname;
            Email        = shipper.Email;
            Phone        = shipper.Phone;
            CountryCode  = shipper.CountryCode;
            Address      = shipper.Address;
            City         = shipper.City;
            CityCode     = shipper.CityCode;
            District     = shipper.District;
            DistrictCode = shipper.DistrictCode;
            ZipCode      = shipper.ZipCode;
            TaxNumber    = shipper.TaxNumber;
            TaxDepartment= shipper.TaxDepartment;
            CreatedAt    = shipper.CreateAt;
            UpdatedAt    = shipper.UpdateAt;
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