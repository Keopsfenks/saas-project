using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Application.Factories.Parameters.Requests
{
    public static class ARASRequest
    {
        public static class APIConfirmOrder
        {
            public static string Request(Provider provider, Shipment shipment)
            {
                int cod = CodEnum.FromValue(shipment.Dispatch.IsCod) switch
                {
                    var codEnum when codEnum == CodEnum.COD => 1,
                    var codEnum when codEnum == CodEnum.NOT_COD => 0,
                    _ => 0
                };

                int package = PackagingTypeEnum.FromValue(shipment.Dispatch.PackagingType) switch
                {
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.File => 1,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Mini_Package => 2,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Package => 3,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Box => 4,
                    _ => 4
                };

                int payment = PaymentTypeEnum.FromValue(shipment.Dispatch.PaymentType) switch
                {
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Sender => 1,
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Receiver => 2,
                    _ => 1
                };

                StringBuilder request = new();

                request.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>");
                request.Append($@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
                request.Append($@"  <soap:Body>");
                request.Append($@"    <SetOrder xmlns=""http://tempuri.org/"">");
                request.Append($@"      <orderInfo>");

                request.Append($@"<Order>");
                request.Append($@"<UserName>{provider.Username}</UserName>");
                request.Append($@"<Password>{provider.Password}</Password>");
                request.Append($@"<TradingWaybillNumber>{shipment.WaybillId}</TradingWaybillNumber>");
                request.Append($@"<InvoiceNumber>{shipment.InvoiceId}</InvoiceNumber>");
                request.Append($@"<IntegrationCode>{shipment.CargoId}</IntegrationCode>");
                request.Append($@"<ReceiverName>{shipment.Recipient.Name} {shipment.Recipient.Surname}</ReceiverName>");
                request.Append($@"<ReceiverAddress>{shipment.Recipient.Residence.Address}</ReceiverAddress>");
                request.Append($@"<ReceiverPhone1>{shipment.Recipient.Phone}</ReceiverPhone1>");
                request.Append($@"<ReceiverCityName>{shipment.Recipient.Residence.City.Name}</ReceiverCityName>");
                request.Append($@"<ReceiverTownName>{shipment.Recipient.Residence.District.Name}</ReceiverTownName>");
                request.Append($@"<VolumetricWeight>{shipment.Cargo.Volume.Desi}</VolumetricWeight>");
                request.Append($@"<Weight>{shipment.Cargo.Volume.Weight}</ReceiverTownName>");
                request.Append($@"<PieceCount>{shipment.Cargo.Items?.Count ?? 1}</PieceCount>");
                if (cod == CodEnum.COD)
                {
                    request.Append($@"<IsCod>{cod}</IsCod>");
                    request.Append($@"<CodAmount>{shipment.Dispatch.CodPrice}</CodAmount>");
                    request.Append($@"<CodCollectionType>0</CodCollectionType>");
                    request.Append($@"<CodBillingType>0</CodBillingType>");
                }
                request.Append($@"<Description>{shipment.Description}</Description>");
                request.Append($@"<TaxNumber>{shipment.Recipient.TaxNumber}</TaxNumber>");
                request.Append($@"<TaxOffice>{shipment.Recipient.TaxDepartment}</TaxOffice>");
                request.Append($@"<CityCode>{shipment.Recipient.TaxNumber}</CityCode>");
                request.Append($@"<TaxNumber>{shipment.Recipient.Residence.City.Code}</TaxNumber>");
                request.Append($@"<TownCode>{shipment.Recipient.Residence.District.Code}</TownCode>");
                request.Append($@"<PayorTypeCode>{payment}</PayorTypeCode>");
                request.Append($@"<IsWorldWide>0</IsWorldWide>");
                request.Append($@"</Order>");
                request.Append($@"</orderInfo>");
                request.Append($@"<userName>{provider.Username}</userName>");
                request.Append($@"<password>{provider.Password}</password>");
                request.Append($@"</SetOrder>");
                request.Append($@"</soap:Body>");
                request.Append($@"</soap:Envelope>");

                return request.ToString();
            }
        }

        public static class APICancelOrder
        {
            public static string Request(Provider provider, Shipment shipment)
            {
                StringBuilder request = new();

                request.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>");
                request.Append($@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
                request.Append($@"  <soap:Body>");
                request.Append($@"CancelDispatch xmlns=""http://tempuri.org/");

                request.Append($@"<userName>{provider.Username}</userName>");
                request.Append($@"<password>{provider.Password}</password>");
                request.Append($@"<integrationCode>{shipment.CargoId}</integrationCode>");
                request.Append($@"</CancelDispatch>");
                request.Append($@"</soap:Body>");
                request.Append($@"</soap:Envelope>");

                return request.ToString();
            }

        }
    }
}
