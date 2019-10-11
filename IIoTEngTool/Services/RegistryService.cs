
using Microsoft.Azure.IIoT.Http;
using Microsoft.Azure.IIoT.Http.Default;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Clients;
using System.Threading.Tasks;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
using System.Collections.Generic;
using Microsoft.Azure.IIoT.Http.Auth;
using IIoTEngTool.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Azure.IIoT.OpcUa.Services.App.TokenStorage;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace IIoTEngTool.Registry
{
    public class RegistryServiceConfig : IRegistryConfig
    {
        public string OpcUaRegistryServiceUrl { get; }
        public string OpcUaRegistryServiceResourceId { get; }
        public RegistryServiceConfig(string url, string resourceId)
        {
            OpcUaRegistryServiceUrl = url;
            OpcUaRegistryServiceResourceId = resourceId;
        }
    }

    public class RegistryService
    {
        private readonly RegistryServiceClient _registryServiceHandler = null;
        private readonly IHttpClient _httpClient = null;
        private readonly ILogger _logger = null;
        private readonly IRegistryConfig _config = null;
        private readonly AzureADOptions _azureADOptions;
        private readonly TwinServiceApiOptions _twinServiceOptions;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly HttpContextAccessor _contextAccessor;
        private readonly CancellationToken ct;

        public RegistryService(
            AzureADOptions azureADOptions,
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

            _config = new RegistryServiceConfig(_twinServiceOptions.RegistryServiceUrl, _twinServiceOptions.ResourceId);
            
            _registryServiceHandler = new RegistryServiceClient(_httpClient, _config);
        }

        public async Task<IEnumerable<SupervisorApiModel>> ListSupervisorsAsync()
        {
            var applications = await _registryServiceHandler.ListAllSupervisorsAsync(true).ConfigureAwait(false);
            return applications;
        }

        public IEnumerable<SupervisorApiModel> ListSupervisors()
        {
            Task<IEnumerable<SupervisorApiModel>> t = Task.Run(() => _registryServiceHandler.ListAllSupervisorsAsync(true));
            return t.Result;
        }

        public async Task<IEnumerable<ApplicationInfoApiModel>> ListApplicationsAsync()
        {
            var applications = await _registryServiceHandler.ListAllApplicationsAsync().ConfigureAwait(false);
            return applications;
        }

        public IEnumerable<ApplicationInfoApiModel> ListApplications()
        {
            Task<IEnumerable<ApplicationInfoApiModel>> t = Task.Run(() => _registryServiceHandler.ListAllApplicationsAsync());
            return t.Result;
        }

        public async Task<ApplicationRegistrationApiModel> GetApplicationAsync(string applicationId)
        {
            var applicationRecord = await _registryServiceHandler.GetApplicationAsync(applicationId, ct).ConfigureAwait(false);
            return applicationRecord;
        }

        public ApplicationRegistrationApiModel GetApplication(string applicationId)
        {
            Task<ApplicationRegistrationApiModel> t = Task.Run(() => _registryServiceHandler.GetApplicationAsync(applicationId, ct));
            return t.Result;
        }

        public async Task UnregisterApplicationAsync(string applicationId)
        {
            await _registryServiceHandler.UnregisterApplicationAsync(applicationId, ct).ConfigureAwait(false);
        }

        public void UnregisterApplication(string applicationId)
        {
            Task t = Task.Run(() => _registryServiceHandler.UnregisterApplicationAsync(applicationId, ct));
            t.Wait();
        }

        public async Task<IEnumerable<EndpointInfoApiModel>> ListEndpointsAsync()
        {
            var endpoints = await _registryServiceHandler.ListAllEndpointsAsync(true).ConfigureAwait(false);
            return endpoints;
        }

        public IEnumerable<EndpointInfoApiModel> ListEndpoints()
        {
            Task<IEnumerable<EndpointInfoApiModel>> t = Task.Run(() => _registryServiceHandler.ListAllEndpointsAsync(true));
            return t.Result;
        }


        public async Task ActivateEndpointAsync(string endpointId)
        {
            await _registryServiceHandler.ActivateEndpointAsync(endpointId, ct).ConfigureAwait(false);
        }

        public void ActivateEndpoint(string endpointId)
        {
            Task t = Task.Run(() => _registryServiceHandler.ActivateEndpointAsync(endpointId, ct));
        }

        public async Task DeActivateEndpointAsync(string endpointId)
        {
            await _registryServiceHandler.DeactivateEndpointAsync(endpointId, ct).ConfigureAwait(false);
        }

        public void DeActivateEndpoint(string endpointId)
        {
            Task t = Task.Run(() => _registryServiceHandler.DeactivateEndpointAsync(endpointId, ct));
        }

        public async Task UpdateSupervisorAsync(string supervisorId, SupervisorUpdateApiModel model)
        {
            await _registryServiceHandler.UpdateSupervisorAsync(supervisorId, model, ct).ConfigureAwait(false);
        }

        public void UpdateSupervisor(string supervisorId, SupervisorUpdateApiModel model)
        {
            Task t = Task.Run(() => _registryServiceHandler.UpdateSupervisorAsync(supervisorId, model, ct));
        }

        public async Task<EndpointInfoApiModel> GetEndpointAsync(string endpointId)
        {
            var endpointModel = await _registryServiceHandler.GetEndpointAsync(endpointId, true, ct).ConfigureAwait(false);
            return endpointModel;
        }

        public EndpointInfoApiModel GetEndpoint(string endpointId)
        {
            Task<EndpointInfoApiModel> t = Task.Run(() => _registryServiceHandler.GetEndpointAsync(endpointId, true, ct));
            return t.Result;
        }
    }
}