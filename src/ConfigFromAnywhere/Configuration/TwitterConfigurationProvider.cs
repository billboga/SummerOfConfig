using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ConfigFromAnywhere.Configuration
{
    public class TwitterConfigurationProvider : ConfigurationProvider
    {
        public TwitterConfigurationProvider(IDictionary<string, string> data)
        {
            Data = data;
        }
    }
}
