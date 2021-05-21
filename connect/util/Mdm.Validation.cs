using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trifenix.connect.input;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;

namespace trifenix.connect.util
{
    public partial class Mdm
    {

        /// <summary>
        /// Validación de módelods IMd
        /// </summary>
        public static class Validation {


            
            /// <summary>
            /// Compara un input model con un modelo, para validar si los datos que están en el input coinciden con el del modelo.
            /// </summary>
            /// <param name="input">objeto input de ingreso</param>
            /// <param name="model">modelo a validar</param>
            /// <param name="bind">Colección de funciones, que tomar un input y devuelven una colección de posibles valores que puedan coincidir, es un prefiltro. el método los seleccionará y verá si alguno tiene los mismos valores, si la función no devuelve al menos un modelo correspondiente al input fallará</param>
            /// <returns>true, si los campos coinciden</returns>
            public static bool CompareModel(object input, object model,  Dictionary<Type, Func<object, IEnumerable<object>>> bind)  {

                

                // busca si existen propieades con metadata.
                var values = Reflection.Attributes.GetPropertiesByAttributeWithValue(input);



                // busqueda de propiedades que no tengan atributos mdm, en busqueda de locals.
                var valuesWithoutProperty = Reflection.Attributes.GetPropertiesWithoutAttributeWithValues(input).Where(s => !Mdm.Reflection.IsPrimitiveAndCollection(s.GetType()) && !s.GetType().IsEnum);



                // toma todos los valores de propiedad que sean de tipo local reference o no tengan atributos de metadata y que los valores deben ser clases y no valores primitivos, para poder identificar entidades locales.
                // el método recorrerá el objeto y verificará que tenga el atributo que lo identifique como entidad, sino lo tiene no será reconocido como entidad, no importa si la propiedad tiene el atributo de entidad local.
                // el atributo de la clase es el que vale (EntityIndexAttribute).
                var posibleLocals = valuesWithoutProperty.Union(values.Where(s => !Mdm.Reflection.IsPrimitiveAndCollection(s.Value.GetType()) && !s.Value.GetType().IsEnum).Select(s => s.Value)).ToList();



                // si no existen valores locales, ni tampoco valores de entitySearch, devolverá true.
                if (!posibleLocals.Any() && !values.Any()) return true;


                // valores de tipo referencia o valor
                if (values.Any())
                {

                    // obtiene los valores que sean datos primitivos o enumeraciones, dejando fuera los locals.
                    var valuesWithoutLocals = values.Where(s => Mdm.Reflection.IsPrimitiveAndCollection(s.Value.GetType()) || s.Value.GetType().IsEnum);


                    // obtiene los valores de referencia que incluye un input.
                    var relatedInputs = GetPrimitiveValues<string>(input, true);


                    // incluye las cadenas de texto que pudiese tener un input
                    var stringInputs = GetPrimitiveValues<string>(input, false, KindProperty.STR);


                    // devuelve las sugerencias que puede tener un input.
                    var suggestInputs = GetPrimitiveValues<string>(input, false, KindProperty.SUGGESTION);


                    // devuelve los num32 que podría tener un input.
                    var num32Inputs = GetPrimitiveValues<int>(input, false, KindProperty.NUM32);


                    // devuelve los num64 que podría tener un input.
                    var num64Inputs = GetPrimitiveValues<long>(input, false, KindProperty.NUM64);


                    // devuelve los valores dobles que pudiese tener un input
                    var dblsInputs = GetPrimitiveValues<double>(input, false, KindProperty.DBL);


                    // devuelva los valores fecha que pudiesen estar en el input.
                    var datesInputs = GetPrimitiveValues<DateTime>(input, false, KindProperty.DATE);


                    // devuelva los valores booleanos que pudiesen estar en el input.
                    var blsInputs = GetPrimitiveValues<bool>(input, false, KindProperty.BOOL);


                    // devuelva los valores de enumeraciones que pudiesen estar en el input.
                    var enumInputs = GetPrimitiveValues<int>(input, false, KindProperty.ENUM);


                    // lo mismo anterior pero del modelo a validar.

                    var relateds = GetPrimitiveValues<string>(model, true);

                    var strings = GetPrimitiveValues<string>(model, false, KindProperty.STR);

                    var suggests = GetPrimitiveValues<string>(model, false, KindProperty.SUGGESTION);

                    var num32 = GetPrimitiveValues<int>(model, false, KindProperty.NUM32);

                    var num64 = GetPrimitiveValues<long>(model, false, KindProperty.NUM64);


                    var dbls = GetPrimitiveValues<double>(model, false, KindProperty.DBL);


                    var bls = GetPrimitiveValues<bool>(model, false, KindProperty.BOOL);


                    var enums = GetPrimitiveValues<int>(model, false, KindProperty.ENUM);


                    var dates = GetPrimitiveValues<DateTime>(model, false, KindProperty.DATE);                    
                    
                    
                    // Compara todos los tipos de datos.
                    if (!CompareValueContainer(relatedInputs, relateds)) return false;

                    if (!CompareValueContainer(stringInputs, strings)) return false;

                    if (!CompareValueContainer(suggestInputs, suggests)) return false;

                    if (!CompareValueContainer(num32Inputs, num32)) return false;

                    if (!CompareValueContainer(num64Inputs, num64)) return false;


                    if (!CompareValueContainer(dblsInputs, dbls)) return false;

                    if (!CompareValueContainer(datesInputs, dates)) return false;

                    if (!CompareValueContainer(blsInputs, bls)) return false;

                    if (!CompareValueContainer(enumInputs, enums)) return false; 
                }

                // geo queda pendiente



                // esta es una función recursiva
                // buscará todos los valores no primitivos que pudiesen tener datos primitivos dentro.
                if (posibleLocals.Any()) {

                    // recorre los datos no primitivos de un objeto para la recursividad
                    foreach (var item in posibleLocals)
                    {
                        // obtiene el tipo de dato de cada posble local.
                        var tp = item.GetType();
                        
                        // verifica si existe en el diccionario dado como parámetro el tipo a buscar.
                        // el diccionario tiene una función que permite entregar una serie de modelos que puedan coincidir
                        // si alguno de los modelos entregados coincide con los valores retorna ok, sino entregará que no es válido.
                        if (bind.ContainsKey(tp))
                        {

                            // invoca el método con el o los modelos que se compararan
                            var mdls = bind[item.GetType()].Invoke(item);


                            // si el valor es enumerable se recorrerá para verificar si uno de los modelos coincide.
                            if (Reflection.IsEnumerable(item))
                            {
                                // convierte en colección
                                var itemCollection = (IEnumerable<object>)item;

                                // recorre la colección y revisa uno por uno.
                                foreach (var collectionItem in itemCollection)
                                {
                                    // revisa todos los modelos con cada item.
                                    var resultLocal = mdls.Any(m => CompareModel(collectionItem, m, bind));

                                    // si todos los modelos ingresados no coinciden, es decir tienen distintos valores en sus campos, retornará false.
                                    if (!resultLocal)
                                    {
                                        return false;
                                    }
                                }
                            }
                            // si no es colección, tomará solo el valor.
                            else {
                                var result = mdls.Any(m => CompareModel(item, m, bind));
                                if (!result)
                                {
                                    return false;
                                }
                            }

                            
                        }

                    }
                }

                return true;
            }


