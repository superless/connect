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
                        PathToProcess = new PathToFiltersValue[]{ 
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
            ToProcess = new ToProcessClass[] { 
                new ToProcessClass{ 
                    Index = 0,
                    Name = "GlobalFilter",
                    SourceIndex = 15,
                    TargetIndex = 1,
                    SourceName = "SeasonTest",
                    TargetName = "BarrackTest",
                    TargetRealIndex = 1,
                    PathToProcess = new PathToFiltersValue[]{ 
                        new PathToFiltersValue{ 
                            OriginClass = "SeasonTest",
                            OriginIndex = 15,
                            TargetIndex = 1,
                            TargetClass = "BarrackTest",
                            PropertyName = "SeasonId"
                        }
                    }
                },
                new ToProcessClass{ 
                    Index = 0,
                    Name="GlobalFilter",
                    SourceIndex=30,
                    SourceName="SpecieTest",
                    TargetIndex=1,
                    TargetName="BarrackTest",
                    TargetRealIndex = 1,
                    PathToProcess = new PathToFiltersValue[]{ 
                        new PathToFiltersValue{ 
                            OriginClass = "SpecieTest",
                            OriginIndex = 30,
                            TargetIndex = 21,
                            TargetClass="VarietyTest",
                            PropertyName="IdSpecie"
                        },
                        new PathToFiltersValue{
                            OriginClass = "VarietyTest",
                            OriginIndex = 21,
                            TargetIndex = 1,
                            TargetClass="BarrackTest",
                            PropertyName="IdVariety"
                        }
                    }
                }
            }
        };
    }
}
