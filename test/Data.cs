using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.ts_model;

namespace trifenix.connect.test
{
    public static  class Data
    {

        public static GlobalFilters GetModelGlobalFilter => new GlobalFilters
        {
            EntityForGlobalFilters = "Barrack",
            IndexEntityForGlobalFilters = 1,
            ToValue = new Dictionary<string, ToValue> {
                {"costCenterTest-seasonTest", new ToValue{ 
                        OriginClass="CostCenterTest",
                        OriginIndex = 5,
                        ValueClass = "SeasonTest",
                        ValueIndex = 15,
                        FiltersToValue = new PathToFiltersValue[]{ 
                            new PathToFiltersValue{
                                OriginClass="CostCenterTest",
                                OriginIndex = 5,
                                TargetClass = "SeasonTest",
                                TargetIndex = 15,
                                PropertyName = "IdCostCenter"
                            }
                        }
                    } 
               }
            },
            ToProcess = new 
        };
    }
}
