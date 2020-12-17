using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace trifenix.connect.util
{





    public static partial class Mdm
    {
        public static partial class Reflection {
            /// <summary>
            /// Operaciones reflection para enumeraciones.
            /// </summary>
            public static class Enumerations
            {

                /// <summary>
                /// Convierte una enumeración en un dictionary
                /// </summary>
                /// <typeparam name="T">enumeración</typeparam>
                /// <returns>dictionary</returns>
                public static Dictionary<int, string> GetDictionaryFromEnum<T>() where T : Enum {
                    if (!typeof(T).IsEnum)
                        throw new ArgumentException("Type must be an enum");
                    return Enum.GetValues(typeof(T))
                        .Cast<T>()
                        .ToDictionary(t => (int)(object)t, t => t.ToString());
                }
            }
        }

        


    }
}
