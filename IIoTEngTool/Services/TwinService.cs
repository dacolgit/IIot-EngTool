using Microsoft.Azure.IIoT.Http;
using Microsoft.Azure.IIoT.Http.Default;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.IIoT.Http.Auth;
using IIoTEngTool.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Azure.IIoT.OpcUa.Services.App.TokenStorage;
using System.Threading;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Clients;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin;
using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
using Microsoft.AspNetCore.Http;

namespace IIoTEngTool.Twin
{
    public class TwinServiceConfig : ITwinConfig
    {
        public string OpcUaTwinServiceUrl { get; }
        public string OpcUaTwinServiceResourceId { get; }
        public TwinServiceConfig(string url, string resourceId)
        {
            OpcUaTwinServiceUrl = url;
            OpcUaTwinServiceResourceId = resourceId;
        }
    }

    public class TwinService
    {
        private readonly TwinServiceClient _twinServiceHandler = null;
        private readonly IHttpClient _httpClient = null;
        private readonly ILogger _logger = null;
        private readonly ITwinConfig _config = null;
        private readonly AzureADOptions _azureADOptions;
        private readonly TwinServiceApiOptions _twinServiceOptions;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly HttpContextAccessor _contextAccessor;
        private readonly CancellationToken ct;

        public TwinService(AzureADOptions azureADOptions,
            TwinServiceApiOptions twinServiceOptions,
            ITokenCacheService tokenCacheService,
            HttpContextAccessor contextAccessor
            )
        {
            _azureADOptions = azureADOptions;
            _tokenCacheService = tokenCacheService;
            _contextAccessor = contextAccessor;
            _twinServiceOptions = twinServiceOptions;
            _logger = LogEx.Trace();

            _httpClient = new HttpClient(new HttpClientFactory(new HttpHandlerFactory(new List<IHttpHandler> {
                new HttpBearerAuthentication(new BehalfOfTokenProvider(_azureADOptions, _twinServiceOptions, _tokenCacheService, _contextAccessor))
            }, _logger), _logger), _logger);

            _config = new TwinServiceConfig(_twinServiceOptions.TwinServiceUrl, _twinServiceOptions.ResourceId);
            _twinServiceHandler = new TwinServiceClient(_httpClient, _config);
        }

        public async Task<BrowseResponseApiModel> NodeBrowseAsync(string endpoint, BrowseRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeBrowseFirstAsync(endpoint, content, ct);
            return applications;
        }

        public BrowseNextResponseApiModel NodeBrowseNext(string endpoint, BrowseNextRequestApiModel content)
        {
            Task<BrowseNextResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeBrowseNextAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<BrowseNextResponseApiModel> NodeBrowseNextAsync(string endpoint, BrowseNextRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeBrowseNextAsync(endpoint, content, ct);
            return applications;
        }

        public BrowseResponseApiModel NodeBrowse(string endpoint, BrowseRequestApiModel content)
        {
            Task<BrowseResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeBrowseAsync(endpoint, content));
            return t.Result;
        }

        public async Task<PublishStartResponseApiModel> PublishNodeValuesAsync(string endpoint, PublishStartRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodePublishStartAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public PublishStartResponseApiModel PublishNodeValues(string endpoint, PublishStartRequestApiModel content)
        {
            Task<PublishStartResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodePublishStartAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<PublishStopResponseApiModel> UnPublishNodeValuesAsync(string endpoint, PublishStopRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodePublishStopAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public PublishStopResponseApiModel UnPublishNodeValues(string endpoint, PublishStopRequestApiModel content)
        {
            Task<PublishStopResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodePublishStopAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<PublishedItemListResponseApiModel> GetPublishedNodesAsync(string endpoint, PublishedItemListRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodePublishListAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public PublishedItemListResponseApiModel GetPublishedNodes(string endpoint, PublishedItemListRequestApiModel content)
        {
            Task<PublishedItemListResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodePublishListAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<ValueReadResponseApiModel> ReadNodeValueAsync(string endpoint, ValueReadRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeValueReadAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public ValueReadResponseApiModel ReadNodeValue(string endpoint, ValueReadRequestApiModel content)
        {
            Task<ValueReadResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeValueReadAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<ReadResponseApiModel> ReadNodeAsync(string endpoint, ReadRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeReadAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public ReadResponseApiModel ReadNode(string endpoint, ReadRequestApiModel content)
        {
            Task<ReadResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeReadAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<ValueWriteResponseApiModel> WriteNodeValueAsync(string endpoint, ValueWriteRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeValueWriteAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public ValueWriteResponseApiModel WriteNodeValue(string endpoint, ValueWriteRequestApiModel content)
        {
            Task<ValueWriteResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeValueWriteAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<MethodMetadataResponseApiModel> NodeMethodGetMetadataAsync(string endpoint, MethodMetadataRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeMethodGetMetadataAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public MethodMetadataResponseApiModel NodeMethodGetMetadata(string endpoint, MethodMetadataRequestApiModel content)
        {
            Task<MethodMetadataResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeMethodGetMetadataAsync(endpoint, content, ct));
            return t.Result;
        }

        public async Task<MethodCallResponseApiModel> NodeMethodCallAsync(string endpoint, MethodCallRequestApiModel content)
        {
            var applications = await _twinServiceHandler.NodeMethodCallAsync(endpoint, content, ct).ConfigureAwait(false);
            return applications;
        }

        public MethodCallResponseApiModel NodeMethodCall(string endpoint, MethodCallRequestApiModel content)
        {
            Task<MethodCallResponseApiModel> t = Task.Run(() => _twinServiceHandler.NodeMethodCallAsync(endpoint, content, ct));
            return t.Result;
        }
    }
}