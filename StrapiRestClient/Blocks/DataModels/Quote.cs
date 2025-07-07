using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Blocks.DataModels
{
    public class Quote
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public string? body { get; set; }
    }
}
