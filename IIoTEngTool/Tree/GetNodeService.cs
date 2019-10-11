
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using System.Diagnostics;
using IIoTEngTool.Data;


namespace IIoTEngTool.tree
{
    public class GetNodeService : IgetNodeService
    {
        public string path;

        public GetNodeService()
        {
            Startup.RegistryServiceInstance.ConfigureTwinService();
        }

        public async Task<PagedResult<ListNode>> GetTree(string endpointId, string id, List<string> parentId, BrowseDirection direction)
        {
            PagedResult<ListNode> pageResult = new PagedResult<ListNode>();
            BrowseRequestApiModel model = new BrowseRequestApiModel();

            model.TargetNodesOnly = true;

            if (direction == BrowseDirection.Forward)
            {
                model.MaxReferencesToReturn = 10;
                model.NodeId = id;
                if (id == string.Empty)
                {
                    path = string.Empty;
                }
            }
            else
            {
                model.NodeId = parentId.ElementAt(parentId.Count - 2);
            }
            
            try
            {
                if (Startup.RegistryServiceInstance.TwinService == null)
                {
                    Startup.RegistryServiceInstance.ConfigureTwinService();
                }
                var browseData = await Startup.RegistryServiceInstance.TwinService.NodeBrowseAsync(endpointId, model);
                var continuationToken = browseData.ContinuationToken;
                List<NodeReferenceApiModel> references = browseData.References;
                BrowseNextResponseApiModel browseDataNext = new BrowseNextResponseApiModel();

                if (direction == BrowseDirection.Forward)
                {
                    parentId.Add(browseData.Node.NodeId);
                    path += "/" + browseData.Node.DisplayName;
                }
                else 
                {
                    parentId.RemoveAt(parentId.Count - 1);
                    path = path.Substring(0, path.LastIndexOf("/"));
                }
                
                do
                {
                    if (references != null)
                    {
                        foreach (var nodeReference in references)
                        {
                            pageResult.Results.Add(new ListNode
                            {
                                id = nodeReference.Target.NodeId.ToString(),
                                nodeClass = nodeReference.Target.NodeClass.ToString(),
                                nodeName = nodeReference.Target.DisplayName.ToString(),
                                children = (bool)nodeReference.Target.Children,
                                parentIdList = parentId,
                                parentName = browseData.Node.DisplayName
                        });
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(continuationToken))
                    {
                        BrowseNextRequestApiModel modelNext = new BrowseNextRequestApiModel();
                        modelNext.ContinuationToken = continuationToken;
                        browseDataNext = await Startup.RegistryServiceInstance.TwinService.NodeBrowseNextAsync(endpointId, modelNext);
                        references = browseDataNext.References;
                        continuationToken = browseDataNext.ContinuationToken;
                    }
                    else
                    {
                        browseDataNext.References = null;
                    }

                } while (!string.IsNullOrEmpty(continuationToken) || browseDataNext.References != null);
                
            }
            catch (Exception e)
            {
                // skip this node
                Trace.TraceError("Can not browse node '{0}'", id);
                string errorMessage = string.Format(e.Message, e.InnerException?.Message ?? "--", e?.StackTrace ?? "--");
                Trace.TraceError(errorMessage);
            }
           
            pageResult.PageSize = 10;
            pageResult.RowCount = (int)pageResult.Results.Count;
            pageResult.PageCount = (int)Math.Ceiling((decimal)pageResult.RowCount / 10);
            return pageResult;
        }
    }
}
