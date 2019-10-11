using Gremlin.Net.Process.Traversal;
using IIoTEngTool.Data;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIoTEngTool.tree
{
    interface IgetNodeService
    {
        public Task<PagedResult<ListNode>> GetTree(string endpointId, string id, List<string> parentId, BrowseDirection direction);

        public Task<PagedResult<ListNode>> GetTreeBack(string endpointId, string id, List<string> parentId);

    }
}
