using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;

namespace Application.Factories.Parameters.Requests
{
    public sealed record MNGRequestProvider(
        string ClientId,
        string ClientSecret);
    
    public sealed record MNGRequestToken(
        string CustomerNumber,
        string Password,
        int    identityType = 1);

    public sealed class MNGRequestOrderCargo
    {
        public MNGRequestOrderCargo(string refId, CargoList cargo)
        {
            barcode = $"{refId} - {cargo.Id}";
            desi    = Convert.ToInt32(cargo.Volume.Desi);
            kg      = Convert.ToInt32(cargo.Volume.Weight);
            content = cargo.Name;
        }

        public string barcode { get; set; }
        public int    desi    { get; set; }
        public int    kg      { get; set; }
        public string content { get; set; }
    }

    public sealed class MNGRequestOrderMember
    {
        public MNGRequestOrderMember(Member member)
        {
            customerId           = 00000000;
            refCustomerId        = member.Id ?? "";
            cityCode             = member.Residence.City.Code;
            districtCode         = member.Residence.District.Code;
            address              = member.Residence.Address;
            email                = member.Email;
            fullName             = $"{member.Name} {member.Surname}";
            homePhoneNumber      = member.Phone;
            mobilePhoneNumber    = member.Phone;
            bussinessPhoneNumber = member.Phone;
            taxOffice            = member.TaxDepartment ?? "";
            taxNumber            = member.TaxNumber ?? "";
        }

        public int    customerId           { get; set; }
        public string refCustomerId        { get; set; }
        public int    cityCode             { get; set; }
        public int    districtCode         { get; set; }
        public string address              { get; set; }
        public string bussinessPhoneNumber { get; set; }
        public string email                { get; set; }
        public string taxOffice            { get; set; }
        public string taxNumber            { get; set; }
        public string fullName             { get; set; }
        public string homePhoneNumber      { get; set; }
        public string mobilePhoneNumber    { get; set; }
    }

    public sealed class MNGRequestOrder
    {
        public MNGRequestOrder(int Cod, decimal price, int package, int payment)
        {
            string refId = "SIP_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            referenceId     = refId;
            barcode         = refId;
            billOfLandingId = refId;

            isCod               = Cod;
            codAmount           = price;
            shipmentServiceType = 1;
            packagingType       = package;
            content             = $"{refId} numaralı sipariş";

            smsPreference1 = 1;
            smsPreference2 = 1;
            smsPreference3 = 1;

            paymentType  = payment;
            deliveryType = 1;
            description  = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} tarihinde oluşturulan sipariş";

            marketPlaceShortCode = "";
            marketPlaceSaleCode  = "";
            pudoId               = "";
        }

        public string   referenceId          { get; set; }
        public string   barcode              { get; set; }
        public string   billOfLandingId      { get; set; }
        public int      isCod                { get; set; }
        public decimal? codAmount            { get; set; }
        public int      shipmentServiceType  { get; set; }
        public int      packagingType        { get; set; }
        public string   content              { get; set; }
        public int      smsPreference1       { get; set; }
        public int      smsPreference2       { get; set; }
        public int      smsPreference3       { get; set; }
        public int      paymentType          { get; set; }
        public int      deliveryType         { get; set; }
        public string   description          { get; set; }
        public string   marketPlaceShortCode { get; set; }
        public string   marketPlaceSaleCode  { get; set; }
        public string   pudoId               { get; set; }
    }

    public sealed record MNGRequestFullOrder(
        MNGRequestOrder            order,
        List<MNGRequestOrderCargo> orderPieceList,
        MNGRequestOrderMember      shipper,
        MNGRequestOrderMember      recipient);


    public sealed record MNGRequestCancelOrder(
        string  ReferenceId,
        string? Description);
}