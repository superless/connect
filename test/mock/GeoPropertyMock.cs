using System;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.test.model;

namespace trifenix.connect.tests.mock
{
    /// <summary>
    /// implementación mocking de propiedad geo.
    /// </summary>
    public class GeoPropertyMock : IProperty<GeoPointTs>
    {
        /// <summary>
        /// índice de una propiedad geo
        /// </summary>
        /// <value></value>
        public int index { get; set; }
        
        /// <summary>
        /// valor de propiedad geo
        /// </summary>
        /// <value></value>
        public GeoPointTs value { get; set; }
    }
}
