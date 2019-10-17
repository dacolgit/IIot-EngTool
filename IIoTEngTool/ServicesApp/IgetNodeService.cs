
using IIoTEngTool.Data;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IIoTEngTool.ServicesApp
{
    interface IgetNodeService
    {
        public Task<PagedResult<ListNode>> GetTree(string endpointId, string id, List<string> parentId, string supervisorId, BrowseDirection direction);
    }
}
