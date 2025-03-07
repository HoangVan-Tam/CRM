using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class AppConfig
    {
        
        public ExternalApiConfig ExternalApiConfig { get; set; }
        public AzureBlobStorage AzureBlobStorage { get; set; }
    }
    public class ExternalApiConfig
    {
        public string OutboundURL { get; set; }
    }
    public class AzureBlobStorage{
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string EndpointUrl{ get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }
}
