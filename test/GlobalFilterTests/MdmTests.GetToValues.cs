using System;
using System.Linq;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.model;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{
    public partial class MdmTests
    {
        public class GetToValues
        {

            [Fact]
            public void GetToValuesToGlobalFilter()
            {
                // assign
                var tuplas = new (Type type, ToGlobalFilterValueAttribute tov)[] {
                    (typeof(CostCenterTest), 
                    new ToGlobalFilterValueAttribute(
                        typeof(CostCenterTest), 
                        typeof(SeasonTest))
                    )
                };

                var types = new Type[]{
                    typeof(CostCenterTest),typeof(SeasonTest)
                };

                // action
                var toValueResult = Mdm.GetToValues(types, tuplas).ToList();



                var globalFilterExpect = Data.GetModelGlobalFilter;

                var toValueExpected = globalFilterExpect.ToValue;


                // assert
                // key correcta
                var keyResult = toValueResult.First().Key;
                var keyExpected = toValueExpected.First().Key;



                Assert.Equal(keyResult, keyExpected);

                






            }
        }
    }
}
