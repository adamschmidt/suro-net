using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Configuration
{
    /// <summary>
    ///     <section name="suro" type="Suro.Net.Configuration.SuroConfigurationSection, Suro.Net"/>
    /// </summary>
    public class SuroConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("applicationName", DefaultValue = "Suro .NET Application", IsRequired = false)]
        public string ApplicationName
        {
            get
            {
                return (string)this["applicationName"];
            }
            set
            {
                this["applicationName"] = value;
            }
        }

        [ConfigurationProperty("compressionEnabled", DefaultValue = "false", IsRequired = false)]
        public bool CompressionEnabled
        {
            get
            {
                return (bool)this["compressionEnabled"];
            }
            set
            {
                this["compressionEnabled"] = value;
            }
        }

        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value;
            }
        }
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }
        [ConfigurationProperty("poolSize", DefaultValue = 5, IsRequired = false)]
        public int PoolSize
        {
            get
            {
                return (int)this["poolSize"];
            }
            set
            {
                this["poolSize"] = value;
            }
        }
    }
}
