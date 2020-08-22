using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Dgp.Domain.Core.Test
{
    public class NamespaceTest
    {
        /// <summary>
        /// Test that all classes defined in the Dgp.Domain.Core assembly use the root namespace.
        /// </summary>
        [Fact]
        public void All_Classes_Have_Root_Namespace()
        {
            var types = Assembly.GetAssembly(typeof(Course))
                                .GetTypes()
                                .Where(t => t.Namespace != null)
                                .Where(t => !t.Namespace.Equals("Dgp.Domain.Core"))
                                .Select(t => $"{t.Namespace}.{t.Name}")
                                .ToArray();
            Assert.Empty(types);
        }
    }
}
