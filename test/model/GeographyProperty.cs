using trifenix.connect.mdm.entity_model;

namespace trifenix.connect.test.model
{
    /// <summary>
    /// Clase de tipo geo, que será generada como componente typescript
    /// para identificar un campo de tipo geo.
    /// También se usa para test
    /// </summary>
    public class GeographyProperty : IProperty<GeoPointTs>
    {
        /// <summary>
        /// índice de la propiedad de tipo geo
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// valor de la propiedad de tipo geo.
        /// </summary>
        public GeoPointTs value { get; set; }



    }
}
