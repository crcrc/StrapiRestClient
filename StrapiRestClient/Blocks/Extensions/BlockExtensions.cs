using StrapiRestClient.Blocks.DataModels;
using StrapiRestClient.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Blocks.Extensions
{
    public static class BlockExtensions
    {
        public static IEnumerable<T> GetBlocksOfType<T>(this List<IBlockComponent> blocks) where T : class, IBlockComponent
        {
            return blocks.OfType<T>();
        }

        public static IEnumerable<IBlockComponent> GetBlocksByComponent(this List<IBlockComponent> blocks, string componentName)
        {
            return blocks.Where(b => b.__component == componentName);
        }

        public static T? GetFirstBlockOfType<T>(this List<IBlockComponent> blocks) where T : class, IBlockComponent
        {
            return blocks.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Get blocks by registered type
        /// </summary>
        public static IEnumerable<IBlockComponent> GetBlocksByRegisteredType(this List<IBlockComponent> blocks, Type blockType)
        {
            var componentName = BlockTypeRegistry.GetComponentName(blockType);
            return componentName != null ? blocks.GetBlocksByComponent(componentName) : Enumerable.Empty<IBlockComponent>();
        }

        /// <summary>
        /// Check if any blocks are of unknown/unregistered types
        /// </summary>
        public static bool HasUnknownBlocks(this List<IBlockComponent> blocks)
        {
            return blocks.Any(b => b is GenericBlock);
        }

        /// <summary>
        /// Get all unknown/unregistered blocks
        /// </summary>
        public static IEnumerable<GenericBlock> GetUnknownBlocks(this List<IBlockComponent> blocks)
        {
            return blocks.OfType<GenericBlock>();
        }
    }
}
