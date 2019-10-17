using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IIoTEngTool.Data;
using IIoTEngTool.Registry;
using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;

namespace IIoTEngTool.ServicesApp
{
    public class GetSupervisor
    {
        private RegistryService _registryService;
        public IEnumerable<ApplicationInfoApiModel> allApplications;

        public GetSupervisor(RegistryService registryService)
        {
            _registryService = registryService;
        }

        public async Task<PagedResult<SupervisorInfo>> GetSupervisorList()
        {
            PagedResult<SupervisorInfo> pageResult = new PagedResult<SupervisorInfo>();

            try
            {
                IEnumerable<SupervisorApiModel> supervisors = await _registryService.ListSupervisorsAsync();
                IEnumerable<ApplicationInfoApiModel> applications = await _registryService.ListApplicationsAsync();
                allApplications = applications;

                if (supervisors != null)
                {
                    foreach (var supervisor in supervisors)
                    {
                        SupervisorInfo supervisorInfo = new SupervisorInfo();
                        supervisorInfo.SupervisorModel = supervisor;
                        supervisorInfo.HasApplication = false;
                        supervisorInfo.ScanStatus = (supervisor.Discovery == DiscoveryMode.Off) || (supervisor.Discovery == null) ? false : true;
                        foreach (var application in applications)
                        {
                            if (application.SupervisorId == supervisor.Id)
                            {
                                supervisorInfo.HasApplication = true;
                            }
                        }
                        pageResult.Results.Add(supervisorInfo);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Can not get supervisors list");
                string errorMessage = string.Format(e.Message, e.InnerException?.Message ?? "--", e?.StackTrace ?? "--");
                Trace.TraceWarning(errorMessage);
            }

            pageResult.PageSize = 10;
            pageResult.RowCount = pageResult.Results.Count;
            pageResult.PageCount = (int)Math.Ceiling((decimal)pageResult.RowCount / 10);
            return pageResult;
        }

        public async void SetScan(SupervisorInfo supervisor)
        {
            SupervisorUpdateApiModel model = new SupervisorUpdateApiModel();
            model.DiscoveryConfig = new DiscoveryConfigApiModel();
            model.DiscoveryConfig.AddressRangesToScan = "";
            model.DiscoveryConfig.PortRangesToScan = "";

            if (supervisor.ScanStatus == false)
            {
                model.Discovery = DiscoveryMode.Fast;
                //if ((ipMask != null) && (ipMask != string.Empty))
                //{
                //    model.DiscoveryConfig.AddressRangesToScan = ipMask;
                //}
                //if (portRange != null)
                //{
                //    model.DiscoveryConfig.PortRangesToScan = portRange;
                //}
            }
            else
            {
                model.Discovery = DiscoveryMode.Off;
            }

            try
            {
                await _registryService.UpdateSupervisorAsync(supervisor.SupervisorModel.Id, model);
            }
            catch (Exception exception)
            {
                string errorMessageTrace = string.Format(exception.Message, exception.InnerException?.Message ?? "--", exception?.StackTrace ?? "--");
                Trace.TraceError(errorMessageTrace);
            }
        }
    }
}
