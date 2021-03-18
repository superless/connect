using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.model;
using trifenix.connect.test.resources;
using trifenix.connect.util;
using Xunit;

namespace trifenix.connect.test.GlobalFilterTests
{
    public partial class MdmTests
    {
        public class GetGlobalFilter {

            [Fact]
            public void GetGlobalFilterFromModel() {
                // assign
                var assembly = typeof(BarrackTest).Assembly;

                // action
                var globalFilters = Mdm.GlobalFilter.GetGlobalFilter<FilterPathEnum>(assembly);



                Assert.True(globalFilters!=null);
                Assert.True(globalFilters.ToProcess.Count() == 2);
            }
        }
    }

    public partial class MdmTests
    {
        public class GetEntityCollection
        {

            [Fact]
            public void Test1()
            {
                // assign
                var assembly = typeof(BarrackTest).Assembly;

                // action
                var globalFilters = Mdm.GetEntityCollection(assembly);



                Assert.True(globalFilters != null);
                
            }
        }
    }
}
