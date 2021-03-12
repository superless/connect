using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.model;
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
                var globalFilters = Mdm.GetGlobalFilter(
                    assembly, // assembly
                    s=> new Type[] { // tipos de tipo documentDb en el assembly
                        typeof(CostCenterTest) 
                    }, 
                    x => // array de tuplas, con el tipo y ToGlobalFilterValue de CostCenter
                    x.Select(y => 
                        (y, 
                        new ToGlobalFilterValueAttribute(typeof(CostCenterTest), typeof(SeasonTest))
                        )
                    ).ToArray());



                Assert.True(globalFilters!=null);
                Assert.True(globalFilters.ToProcess.Count() == 2);
            }
        }
    }
}
