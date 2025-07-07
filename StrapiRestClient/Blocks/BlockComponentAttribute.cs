using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Blocks
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlockComponentAttribute : Attribute
    {
        public string? ComponentName { get; }

        public BlockComponentAttribute(string componentName)
        {
            ComponentName = componentName;
        }
    }
}
