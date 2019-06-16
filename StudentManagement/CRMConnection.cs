using System;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using log4net;
using System.Reflection;
using System.Configuration;

namespace StudentManagement
{
    ///
    /// Singleton class
    ///

    public sealed class CrmConnection : IConnection
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Lazy<CrmConnection> LazyInstance = new Lazy<CrmConnection>(() => new CrmConnection());

        /// <summary>
        /// Crm service
        /// </summary>
        public IOrganizationService organizationService { get; }

        public static CrmConnection Instance => LazyInstance.Value;

        private CrmConnection()
        {
            try
            {
                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string username = ConfigurationManager.AppSettings["Username"];
                string password = ConfigurationManager.AppSettings["Password"];
                string url = ConfigurationManager.AppSettings["Url"];

                ClientCredentials clientCredentials = new ClientCredentials();
                clientCredentials.UserName.UserName = username;
                clientCredentials.UserName.Password = password;

                OrganizationServiceProxy proxy = new OrganizationServiceProxy(new Uri(url), null, clientCredentials, null);
                proxy.EnableProxyTypes();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(url))
                {
                    _log.Error($"Logging attempt with invalid credentials: Username {username}, Password {password}, Url {url}");
                }
                else
                {
                    _log.Info($"Logging with credentials: Username {username}, Password {password}, Url {url}");
                    organizationService = (IOrganizationService)proxy;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught during connection initialization - {ex.Message}");
            }
        }
    }
}