            /// <summary>
            /// Compara dos contenedores de valores.
            /// </summary>
            /// <typeparam name="T">Las colecciones serán del tipo que se indique</typeparam>
            /// <param name="input">colección de propiedades y valores de un objeto input</param>
            /// <param name="model">colección de propiedades y valores de un objeto model</param>
            /// <returns>true, si los valores coinciden</returns>
            private static bool CompareValueContainer<T>(ValueContainer<T> input, ValueContainer<T> model) {

                


                // recorre todos los valores primitivos del tipo (valores primitivos de tipo individual no colección)
                foreach (var item in model.Primitives)
                {

                    // obtiene el valor del modelo en el input
                    var inputValues = input.Primitives.FirstOrDefault(a=>a.Key.Equals(item.Key));


                    // si encuentra el valor en el input. lo compara.
                    // pueden existir valores del modelo que pudiesen no estar en el input, como por ejemplo ClienteId.
                    if (!inputValues.Equals(default(KeyValuePair<int, T>)))
                    {
                        // false si es distinto.
                        var result = inputValues.Value.Equals(item.Value);
                        if (!result)
                        {
                            return false;
                        }
                    }
                }


                foreach (var item in model.PrimitiveCollections)
                {
                    var inputValues = input.PrimitiveCollections.FirstOrDefault(a => a.Key.Equals(item.Key));

                    if (!inputValues.Equals(default(KeyValuePair<int, IEnumerable<T>>)))
                    {
                        var result =  inputValues.Value.All(a => inputValues.Value.Any(s => s.Equals(a)));
                        if (!result)
                        {
                            return false;
                        }
                    }

                    
                }

                return true;
            }


            private static ValueContainer<T> GetValueContainer<T>(IEnumerable<KeyValuePair<BaseIndexAttribute, object>> collection) => new ValueContainer<T>
            {
                Primitives = collection.Where(v => !Mdm.Reflection.IsPrimitiveCollection(v.Value.GetType())).ToDictionary(k => k.Key.Index, v => (T)v.Value),
                PrimitiveCollections = collection.Where(v => Mdm.Reflection.IsPrimitiveCollection(v.Value.GetType())).ToDictionary(k => k.Key.Index, v => (IEnumerable<T>)v.Value)

            };


          
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Mdm.Validation.GetPrimitiveValues<T>(object, bool, KindProperty)'
            public static ValueContainer<T> GetPrimitiveValues<T>(object input,  bool isEntity = false, KindProperty kindProperty = KindProperty.BOOL) {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Mdm.Validation.GetPrimitiveValues<T>(object, bool, KindProperty)'

                var values = Reflection.Attributes.GetPropertiesByAttributeWithValue(input);


                var valuesWithoutLocals = values.Where(s => Mdm.Reflection.IsPrimitiveAndCollection(s.Value.GetType()) || s.Value.GetType().IsEnum);


                if (isEntity) return GetValueContainer<T>(valuesWithoutLocals.Where(g => g.Key.IsEntity));

                return GetValueContainer<T>(valuesWithoutLocals.Where(g => !g.Key.IsEntity && g.Key.KindIndex == (int)kindProperty ));


            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>'
        public class ValueContainer<T> {
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>.Primitives'
            public Dictionary<int, T> Primitives { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>.Primitives'


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>.PrimitiveCollections'
            public Dictionary<int, IEnumerable<T>> PrimitiveCollections { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Mdm.ValueContainer<T>.PrimitiveCollections'
        }

    }
}
