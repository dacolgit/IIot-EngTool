using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIoTEngTool.ServicesApp
{
    public class EndpointInfo
    {
        /// <summary>
        /// The OPC UA Endpoint endpoint Id.
        /// </summary>
        public string EndpointId { get; set; }

        /// <summary>
        /// The OPC UA Endpoint endpoint Url.
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        /// The OPC UA Endpoint Security Mode.
        /// </summary>
        public string SecurityMode { get; set; }

        /// <summary>
        ///The OPC UA Endpoint Security Policy.
        /// </summary>
        public string SecurityPolicy { get; set; }

        /// <summary>
        /// The OPC UA Endpoint Security Level.
        /// </summary>
        public int? SecurityLevel { get; set; }

        /// <summary>
        /// The OPC UA Application Id.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// The OPC UA ProductUri
        /// </summary>
        public string ProductUri { get; set; }

        /// <summary>
        /// The OPC UA endpoint activation status
        /// </summary>
        public bool Activated { get; set; }
    }
}
