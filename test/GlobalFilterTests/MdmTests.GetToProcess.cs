using trifenix.connect.test.model;
using trifenix.connect.test.resources;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{
    public partial class MdmTests
    {
        public class GetToProcess {
            [Fact]
            public void GetToValuesToGlobalFilter()
            {
                // assign

                var assembly = typeof(BarrackTest).Assembly;


                // action
                var processResult = Mdm.GetFilterProcess<FilterPathEnum>(Mdm.GetTypeModel(assembly), true);



                var globalFilterExpect = Data.GetModelGlobalFilter;

                var processExpected = globalFilterExpect.ToProcess;


                // assert
               

                Assert.Equal(processResult[1].SourceIndex, processExpected[1].SourceIndex);








            }

        }
    }
}
