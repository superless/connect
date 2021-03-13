using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.versions.model;

namespace trifenix.connect.util
{




    /// <summary>
    /// Todos los métodos relacionados con la obtención de metadata y valores desde el modelo de clases y la conversión de esta 
    /// al modelo de metada de trifenix y viceversa.
    /// sus subclases son clases estáticas usadas en Reflection.
    /// </summary>
    public static partial class Mdm
    {
        ///
        public static ModelMetaData GetMdm<T_ENUM_FILTERS>(Assembly assembly, string version, VersionStructure versionStructure ) where T_ENUM_FILTERS: Enum
        {

            return new ModelMetaData
            {
                Version = version,
                VersionStructure = versionStructure,
                GlobalFilters = GlobalFilter.GetGlobalFilter<T_ENUM_FILTERS>(assembly)
            };

        }



        
    }
}