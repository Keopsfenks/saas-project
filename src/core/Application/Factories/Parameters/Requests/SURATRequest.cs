using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Application.Factories.Parameters.Requests
{
    public static class SURATRequest
    {
        public static class APIConfirmOrder
        {
            public static string Request(string auth, Shipment shipment)
            {
                StringBuilder request = new();

                int cod = CodEnum.FromValue(shipment.Dispatch.IsCod) switch
                {
                    var codEnum when codEnum == CodEnum.COD     => 1,
                    var codEnum when codEnum == CodEnum.NOT_COD => 0,
                    _                                           => 0
                };

                int package = PackagingTypeEnum.FromValue(shipment.Dispatch.PackagingType) switch
                {
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.File         => 1,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Mini_Package => 2,
                    var packagingTypeEnum when packagingTypeEnum == PackagingTypeEnum.Box          => 3,
                    _                                                                              => 3
                };

                int payment = PaymentTypeEnum.FromValue(shipment.Dispatch.PaymentType) switch
                {
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Sender   => 1,
                    var paymentTypeEnum when paymentTypeEnum == PaymentTypeEnum.Receiver => 2,
                    _                                                                    => 1
                };

                int type = ShipmentTypeEnum.FromValue(shipment.Type) switch
                {
                    var typeEnum when typeEnum == ShipmentTypeEnum.Order  => 0,
                    var typeEnum when typeEnum == ShipmentTypeEnum.Return => 1,
                    _                                                     => 0
                };


                request.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>");
                request.Append($@"  <soap:Body>
                                <GonderiyiKargoyaGonderYeni xmlns=""http://tempuri.org/"">");

                request.Append($"{auth}");
                request.Append($"<Gonderi>");
                request.Append($"<KisiKurum>{shipment.Recipient.Name} {shipment.Recipient.Surname}</KisiKurum>");
                request.Append($"<SahisBirim>{shipment.Recipient.Name} {shipment.Recipient.Surname}</SahisBirim>");
                request.Append($"<AliciAdresi>{shipment.Recipient.Residence.Address}</AliciAdresi");
                request.Append($"<Il>{shipment.Recipient.Residence.City.Name}</Il>");
                request.Append($"<Ilce>{shipment.Recipient.Residence.District.Name}</Ilce>");
                request.Append($"<TelefonCep>{shipment.Recipient.Phone}</TelefonCep>");
                request.Append($"<Email>{shipment.Recipient.Email}</Email>");
                request.Append($"<AliciKodu>{shipment.CargoId}</AliciKodu>");
                request.Append($"<KargoTuru>{package}</KargoTuru>");
                request.Append($"<OdemeTipi>{payment}</OdemeTipi>");
                request.Append($"<IrsaliyeSeriNo>{shipment.WaybillId}</IrsaliyeSeriNo>");
                request.Append($"<IrsaliyeSiraNo>S{shipment.WaybillId}</IrsaliyeSiraNo>");
                request.Append($"<ReferansNo>{shipment.CargoId}</ReferansNo>");
                request.Append($"<Adet>{shipment.Cargo.Items?.Count ?? 1}</Adet>");
                request.Append($"<BirimDesi>{shipment.Cargo.Volume.Desi}</BirimDesi>");
                request.Append($"<BirimKg>{shipment.Cargo.Volume.Weight}</BirimKg>");
                if (cod == CodEnum.COD)
                {
                    request.Append($"<KapidanOdemeTahsilatTipi>1</KapidanOdemeTahsilatTipi>");
                    request.Append($"<KapidanOdemeTutari>{shipment.Dispatch.CodPrice}</KapidanOdemeTutari>");
                }

                request.Append($"<EkHizmetler>GondericiyeSms, TelefonIhbar, TelefonIhbar</EkHizmetler>");
                request.Append($"<TasimaSekli>1</TasimaSekli>");
                request.Append($"<TeslimSekli>1</TeslimSekli>");
                request.Append($"<GonderiSekli xsi:nil=\"true\"/>");
                request.Append($"<Pazaryerimi>1</Pazaryerimi>");
                request.Append($"<Iademi>{type}</Iademi>");

                request.Append($"</Gonderi>\n</GonderiyiKargoyaGonderYeni>\n</soap:Body>\n</soap:Envelope>");


                return request.ToString();
            }
        }
    }
}