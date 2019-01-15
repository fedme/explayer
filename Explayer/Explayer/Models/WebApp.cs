using System;
using System.Collections.Generic;
using System.Text;

namespace Explayer.Models
{
    public class WebApp
    {
        public string Name { get; }
        public string PreferredVersion { get; set; }
        public List<string> InstalledVersions;

        public WebApp(string name)
        {
            Name = name;
        }
        
        protected bool Equals(WebApp other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
