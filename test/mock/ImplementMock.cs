using System;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.search_mdl;

namespace trifenix.connect.tests.mock
{
    /// <summary>
    /// Una implementación permite crear un objeto entitySearch desde cero.
    /// </summary>
    public class ImplementMock : Implements<GeoPointTs>
    {
        public Type num32 => typeof(Num32BaseProperty);

        public Type dbl => typeof(DblBaseProperty);

        public Type bl => typeof(BoolBaseProperty);

        public Type num64 => typeof(Num64BaseProperty);

        public Type dt => typeof(DtBaseProperty);

        public Type enm => typeof(EnumBaseProperty);

        public Type rel => typeof(RelatedBaseId);

        public Type str => typeof(StrBaseProperty);

        public Type sug => typeof(StrBaseProperty);

        public Type geo => typeof(GeoPropertyMock);


        // refactorizar.
        public Func<object, GeoPointTs> GeoObjetoToGeoSearch => (ob) => new GeoPointTs { latitude =0, longitude = 0 };

        public Type GetEntitySearchImplementedType => typeof(EntityMockSearch);
    }
}
