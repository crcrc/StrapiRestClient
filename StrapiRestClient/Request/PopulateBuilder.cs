﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace StrapiRestClient.Request
{
    /// <summary>
    /// A builder class for constructing populate queries for Strapi API requests.
    /// </summary>
    public class PopulateBuilder
    {
        /// <summary>
        /// Gets or sets the name of the current populate relation.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the child populate builders, representing nested relations.
        /// </summary>
        public Dictionary<string, PopulateBuilder> Children { get; } = new();

        /// <summary>
        /// Gets the list of fields to select for the current relation.
        /// </summary>
        public List<string>? Fields { get; private set; }

        /// <summary>
        /// Populates a new relation within the current builder.
        /// </summary>
        /// <param name="name">The name of the relation to populate.</param>
        /// <returns>A new <see cref="PopulateBuilder"/> instance for the specified relation.</returns>
        /// <exception cref="ArgumentException">Thrown if the name is null or empty.</exception>
                public PopulateBuilder Populate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            var parts = name.Split(new[] { '.' }, 2);
            var childName = parts[0];

            if (!Children.ContainsKey(childName))
            {
                Children[childName] = new PopulateBuilder { Name = childName };
            }

            if (parts.Length > 1)
            {
                return Children[childName].Populate(parts[1]);
            }

            return Children[childName];
        }

        /// <summary>
        /// Selects specific fields for the current relation.
        /// </summary>
        /// <param name="fields">The fields to select.</param>
        /// <returns>The current <see cref="PopulateBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown if the fields array is null or empty.</exception>
        public PopulateBuilder SelectFields(params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                throw new ArgumentException("Fields cannot be null or empty", nameof(fields));

            Fields = fields.ToList();
            return this; // Return this for method chaining
        }

        /// <summary>
        /// Populates all fields for the current relation.
        /// </summary>
        /// <returns>The current <see cref="PopulateBuilder"/> instance.</returns>
        public PopulateBuilder PopulateAll()
        {
            Fields = new List<string> { "*" };
            return this; // Return this for method chaining
        }

        /// <summary>
        /// Converts the populate builder configuration to an object suitable for Strapi v5 API requests.
        /// </summary>
        /// <returns>An object representing the populate query, optimized for Strapi v5 syntax.</returns>
                public object ToObject()
        {
            if (Fields != null && Fields.Contains("*") && Children.Count == 0) return "*";

            var result = new Dictionary<string, object>();
            if (Fields?.Count > 0)
            {
                result["fields"] = string.Join(",", Fields);
            }

            if (Children.Count > 0)
            {
                // For Strapi v5, check if all children are simple populates (no fields, no nested children)
                var simplePopulates = new List<string>();
                var complexPopulates = new Dictionary<string, object>();

                foreach (var (key, value) in Children)
                {
                    var childObj = value.ToObject();
                    
                    // If it's just "*", it's a simple populate
                    if (childObj is string s && s == "*")
                    {
                        simplePopulates.Add(key);
                    }
                    else
                    {
                        complexPopulates[key] = childObj;
                    }
                }

                // For root level, return simple populates as array for Strapi v5 format
                if (string.IsNullOrEmpty(Name) && simplePopulates.Count > 0 && complexPopulates.Count == 0 && result.Count == 0)
                {
                    return simplePopulates;
                }

                // Mixed case: return dictionary format
                if (simplePopulates.Count > 0 || complexPopulates.Count > 0)
                {
                    var populateDict = new Dictionary<string, object>();
                    
                    // Add simple populates as individual entries
                    foreach (var simple in simplePopulates)
                    {
                        populateDict[simple] = "*";
                    }
                    
                    // Add complex populates
                    foreach (var (key, value) in complexPopulates)
                    {
                        populateDict[key] = value;
                    }
                    
                    result["populate"] = populateDict;
                }
            }

            if (result.Count == 0) return "*";
            
            if (string.IsNullOrEmpty(Name) && result.ContainsKey("populate"))
            {
                return result["populate"];
            }

            return result;
        }
    }
}