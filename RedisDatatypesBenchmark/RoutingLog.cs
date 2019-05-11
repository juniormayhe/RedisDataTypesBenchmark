using System;
using System.Collections.Generic;
using System.Text;

namespace RedisDatatypesBenchmark
{
    public class RoutingLog
    {
        public Guid RequestId { get; set; }

        public int ProductId { get; set; }

        public Guid VariantId { get; set; }

        public IDictionary<string, IEnumerable<string>> RemovedEntitiesByReason { get; set; }

        public string GetKey()
        {
            return $":RequestId_{this.RequestId}:ProductId_{this.ProductId}:VariantId_{this.VariantId}";
        }
    }
}
