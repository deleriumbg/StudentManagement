using Microsoft.Xrm.Sdk;

namespace StudentManagement
{
    public interface IConnection
    {
        IOrganizationService organizationService { get; }
    }
}