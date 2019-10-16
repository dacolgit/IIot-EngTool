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
                        supervisorInfo.supervisorModel = supervisor;
                        supervisorInfo.HasApplication = false;
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
    }
}
