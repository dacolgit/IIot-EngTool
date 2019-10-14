
using IIoTEngTool.tree;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IIoT.Auth;
using Microsoft.Azure.IIoT.Auth.Models;
using Microsoft.Azure.IIoT.OpcUa.Services.App.TokenStorage;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;


namespace IIoTEngTool.Services
{
    public class BehalfOfTokenProvider : ITokenProvider
    {
        private AzureADOptions _azureADOptions;
        private TwinServiceApiOptions _twinServiceOptions;
        private ITokenCacheService _tokenCacheService;
        private HttpContextAccessor _contextAccessor;
        public static Action Authorize { get; set; }

        public BehalfOfTokenProvider(
            AzureADOptions azureADOptions,
            TwinServiceApiOptions twinServiceOptions,
            ITokenCacheService tokenCacheService,
            HttpContextAccessor contextAccessor
            )
        {
            _azureADOptions = azureADOptions;
            _twinServiceOptions = twinServiceOptions;
            _tokenCacheService = tokenCacheService;
            _contextAccessor = contextAccessor;
        }

        public async Task<TokenResultModel> GetTokenForAsync(string resource, IEnumerable<string> scopes = null)
        {
            var tokenCache = _tokenCacheService.GetCacheAsync(_contextAccessor.HttpContext.User).Result;

            var authenticationContext =
                new AuthenticationContext(_azureADOptions.Instance + _azureADOptions.TenantId, tokenCache);

            var credential = new ClientCredential(
                clientId: _azureADOptions.ClientId,
                clientSecret: _azureADOptions.ClientSecret);

            string userObjectId = (_contextAccessor.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
            var user = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);
            
            try
            {
                var _result = await authenticationContext.AcquireTokenSilentAsync(
                        resource: _twinServiceOptions.ResourceId,
                        clientCredential: credential,
                        userId: user);
                return _result.ToTokenResult();
            }
            catch (AdalException ex)
            {
                if (ex is AdalSilentTokenAcquisitionException)
                {
                    Authorize.Invoke();
                }
                else 
                {
                    throw new AuthenticationException(
                    $"Failed to authenticate on behalf of {user}", ex);
                } 
            }
            return null;
        }

        public Task InvalidateAsync(string resource)
        {
            return Task.CompletedTask;
        }
    }
}