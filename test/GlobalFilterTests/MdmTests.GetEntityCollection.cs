using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.model;
using trifenix.connect.test.model_input;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{

    public partial class MdmTests
    {
        public class GetEntityCollection
        {

            [Fact]
            public void Test1()
            {
                // assign
                var assembly = typeof(BarrackTest).Assembly;

                var type = typeof(ProductInputTest);

                var entity = new EntityMetadata
                {
                    
                };


                


                // action
                var globalFilters = Mdm.GetEntityMetadata(type);



                Assert.True(globalFilters != null);
                
            }
        }
    }
}
