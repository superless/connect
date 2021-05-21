using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using trifenix.connect.mdm.resources;
using trifenix.connect.mdm.ts_model;
using static trifenix.connect.util.Mdm;

namespace trifenix.connect.test.resources.filters
{
    public class FilterDetails : IFilterProcessDescription
    {
        public DocFilter GetFilterProcessDescription(int index)
        {
            var name = Enum.GetName(typeof(FilterPathEnum), index);

            var description = new ResourceManager(typeof(filters)).GetString(name);

            return new DocFilter
            {
                Index = index,
                EnumDescription = description,
                EnumName = Reflection.Enumerations.Description((FilterPathEnum)index)
            };
        }
    }


}
