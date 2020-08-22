using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Dgp.Domain.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Null or empty
        /// </summary>
        public static bool Nada<TElement>(this IEnumerable<TElement> collection)
        {
            return !Some(collection);
        }

        /// <summary>
        /// Not null and not empty
        /// </summary>
        public static bool Some<TElement>(this IEnumerable<TElement> collection)
        {
            return collection != null && collection.Any();
        }
    }
}
