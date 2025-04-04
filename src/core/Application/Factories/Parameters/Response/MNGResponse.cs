namespace Application.Factories.Parameters.Response
{
    public static class MNGResponse
    {
        public sealed class APIToken(
            string Jwt,
            string RefreshToken,
            string JwtExpireDate,
            string RefreshTokenExpireDate)
        {
            public string Jwt                    { get; set; } = Jwt;
            public string RefreshToken           { get; set; } = RefreshToken;
            public string JwtExpireDate          { get; set; } = JwtExpireDate;
            public string RefreshTokenExpireDate { get; set; } = RefreshTokenExpireDate;
        }

        public sealed record APICreateOrder(
            string orderInvoiceId,
            string orderInvoiceDetailId,
            string shipperBranchCode);

        public sealed record APICreateBarcode
        {
            public sealed record Barcodes(
                int    pieceNumber,
                string value);

            public string         referenceId { get; set; }
            public string         invoiceId   { get; set; }
            public string         shipmentId  { get; set; }
            public List<Barcodes> barcodes    { get; set; } = new List<Barcodes>();
        }
    }
}