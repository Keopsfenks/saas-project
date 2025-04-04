using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using System.Text;

namespace Application.Factories.Parameters.Requests
{
    public static class YURTICIRequest
    {
        public record Provider;

        public static class APIConfirmOrder
        {

            public static string Request(string auth, Shipment shipment)
            {
                StringBuilder request = new();

                request.Append(
                    @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ship=""https://yurticikargo.com.tr/ShippingOrderDispatcherServices"">");
                request.Append(
                    @"<soapenv:Header/>");
                request.Append(
                    @"<soapenv:Body>");
                request.Append(
                    @"<ship:createShipment>");
                request.Append(auth);

                request.Append(@"<ShippingOrderVO>");

                request.Append($@"<cargoKey>{shipment.CargoId}</cargoKey>");
                request.Append($@"<invoiceKey>{shipment.CargoId}</invoiceKey>");
                request.Append($@"<waybillNo>{shipment.CargoId}</waybillNo>");
                request.Append(
                    $@"<receiverCustName>{shipment.Recipient.Name} {shipment.Recipient.Surname}</receiverCustName>");
                request.Append($@"<receiverAddress>{shipment.Recipient.Residence.Address}</receiverAddress>");
                request.Append($@"<cityName>{shipment.Recipient.Residence.City.Name}</cityName>");
                request.Append($@"<townName>{shipment.Recipient.Residence.District.Name}</townName>");
                request.Append($@"<receiverPhone1>{shipment.Recipient.Phone}</receiverPhone1>");
                request.Append($@"<emailAddress>{shipment.Recipient.Email}</emailAddress>");
                request.Append($@"<taxNumber>{shipment.Recipient.TaxNumber}</taxNumber>");
                request.Append($@"<taxOfficeName>{shipment.Recipient.TaxDepartment}</taxOfficeName>");
                request.Append($@"<desi>{shipment.Cargo.Volume.Desi}</desi>");
                request.Append($@"<kg>{shipment.Cargo.Volume.Weight}</kg>");
                request.Append($@"<cargoCount>{shipment.Cargo.Items?.Count ?? 1}</cargoCount>");

                if (shipment.Dispatch.IsCod == CodEnum.COD)
                {
                    request.Append($@"<ttCollectionType>0</ttCollectionType>");
                    request.Append($@"<ttInvoiceAmount>{shipment.Dispatch.CodPrice}</ttInvoiceAmount>");
                    request.Append($@"<ttDocumentId>{ParametersFactory.CreateNumber()}</ttDocumentId>");
                    request.Append($@"<ttDocumentSaveType>0</ttDocumentSaveType>");
                }
                request.Append("@</ShippingOrderVO>");

                request.Append(
                    @"</ship:createShipment>");
                request.Append(
                    @"</soapenv:Body>");
                request.Append(
                    @"</soapenv:Envelope>");

                return request.ToString();
            }
        }

        public static class APICancelOrder
        {
            public static string Request(string auth, Shipment shipment)
            {
                StringBuilder request = new();

                request.Append(
                    @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ship=""https://yurticikargo.com.tr/ShippingOrderDispatcherServices"">");
                request.Append(
                    @"<soapenv:Header/>");
                request.Append(
                    @"<soapenv:Body>");
                request.Append(
                    @"<ship:cancelShipment>");
                request.Append(auth);
                request.Append($@"<cargoKeys>{shipment.CargoId}</cargoKeys>");

                request.Append(
                    @"</ship:cancelShipment>");
                request.Append(
                    @"</soapenv:Body>");
                request.Append(
                    @"</soapenv:Envelope>");

                return request.ToString();
            }
        }
    }
}