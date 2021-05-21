using System;
using System.Collections.Generic;
using System.Linq;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm.enums;
using trifenix.exception;

namespace trifenix.connect.util
{

    public static partial class Mdm
    {
        /// <summary>
        /// Operaciones relacionadas con procesos
        /// Donde el atributo ToProcess es usado para saltar de una entidad a otra 
        /// cada ruta tiene un identificador y un objetivo
        /// La información originada es usada en la metadata
        /// </summary>
        public static class Docs {

            /// <summary>
            /// Asigna la documentación a la entidad, sus propiedades del modelo y sus propiedades de entrada.
            /// </summary>
            /// <param name="entity">Entidad a documentar</param>
            /// <param name="mainDocumentation">Documentación del modelo.</param>
            /// <returns>Entidad documentada.</returns>
            public static EntityMetadata GetEntityWithDocumentation(EntityMetadata entity, MainDocumentation mainDocumentation)
            {

                // 1. documentacion directa en metadata, directo y como EntitySearchDisplayInfo.
                entity = GetEntityMetadataMainDoc(entity, mainDocumentation);

                // 2. Documentacion de propiedades de modelo.
                entity.ModelDetails = GetModelDetailsDoc(entity, mainDocumentation);

                // 3. Documentación de los inputs.
                if (entity.InputDetails != null)
                {
                    entity.InputDetails = GetInputDetailsDoc(entity, mainDocumentation);
                }
                // 4. Diccionario de documentación.
                entity = GetEntityMetadataDicctionaryDocs(entity, mainDocumentation);

                return entity;
            }

            /// <summary>
            /// Asigna la documentación de cada entidad en la metadata
            /// </summary>
            /// <param name="entity">entidad en la metadata</param>
            /// <param name="mainDocumentation">documentación del modelo.</param>
            /// <returns>Entity Metadata</returns>
            public static EntityMetadata GetEntityMetadataMainDoc(EntityMetadata entity, MainDocumentation mainDocumentation)
            {

                var doc = mainDocumentation.Rels[entity.Index];
                entity.Description = doc.Description;
                entity.Title = doc.Title;
                entity.ShortName = doc.ShortName;
                entity.Info = doc;
                return entity;
            }


            /// <summary>
            /// Obtiene el detalle del modelo de una entidad, el detalle del modelo se refiere a sus propiedades y sus relaciones.
            /// </summary>
            /// <param name="entity">Entida de la emtadata</param>
            /// <param name="mainDocumentation"></param>
            /// <returns>Documentación de propiedades y relaciones de una entidad de metadata</returns>
            public static ModelDetails GetModelDetailsDoc(EntityMetadata entity, MainDocumentation mainDocumentation) => GetModelDetailsDoc(mainDocumentation, entity.ModelDetails);

            /// <summary>
            /// Asigna la documentación al model detail, que viene a ser la metadata de las propiedades.
            /// </summary>
            /// <param name="mainDocumentation">documentación principal, donde se encuentra la documentación de cada propiedad.</param>
            /// <param name="modelDetails"></param>
            /// <returns>modelo de propiedades documentadas</returns>
            public static ModelDetails GetModelDetailsDoc(MainDocumentation mainDocumentation, ModelDetails modelDetails)
            {
                // asigna la documentación a cada propiedad.
                foreach (var propModel in modelDetails.PropsDetails)
                {
                    propModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propModel.Value.KindProperty, propModel.Value.Index);
                }

                // asigna la documentación a cada propiedad de tipo enum.
                foreach (var propEnumModel in modelDetails.PropsEnumDetails)
                {
                    propEnumModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propEnumModel.Value.KindProperty, propEnumModel.Value.Index);
                }

                // asigna la documentación de cada propiedad de entidades relacionadas.
                foreach (var propRelatedModel in modelDetails.RelatedDetails)
                {
                    propRelatedModel.Value.Info = mainDocumentation.Rels[propRelatedModel.Value.RealIndex.Value];
                }

                // si no tiene propiiedades relacionadas anidadas (una propiedad de tipo entidad) retorna la documentación actual.
                if (modelDetails.RelatedModels == null || !modelDetails.RelatedModels.Any())
                {
                    return modelDetails;
                }


                var newMdlDetails = new Dictionary<string, ModelDetails>();

                // asigna recursivamente la documentación de entidades embebidas.
                foreach (var mdlDetails in modelDetails.RelatedModels)
                {
                    newMdlDetails.Add(mdlDetails.Key, GetModelDetailsDoc(mainDocumentation, mdlDetails.Value));
                }

                modelDetails.RelatedModels = newMdlDetails;

                return modelDetails;
            }

