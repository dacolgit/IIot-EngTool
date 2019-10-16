using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;

namespace IIoTEngTool.ServicesApp
{
    public class SupervisorInfo
    {
        /// <summary>
        /// Supervisor models.
        /// </summary>
        public SupervisorApiModel supervisorModel { get; set; }

        /// <summary>
        /// Supervisor has application children.
        /// </summary>
        public bool HasApplication { get; set; }
    }
}
