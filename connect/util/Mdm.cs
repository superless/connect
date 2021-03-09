using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.connect.interfaces.hash;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.search_mdl;
using trifenix.exception;
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
        public static ModelMetaData GetMdm(Assembly assembly, string version, VersionStructure versionStructure) {

            // en construcción.
            return null;
        }


    }
}