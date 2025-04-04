using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;

namespace Application.Dtos
{
    public sealed class AddressDto
    {
        public AddressDto(Address address)
        {
            Id            = address.Id;
            Name          = address.Name;
            Surname       = address.Surname;
            Email         = address.Email;
            Phone         = address.Phone;
            Residence     = address.Residence;
            TaxNumber     = address.TaxNumber;
            TaxDepartment = address.TaxDepartment;

            CreatedAt = address.CreateAt;
            UpdatedAt = address.UpdateAt;
        }


        public string    Id        { get; set; }
        public string    Name      { get; set; }
        public string    Surname   { get; set; }
        public string    Fullname  => $"{Name} {Surname}";
        public string    Email     { get; set; }
        public string    Phone     { get; set; }
        public Residence Residence { get; set; }

        public string? TaxNumber     { get; set; }
        public string? TaxDepartment { get; set; }

        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}