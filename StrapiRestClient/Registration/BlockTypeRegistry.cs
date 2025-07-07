using StrapiRestClient.Blocks;
using StrapiRestClient.Blocks.DataModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Registration
{
    public static class BlockTypeRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> _blockTypes = new();
        private static readonly ConcurrentDictionary<Type, string> _componentNames = new();

        static BlockTypeRegistry()
        {
            // Register built-in types
            //RegisterBlockType<RichTextBlockComponent>("shared.rich-text");
            //RegisterBlockType<QuoteBlockComponent>("shared.quote");
        }

        /// <summary>
        /// Register a custom block type that users can call from their applications
        /// </summary>
        public static void RegisterBlockType<T>(string componentName) where T : IBlockComponent
        {
            RegisterBlockType(typeof(T), componentName);
        }

        /// <summary>
        /// Register a custom block type by Type
        /// </summary>
        public static void RegisterBlockType(Type blockType, string componentName)
        {
            if (!typeof(IBlockComponent).IsAssignableFrom(blockType))
                throw new ArgumentException($"Block type must implement {nameof(IBlockComponent)}", nameof(blockType));

            _blockTypes[componentName] = blockType;
            _componentNames[blockType] = componentName;
        }

        /// <summary>
        /// Get the registered type for a component name
        /// </summary>
        public static Type? GetBlockType(string componentName)
        {
            return _blockTypes.TryGetValue(componentName, out var type) ? type : null;
        }

        /// <summary>
        /// Get the component name for a registered type
        /// </summary>
        public static string? GetComponentName(Type blockType)
        {
            return _componentNames.TryGetValue(blockType, out var name) ? name : null;
        }

        /// <summary>
        /// Get all registered component names
        /// </summary>
        public static IEnumerable<string> GetRegisteredComponents()
        {
            return _blockTypes.Keys;
        }

        /// <summary>
        /// Check if a component is registered
        /// </summary>
        public static bool IsRegistered(string componentName)
        {
            return _blockTypes.ContainsKey(componentName);
        }

        /// <summary>
        /// Clear all registrations (useful for testing)
        /// </summary>
        public static void Clear()
        {
            _blockTypes.Clear();
            _componentNames.Clear();
        }

        /// <summary>
        /// Auto-register all block types in an assembly using attributes
        /// </summary>
        public static void RegisterFromAssembly(Assembly assembly)
        {
            var blockTypes = assembly.GetTypes()
                .Where(t => typeof(IBlockComponent).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t => t.GetCustomAttribute<BlockComponentAttribute>() != null);

            foreach (var type in blockTypes)
            {
                var attribute = type.GetCustomAttribute<BlockComponentAttribute>();
                if (attribute != null)
                {
                    RegisterBlockType(type, attribute.ComponentName);
                }
            }
        }
    }
}
