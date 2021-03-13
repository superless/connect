using System;
using System.Linq;
using trifenix.connect.test.model;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{
    public partial class MdmTests
    {
        public class ToValueTypes
        {

            [Fact]
            public void GetClassCostCenter()
            {
                // assign
                var types = new Type[] {
                    typeof(CostCenterTest)
                };

                // action
                var toValues = Mdm.GlobalFilter.ToValueTypes(types).ToList();

                var (typeToValue, toValueAttr) = toValues.FirstOrDefault();


                // assert
                Assert.True(typeToValue.Name.Equals("CostCenterTest"));

                Assert.True(toValueAttr.TargetType.Equals(typeof(SeasonTest)));






            }
        }
    }
}
