using System.Linq;
using trifenix.connect.test.model;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{
    public partial class MdmTests
    {
        public class GetTypeModel
        {

            [Fact]
            public void GetAllModels()
            {
                // assign
                var assembly = typeof(BarrackTest).Assembly;

                // action
                var types = Mdm.GetTypeModel(assembly);


                Assert.True(types != null);

                Assert.Contains(types, s => s.Name.Equals("SpecieTest"));
                Assert.Contains(types, s => s.Name.Equals("SeasonTest"));
            }
        }
    }
}