            /// <summary>
            /// Entrega la documentación de acuerdo al tipo de propiedad y su índice.
            /// </summary>
            /// <param name="mainDoc">Documentación del modelo.</param>
            /// <param name="kindProperty">Tipo de propiedad.</param>
            /// <param name="index">índice de la propiedad.</param>
            /// <returns>Info con la documentación.</returns>
            public static EntitySearchDisplayInfo GetDisplayInfoProp(MainDocumentation mainDoc, KindProperty kindProperty, int index)
            {

                // devuelve de acuerdo al índice y el tipo.
                switch (kindProperty)
                {
                    case KindProperty.STR:
                        return mainDoc.Strs[index];
                    case KindProperty.SUGGESTION:
                        return mainDoc.Strs[index];
                    case KindProperty.NUM64 | KindProperty.NUM32:
                        return mainDoc.Nums[index];
                    case KindProperty.DBL:
                        return mainDoc.Dbls[index];
                    case KindProperty.BOOL:
                        return mainDoc.Bools[index];
                    case KindProperty.GEO:
                        return mainDoc.Geos[index];
                    case KindProperty.ENUM:
                        return mainDoc.Enums[index];
                    case KindProperty.DATE:
                        return mainDoc.Dates[index];
                    default:
                        throw new CustomException("Bad ENUM");
                }
            }


            /// <summary>
            /// Obtiene el objeto InputDetails (metadata de ls inputs)
            /// y le agrega la documentación desde el modelo.
            /// </summary>
            /// <param name="entity">metadata de la entidad</param>
            /// <param name="mainDocumentation">documentación del modelo</param>
            /// <returns></returns>
            public static InputDetails GetInputDetailsDoc(EntityMetadata entity, MainDocumentation mainDocumentation)
            {
                return GetInputDetailsDoc(mainDocumentation, entity.InputDetails);
            }

            /// <summary>
            /// Asigna las propieades de documentación al modelo de inputs.
            /// </summary>
            /// <param name="mainDocumentation">documentación del modelo.</param>
            /// <param name="inputDetails">modelo de metadatos para inputs.</param>
            /// <returns>InputDetails documentado</returns>
            public static InputDetails GetInputDetailsDoc(MainDocumentation mainDocumentation, InputDetails inputDetails)
            {
                // asigna documentación a propiedades primitivas.
                foreach (var propModel in inputDetails.InputPropsDetails)
                {
                    propModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propModel.Value.KindProperty, propModel.Value.Index);
                }

                // asigna documentación e propiedades de tipo enumeración.
                foreach (var propEnumModel in inputDetails.InputEnumDetails)
                {
                    propEnumModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propEnumModel.Value.KindProperty, propEnumModel.Value.Index);
                }

                // asigna documentación a las relaciones.
                foreach (var propRelatedModel in inputDetails.InputRelatedDetails)
                {
                    propRelatedModel.Value.Info = mainDocumentation.Rels[propRelatedModel.Value.Index];
                }

                // si no existen entidades anidadas, retorna.
                if (inputDetails.RelatedInputs == null || !inputDetails.RelatedInputs.Any())
                {
                    return inputDetails;
                }

                var newInputDetails = new Dictionary<string, InputDetails>();

                // asignación de documentación recursiva para propiedades anidadas.
                foreach (var inputDetailsItem in inputDetails.RelatedInputs)
                {
                    newInputDetails.Add(inputDetailsItem.Key, GetInputDetailsDoc(mainDocumentation, inputDetailsItem.Value));
                }

                inputDetails.RelatedInputs = newInputDetails;
                return inputDetails;
            }

            /// <summary>
            /// Asigna la documentación al diccionario de propiedades.
            /// </summary>
            /// <param name="entity">entidad a documentar</param>
            /// <param name="mainDocumentation">documentación del modelo.</param>
            /// <returns></returns>
            public static EntityMetadata GetEntityMetadataDicctionaryDocs(EntityMetadata entity, MainDocumentation mainDocumentation)
            {
                // asignación de documentación a cada tipo de propiedad.
                foreach (var strData in entity.StringData)
                {
                    strData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.STR, strData.Value.Index);
                }

                foreach (var boolData in entity.BoolData)
                {
                    boolData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.BOOL, boolData.Value.Index);
                }

                foreach (var dateData in entity.DateData)
                {
                    dateData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.DATE, dateData.Value.Index);
                }

                foreach (var dblData in entity.DoubleData)
                {
                    dblData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.DBL, dblData.Value.Index);
                }
                foreach (var enmData in entity.EnumData)
                {
                    enmData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.ENUM, enmData.Value.Index);
                }

                foreach (var geoData in entity.GeoData)
                {
                    geoData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.GEO, geoData.Value.Index);
                }

                foreach (var numData in entity.NumData)
                {
                    numData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.NUM32, numData.Value.Index);
                }

                foreach (var relData in entity.relData)
                {
                    relData.Value.Info = mainDocumentation.Rels[relData.Value.Index];
                }


                return entity;
            }
        }


    }
}
