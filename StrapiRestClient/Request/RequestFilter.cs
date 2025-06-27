using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Request
{
    /// <summary>
    /// Represents a filter to be applied to a Strapi API request.
    /// </summary>
    public struct RequestFilter
    {
        /// <summary>
        /// Gets or sets the type of the filter (e.g., "$eq", "$contains").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the field to which the filter applies.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        public string Value { get; set; }
    }
}
