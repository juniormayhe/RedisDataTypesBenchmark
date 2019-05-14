namespace RedisShared
{
    using System;
    using System.Collections.Generic;

    public class RoutingLog
    {
        public Guid RequestId { get; set; }

        public int ProductId { get; set; }

        public Guid VariantId { get; set; }

        public IDictionary<string, IEnumerable<string>> RemovedEntitiesByReason { get; set; }

        public string GetFullKey()
        {
            return $"RequestId_{this.RequestId}:ProductId_{this.ProductId}:VariantId_{this.VariantId}";
        }

        public string GetProductVariantKey()
        {
            return $"ProductId_{this.ProductId}:VariantId_{this.VariantId}";
        }
    }
}
