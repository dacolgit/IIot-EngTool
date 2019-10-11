
using IIoTEngTool.Registry;
using Microsoft.Azure.IIoT.OpcUa.Services.App.TokenStorage;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Http;
using IIoTEngTool.Twin;
using IIoTEngTool.Services;

namespace IIoTEngTool
{
    public class StartupTwinServices
    {
        /// <summary>
        /// Initialize the Twin service connection
        /// </summary>
        public RegistryService RegistryService;
        public TwinService TwinService;
        private readonly AzureADOptions _azureADOptions;
        private readonly TwinServiceApiOptions _twinServiceOptions;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly HttpContextAccessor _claimsPrincipal;

        public StartupTwinServices(
            AzureADOptions azureADOptions,
            TwinServiceApiOptions twinServiceOptions,
            ITokenCacheService tokenCacheService,
            HttpContextAccessor claimsPrincipal)
        {
            _azureADOptions = azureADOptions;
            _twinServiceOptions = twinServiceOptions;
            _tokenCacheService = tokenCacheService;
            _claimsPrincipal = claimsPrincipal;
        }

        public void ConfigureRegistryService()
        {
            RegistryService = new RegistryService(_azureADOptions, _twinServiceOptions, _tokenCacheService, _claimsPrincipal);
        }

        public void ConfigureTwinService()
        {
            TwinService = new TwinService(_azureADOptions, _twinServiceOptions, _tokenCacheService, _claimsPrincipal);
        }
    }
}

