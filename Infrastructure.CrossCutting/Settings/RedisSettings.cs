namespace Infrastructure.CrossCutting.Settings
{
    using System;

    public class RedisSettings
    {
        public string Server { get; set; }

        public bool Enabled { get; set; }

        public int KeyExpirationInMinutes { get; set; }

        public int MerchantsOnHolidaysKeyExpiration { get; set; }

        /// <summary>
        /// Expiration for merchant service with merchantClient when merchantId is provided
        /// </summary>
        public int StockpointsByMerchantIdKeyExpirationInMinutes { get; set; }

        /// <summary>
        /// Expiration for merchant service with locationClient when merchantId is not provided
        /// </summary>
        public int StockpointsByStockpointIdsKeyExpirationInMinutes { get; set; }

        public int ProductKeyExpirationInMinutes { get; set; }

        public int DefaultDb { get; set; }

        public bool PreferSlaveForRead { get; set; }

        public bool CriticalDependency { get; set; }

        /// <summary>
        /// Validates app settings.
        /// Throws an exception validation does not succeed and prevents app from starting
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.Server))
            {
                throw new Exception("Server must not be null or empty");
            }

            if (this.Enabled)
            {
                if (this.KeyExpirationInMinutes <= 0)
                {
                    throw new Exception("KeyExpirationInMinutes must be greater than zero");
                }

                if (this.MerchantsOnHolidaysKeyExpiration <= 0)
                {
                    throw new Exception("MerchantsOnHolidaysKeyExpiration must be greater than zero");
                }

                if (this.MerchantsOnHolidaysKeyExpiration <= 0)
                {
                    throw new Exception("MerchantsOnHolidaysKeyExpiration must be greater than zero");
                }

                if (this.StockpointsByMerchantIdKeyExpirationInMinutes <= 0)
                {
                    throw new Exception("StockpointsByMerchantIdKeyExpirationInMinutes must be greater than zero");
                }

                if (this.StockpointsByStockpointIdsKeyExpirationInMinutes <= 0)
                {
                    throw new Exception("StockpointsByStockpointIdsKeyExpirationInMinutes must be greater than zero");
                }

                if (this.ProductKeyExpirationInMinutes < 0)
                {
                    throw new ApplicationException("ProductKeyExpirationInMinutes must be greater than zero");
                }

                if (this.DefaultDb < 0)
                {
                    throw new ApplicationException("DefaultDb must be equals or greater than zero");
                }
            }
        }
    }
}
