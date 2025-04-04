using Application.Features.Commands.Shipments.v1;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Factories.Parameters.Requests
{
    public static class MNGRequest
    {
        public sealed record Provider(
            string ClientId,
            string ClientSecret);

        public sealed record APIToken(
            string CustomerNumber,
            string Password,
            int    identityType = 1);


        public sealed class APICreateOrder
        {
            public record Order(
                string   referenceId,
                string   barcode,
                string   billOfLandingId,
                int      isCOD,
                decimal? codAmount,
                int      shipmentServiceType,
                int      packagingType,
                string   content,
                int      smsPreference1,
                int      smsPreference2,
                int      smsPreference3,
                int      paymentType,
                int      deliveryType,
                string   description,
                string   marketPlaceShortCode,
                string   marketPlaceSaleCode,
                string   pudoId);

            public record Cargo(
                string barcode,
                int    desi,
                int    kg,
                string content);

            public record OrderMember
            {
                public string? customerId           { get; init; }
                public string  refCustomerId        { get; init; }
                public int     cityCode             { get; init; }
                public int     districtCode         { get; init; }
                public string  address              { get; init; }
                public string  bussinessPhoneNumber { get; init; }
                public string  email                { get; init; }
                public string  taxOffice            { get; init; }
                public string  taxNumber            { get; init; }
                public string  fullName             { get; init; }
                public string  homePhoneNumber      { get; init; }
                public string  mobilePhoneNumber    { get; init; }
            };

            public Order       order          { get; set; }
            public Cargo orderPieceList { get; set; }
            public OrderMember recipient      { get; set; }

            public APICreateOrder(CreateShipmentRequest request)
            {
                int cod, package, payment;

                cod = CodEnum.FromValue(request.Dispatch.IsCod) switch
                {
                    var codEnum when codEnum == CodEnum.COD     => 1,
                    var codEnum when codEnum == CodEnum.NOT_COD => 0,
                    _                                           => 0
                };
                package = PackagingTypeEnum.FromValue(request.Dispatch.PackagingType) switch
                {
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.File         => 1,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Mini_Package => 2,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Package      => 3,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Box          => 4,
                    _                                                                              => 4
                };
                payment = PaymentTypeEnum.FromValue(request.Dispatch.PaymentType) switch
                {
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Sender     => 1,
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Receiver   => 2,
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.ThirdParty => 3,
                    _                                                                      => 1
                };
                string refId     = ParametersFactory.CreateId("SHIP");
                string waybillId = ParametersFactory.CreateNumber().ToString();

                order = new Order(refId, refId, waybillId, cod, request.Dispatch.CodPrice, 1, package,
                                  $"{refId} numaralı sipariş", 1, 1, 1, payment, 1,
                                  $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} tarihinde oluşturulan sipariş", "", "", "");
                orderPieceList = new Cargo($"{refId}: {request.Cargo.Id}", Convert.ToInt32(request.Cargo.Volume.Desi),
                                           Convert.ToInt32(request.Cargo.Volume.Weight),
                                           request.Cargo.Name);
                recipient = new()
                            {
                                customerId           = null,
                                refCustomerId        = request.Recipient.Id ?? "",
                                cityCode             = request.Recipient.Residence.City.Code,
                                districtCode         = request.Recipient.Residence.District.Code,
                                address              = request.Recipient.Residence.Address,
                                bussinessPhoneNumber = request.Recipient.Phone,
                                email                = request.Recipient.Email,
                                taxOffice            = request.Recipient.TaxDepartment ?? "",
                                taxNumber            = request.Recipient.TaxNumber     ?? "",
                                fullName             = $"{request.Recipient.Name} {request.Recipient.Surname}",
                                homePhoneNumber      = request.Recipient.Phone,
                                mobilePhoneNumber    = request.Recipient.Phone
                            };

            }

        }
        public sealed class APIUpdateOrder
        {
            public record Cargo(
                string barcode,
                int    desi,
                int    kg,
                string content);

            public  string      referenceId     { get; set; }
            public  int         isCOD           { get; set; }
            public  int         codAmount       { get; set; }
            private Cargo orderPieceList  { get; set; }

            public APIUpdateOrder(Shipment shipment)
            {
                int cod = CodEnum.FromValue(shipment.Dispatch.IsCod) switch
                {
                    var codEnum when codEnum == CodEnum.COD     => 1,
                    var codEnum when codEnum == CodEnum.NOT_COD => 0,
                    _                                           => 0
                };

                referenceId     = shipment.CargoId;
                isCOD           = cod;
                codAmount       = Convert.ToInt32(shipment.Dispatch.CodPrice);
                orderPieceList = new Cargo($"{shipment.CargoId}: {shipment.Cargo.Id}", Convert.ToInt32(shipment.Cargo.Volume.Desi),
                                           Convert.ToInt32(shipment.Cargo.Volume.Weight),
                                           shipment.Cargo.Name);
            }

        }

        public sealed class APICreateBarcode
        {
            public record Cargo(
                string barcode,
                int    desi,
                int    kg,
                string content);

            public string      referenceId    { get; set; }
            public Cargo orderPieceList { get; set; }

            public APICreateBarcode(Shipment shipment)
            {
                referenceId    = shipment.CargoId;
                orderPieceList = new Cargo($"{shipment.CargoId}: {shipment.Cargo.Id}", Convert.ToInt32(shipment.Cargo.Volume.Desi),
                                           Convert.ToInt32(shipment.Cargo.Volume.Weight),
                                           shipment.Cargo.Name);
            }
        };
    }
}