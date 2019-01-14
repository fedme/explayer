using System;
using System.Collections.Generic;
using System.Text;

namespace Explayer.Models
{
    public class WebApp
    {
        public string Name { get; set; }
        public string PreferredVersion { get; set; }
        public List<string> InstalledVersions;

        public WebApp(string name)
        {
            Name = name;
        }
    }
}
