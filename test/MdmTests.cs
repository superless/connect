using Newtonsoft.Json;
using System;
using System.Linq;

using trifenix.connect.interfaces.hash;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.search_mdl;
using trifenix.connect.test.enums;
using trifenix.connect.test.model;
using trifenix.connect.tests.mock;
using trifenix.connect.util;
using Xunit;


namespace trifenix.connect.test
{
    /// <summary>
    /// Test de transformación de modelo de base de datos a entitySearch
    /// </summary>
    public class MdmTests
    {

        /// <summary>
        /// Convierte un model de objeto de base de datos a un entitySearch
        /// </summary>
        [Fact]
        public void ConvertObjectoToEntity() {

            //new specie
            var objsSpecie = new SpecieTest()
            {
                Id = Guid.NewGuid().ToString("N"),
                ClientId = "2",
                Name = "Specie",
                Abbreviation = "SPC"
            };
            // generación de hash
            var hashSpecie = new HashEntityMockSearch();

            // action
            // obtiene el entitySearch
            var entityspecie = Mdm.GetEntitySearch(new ImplementMock(), objsSpecie, hashSpecie);

            // genera el hash de las cabeceras de entitySearch
            // esto es un diccionario
            var hhSpecie = hashSpecie.HashHeader(typeof(SpecieTest));

            // genera un hash del objeto de base de datos.
            var hmSpecie = hashSpecie.HashModel(objsSpecie);

            // obtiene el primer elemento de tipo barracks
            var specieEntityFather = entityspecie.First(s => s.index == (int)EntityRelated.SPECIE);

            // assert
            // verifica que el hash sea correcto
            Assert.True(specieEntityFather.hh.Equals(hhSpecie) && specieEntityFather.hm.Equals(hmSpecie));



            //new variety
            var objsVariety = new VarietyTest()
            {
                Id = Guid.NewGuid().ToString("N"),
                ClientId = "3",
                Name = "Variety",
                Abbreviation = "VRTY",
                IdSpecie = objsSpecie.Id
            };
            // generación de hash
            var hashVariety = new HashEntityMockSearch();

            // action
            // obtiene el entitySearch
            var entityVariety = Mdm.GetEntitySearch(new ImplementMock(), objsVariety, hashVariety);

            // genera el hash de las cabeceras de entitySearch
            // esto es un diccionario
            var hhVariety = hashVariety.HashHeader(typeof(VarietyTest));

            // genera un hash del objeto de base de datos.
            var hmVariety = hashVariety.HashModel(objsVariety);

            // obtiene el primer elemento de tipo barracks
            var VarietyEntityFather = entityVariety.First(s => s.index == (int)EntityRelated.VARIETY);

            // assert
            // verifica que el hash sea correcto
            Assert.True(VarietyEntityFather.hh.Equals(hhVariety) && VarietyEntityFather.hm.Equals(hmVariety));






            // new BarrackTest
            var objs = new BarrackTest()
            {
                ClientId = "1",
                GeographicalPoints = new GeoPointTs[] { new GeoPointTs { latitude = 1.3, longitude = 1.45 } },
                Hectares = 3.4,
                Id = Guid.NewGuid().ToString("N"),
                IdPlotLand = Guid.NewGuid().ToString("N"),
                IdPollinator = Guid.NewGuid().ToString("N"),
                IdRootstock = Guid.NewGuid().ToString("N"),
                IdVariety = objsVariety.Id,
                Name = "Barrack1",
                NumberOfPlants = 1221,
                PlantingYear = 1982,
                SeasonId = Guid.NewGuid().ToString("N")
            };
            // generación de hash
            var hash = new HashEntityMockSearch();

            // action
            // obtiene el entitySearch
            var entities = Mdm.GetEntitySearch(new ImplementMock(), objs, hash);

            // genera el hash de las cabeceras de entitySearch
            // esto es un diccionario
            var hh = hash.HashHeader(typeof(BarrackTest));

            // genera un hash del objeto de base de datos.
            var hm = hash.HashModel(objs);


            // obtiene el primer elemento de tipo barracks
            var barrackEntityFather = entities.First(s => s.index == (int)EntityRelated.BARRACK);



            // assert
            // verifica que el hash sea correcto
            Assert.True(barrackEntityFather.hh.Equals(hh) && barrackEntityFather.hm.Equals(hm));
        }


