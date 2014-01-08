using Suro.Net.Client;
using Suro.Net.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Suro.Net.Web
{
    public class SuroModule : IHttpModule
    {
        private static SuroConnectionPool connectionPool = null;
        private static bool compressionEnabled;

        private static object lockObject = new object();

        public void Dispose()
        {
        }

        /// <summary>
        /// Initialises the module based on configuration found in Web.config. If no configuration is found, the
        /// following defaults are assumed: Application Name = 'ASP.NET Application', Host = 'localhost', Port = 7101,
        /// Pool Size = 5, Compression Enabled = false.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            lock (lockObject)
            {
                if (connectionPool == null)
                {
                    var config = System.Configuration.ConfigurationManager.GetSection("suro") as SuroConfigurationSection;

                    connectionPool = new SuroConnectionPool(
                        config == null ? "ASP.NET Application" : config.ApplicationName,
                        config == null ? "localhost" : config.Host,
                        config == null ? 7101 : config.Port,
                        config == null ? 5 : config.PoolSize);

                    compressionEnabled = config != null && config.CompressionEnabled;
                }
            }
            
            context.EndRequest += context_EndRequest;
        }

        /// <summary>
        /// Handler for the end-of-request event, which packages up the context and ships it off to Suro. The
        /// write function will only occur if stuff has actually been added to the context, otherwise this is
        /// a no-op.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        void context_EndRequest(object sender, EventArgs e)
        {
            if (SuroContext.Current.Size > 0)
            {
                using (var conn = connectionPool.Acquire())
                {
                    try
                    {
                        SuroContext.Current.Write(conn, compressionEnabled);
                    }
                    catch (SuroException)
                    {
                        //could not connect to the suro server. Catch this so that it does not impact the user.
                    }
                }
            }
        }
    }
}
