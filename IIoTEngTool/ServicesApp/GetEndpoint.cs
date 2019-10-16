using IIoTEngTool.Data;
using IIoTEngTool.Registry;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IIoTEngTool.ServicesApp
{
    public class GetEndpoint
    {

        private RegistryService _registryService;

        public GetEndpoint(RegistryService registryService)
        {
            _registryService = registryService;
        }

        public async Task<PagedResult<EndpointInfo>> GetEndpointList(string supervisorId, IEnumerable<ApplicationInfoApiModel> applications)
        {
            PagedResult<EndpointInfo> pageResult = new PagedResult<EndpointInfo>();

            try
            {
                IEnumerable<EndpointInfoApiModel> endpoints = await _registryService.ListEndpointsAsync();

                if (applications != null)
                {
                    foreach (var application in applications)
                    {
                        if (application.SupervisorId == supervisorId)
                        {
                            foreach (var endpoint in endpoints)
                            {
                                if (endpoint.ApplicationId == application.ApplicationId)
                                {
                                    pageResult.Results.Add(new EndpointInfo
                                    {
                                        EndpointId = endpoint.Registration.Id,
                                        EndpointUrl = endpoint.Registration.Endpoint.Url,
                                        SecurityMode = endpoint.Registration.Endpoint.SecurityMode != null ? endpoint.Registration.Endpoint.SecurityMode.ToString() : string.Empty,
                                        SecurityPolicy = endpoint.Registration.Endpoint.SecurityPolicy != null ? endpoint.Registration.Endpoint.SecurityPolicy.Remove(0, endpoint.Registration.Endpoint.SecurityPolicy.IndexOf('#') + 1) : string.Empty,
                                        SecurityLevel = endpoint.Registration.SecurityLevel,
                                        ApplicationId = application.ApplicationId,
                                        ProductUri = application.ProductUri,
                                        Activated = endpoint.ActivationState == EndpointActivationState.Activated || endpoint.ActivationState == EndpointActivationState.ActivatedAndConnected
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Can not get endpoint list");
                string errorMessage = string.Format(e.Message, e.InnerException?.Message ?? "--", e?.StackTrace ?? "--");
                Trace.TraceWarning(errorMessage);
            }

            pageResult.PageSize = 10;
            pageResult.RowCount = pageResult.Results.Count;
            pageResult.PageCount = (int)Math.Ceiling((decimal)pageResult.RowCount / 10);
            return pageResult;
        }

        public async void ActivateEndpoint(string endpointId)
        {
            try
            {
                await _registryService.ActivateEndpointAsync(endpointId);
            }
            catch (Exception exception)
            {
                // Generate an error to be shown in the error view and trace    .
                string errorMessageTrace = string.Format(exception.Message, exception.InnerException?.Message ?? "--", exception?.StackTrace ?? "--");
                Trace.TraceError(errorMessageTrace);
            }
        }

        public async void DeActivateEndpoint(string endpointId)
        {
            try
            {
                await _registryService.DeActivateEndpointAsync(endpointId);
            }
            catch (Exception exception)
            {
                // Generate an error to be shown in the error view and trace    .
                string errorMessageTrace = string.Format(exception.Message, exception.InnerException?.Message ?? "--", exception?.StackTrace ?? "--");
                Trace.TraceError(errorMessageTrace);
            }
        }
    }
}
