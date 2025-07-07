using StrapiRestClient.Blocks.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Registration
{
    public interface IBlockRegistration
    {
        IBlockRegistration RegisterBlock<T>(string componentName) where T : IBlockComponent;
        IBlockRegistration RegisterFromAssembly(Assembly assembly);
    }

    public class BlockRegistration : IBlockRegistration
    {
        public IBlockRegistration RegisterBlock<T>(string componentName) where T : IBlockComponent
        {
            BlockTypeRegistry.RegisterBlockType<T>(componentName);
            return this;
        }

        public IBlockRegistration RegisterFromAssembly(Assembly assembly)
        {
            BlockTypeRegistry.RegisterFromAssembly(assembly);
            return this;
        }
    }
}