        /// <summary>
        /// convierte una entidad en un objeto.
        /// </summary>
        [Fact]
        public void ConvertEntityToObject() {

            // operaciones de hash para crear las identidades.
            var hash = new HashEntityMockSearch();


            // un id genérico.
            var genericId = Guid.NewGuid().ToString("N");


            // objeto del modelo de base de datos.
            var objs = new BarrackTest()
            {
                ClientId = "1",
                GeographicalPoints = new GeoPointTs[] { new GeoPointTs { latitude = 1.3, longitude = 1.45 } },
                Hectares = 3.4,
                Id = genericId,
                IdPlotLand = genericId,
                IdPollinator = genericId,
                IdRootstock = genericId,
                IdVariety = genericId,
                Name = "Barrack1",
                NumberOfPlants = 1221,
                PlantingYear = 1982,
                SeasonId = genericId
            };


            
            // Creamos una entidad que coincide con el objeto de modelo de base de datos.
            var entity = new EntityBaseSearch<GeoPointTs>
            {
                bl = Array.Empty<BoolBaseProperty>(), 
                created = DateTime.Now,
                index = (int)EntityRelated.BARRACK,
                dbl = new DblBaseProperty[] { new DblBaseProperty { index = (int)DoubleRelated.HECTARES, value = 3.4 } },
                dt = Array.Empty<DtBaseProperty>(),
                enm = Array.Empty<EnumBaseProperty>(),
                geo = new GeographyProperty[] { new GeographyProperty { index = (int)GeoRelated.LOCATION_BARRACK, value = new GeoPointTs { latitude = 1.3, longitude = 1.45 } } },
                id = genericId,
                num32 = new Num32BaseProperty[] { new Num32BaseProperty { index = (int)NumRelated.NUMBER_OF_PLANTS, value = 1221 }, new Num32BaseProperty { index = (int)NumRelated.PLANTING_YEAR, value = 1982 } },
                num64 = Array.Empty<Num64BaseProperty>(),
                rel = new IRelatedId[] {
                    new RelatedBaseId{ id = genericId, index = (int)EntityRelated.PLOTLAND },
                    new RelatedBaseId{ id = genericId, index = (int)EntityRelated.POLLINATOR },
                    new RelatedBaseId{ id = genericId, index = (int)EntityRelated.ROOTSTOCK },
                    new RelatedBaseId { id = genericId, index = (int)EntityRelated.VARIETY },
                    new RelatedBaseId { id = genericId, index = (int)EntityRelated.SEASON },
                },
                str = new StrBaseProperty[] { new StrBaseProperty { index = (int)StringRelated.GENERIC_NAME, value = "Barrack1",  },  new StrBaseProperty { index = (int)StringRelated.GENERIC_CORRELATIVE, value = "1" } },
                sug = Array.Empty<StrBaseProperty>(),
                hh = hash.HashHeader(typeof(BarrackTest)),
                hm = hash.HashModel(objs)
            };

            // obtenemos el entitySearch del barrack, internamente valida el hash.
            var barrack = (BarrackTest)Mdm.GetEntityFromSearch(entity, typeof(BarrackTest), "trifenix.connect.test.model", s => s, new SearchElement(), hash);


            //assert
            Assert.Equal(entity.str.First().value, barrack.Name);


        }


    }

    public class SearchElement : ISearchEntity<GeoPointTs>
    {
        public IEntitySearch<GeoPointTs> GetEntity(int entityKind, string idEntity)
        {
            return new EntityBaseSearch<GeoPointTs>();
        }
    }


    public class HashEntityMockSearch : IHashSearchHelper
    {
        /// <summary>
        /// obtiene un hash desde el diccionario generado a partir de un elemento de base de datos. 
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string HashHeader(Type type)
        {
            // obtenemos los diccionarios desde las enumeraciones.
            var dictRel = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<EntityRelated>();
            var dictEnum = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<EnumRelated>();
            var dictNum = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<NumRelated>();
            var dictStr = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<StringRelated>();
            var dictDbl = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<DoubleRelated>();
            var dictDt = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<DateRelated>();
            var dictGeo = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<GeoRelated>();
            var dictBool = Mdm.Reflection.Enumerations.GetDictionaryFromEnum<BoolRelated>();

            // obtiene los índices de la clase
            var indexes = Mdm.PreLoadedDictionary(type);

            if (indexes == null)
            {
                return string.Empty;
            }

            // asigna colecciones de dictionary de los índices de propiedades de un objeto de una base de datos de persistencia.
            var dict = new JsonDictionaryHeaders
            {
                bl = indexes.bl.ToDictionary(s => s, s => dictBool[s]),
                dbl = indexes.dbl.ToDictionary(s => s, s => dictDbl[s]),
                dt = indexes.dt.ToDictionary(s => s, s => dictDt[s]),
                enm = indexes.enm.ToDictionary(s => s, s => dictEnum[s]),
                geo = indexes.geo.ToDictionary(s => s, s => dictGeo[s]),
                index = indexes.index,
                num64 = indexes.num32.ToDictionary(s => s, s => dictNum[s]),
                num32 = indexes.num64.ToDictionary(s => s, s => dictNum[s]),
                rel = indexes.rel.ToDictionary(s => s, s => dictRel[s]),
                str = indexes.str.ToDictionary(s => s, s => dictStr[s]),
                sug = indexes.sug.ToDictionary(s => s, s => dictStr[s])
            };




            // serializa para el hash
            var jsonDict = JsonConvert.SerializeObject(dict);


            // retorna hash
            return Mdm.Reflection.Cripto.ComputeSha256Hash(jsonDict);
        }

        /// <summary>
        /// Obtiene un hash desde un objeto
        /// </summary>
        /// <param name="model">elemento de base de datos a validar</param>
        /// <returns>hash único del elemento</returns>
        public string HashModel(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return Mdm.Reflection.Cripto.ComputeSha256Hash(JsonConvert.SerializeObject(obj));
        }


    }
}
