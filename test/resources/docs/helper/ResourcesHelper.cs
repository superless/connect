using System;
using trifenix.connect.agro.resources.mdl;
using trifenix.connect.mdm.enums;
using trifenix.connect.test.enums;
using trifenix.connect.test.resources.docs.dictionary.booleans;
using trifenix.connect.test.resources.docs.dictionary.dates;
using trifenix.connect.test.resources.docs.dictionary.doubles;
using trifenix.connect.test.resources.docs.dictionary.entities;
using trifenix.connect.test.resources.docs.dictionary.enums;
using trifenix.connect.test.resources.docs.dictionary.geos;
using trifenix.connect.test.resources.docs.dictionary.nums;
using trifenix.connect.test.resources.docs.dictionary.strings;

namespace trifenix.connect.agro.resources.helper
{
    public static class ResourcesHelper {
        public static string GetName(KindProperty related, int index)
        {
            switch (related)
            {
                case KindProperty.STR:
                case KindProperty.SUGGESTION:
                    return Enum.GetName(typeof(StringRelated), index);
                case KindProperty.NUM64:
                case KindProperty.NUM32:
                    return Enum.GetName(typeof(NumRelated), index);
                case KindProperty.DBL:
                    return Enum.GetName(typeof(DoubleRelated), index);
                case KindProperty.BOOL:
                    return Enum.GetName(typeof(BoolRelated), index);
                case KindProperty.GEO:
                    return Enum.GetName(typeof(GeoRelated), index);
                case KindProperty.ENUM:
                    return Enum.GetName(typeof(EnumRelated), index);
                default:
                    return Enum.GetName(typeof(DateRelated), index);

            }

        }
        public static string GetEntityName(int index) => Enum.GetName(typeof(EntityRelated), index);

        public static ResourceModel GetEntityResourceCollection()
        {
            return new ResourceModel
            {
                Columns = null,
                Titles = typeof(entity_titles),
                Descriptions = typeof(entity_descriptions),
                ShortNames = typeof(entity_snames),
                PlaceHolders = typeof(entity_placeholders)
            };
        }
        public static ResourceModel GetResourceCollection(KindProperty related)
        {
            switch (related)
            {
                case KindProperty.STR:
                case KindProperty.SUGGESTION:
                    return new ResourceModel
                    {
                        Columns = typeof(string_columns),
                        Titles = typeof(string_titles),
                        Descriptions = typeof(string_descriptions),
                        ShortNames = typeof(string_snames),
                        PlaceHolders = typeof(string_placeholders),
                    };
                case KindProperty.NUM32:
                case KindProperty.NUM64:
                    return new ResourceModel
                    {
                        Columns = typeof(num_columns),
                        Titles = typeof(num_titles),
                        Descriptions = typeof(num_descriptions),
                        ShortNames = typeof(num_snames),
                        PlaceHolders = typeof(num_placeholders)
                    };
                case KindProperty.DBL:
                    return new ResourceModel
                    {
                        Columns = typeof(double_columns),
                        Titles = typeof(double_titles),
                        Descriptions = typeof(double_descriptions),
                        ShortNames = typeof(double_snames),
                        PlaceHolders = typeof(double_placeholders),
                    };
                case KindProperty.BOOL:
                    return new ResourceModel
                    {
                        Columns = typeof(boolean_columns),
                        Titles = typeof(boolean_titles),
                        Descriptions = typeof(boolean_descriptions),
                        ShortNames = typeof(boolean_snames),
                        PlaceHolders = typeof(boolean_placeholder)
                    };

                case KindProperty.GEO:
                    return new ResourceModel
                    {
                        Columns = typeof(geo_columns),
                        Titles = typeof(geo_titles),
                        Descriptions = typeof(geo_descriptions),
                        ShortNames = typeof(geo_snames),
                        PlaceHolders = typeof(geo_placeholders),
                    };
                case KindProperty.ENUM:
                    return new ResourceModel
                    {
                        Columns = typeof(enum_columns),
                        Titles = typeof(enum_titles),
                        Descriptions = typeof(enum_descriptions),
                        ShortNames = typeof(enum_snames),
                        PlaceHolders = typeof(enum_placeholders),
                    };

                default:
                    return new ResourceModel
                    {
                        Columns = typeof(date_columns),
                        Titles = typeof(date_titles),
                        Descriptions = typeof(date_descriptions),
                        ShortNames = typeof(date_snames),
                        PlaceHolders = typeof(date_placeholders),
                    };
            }

        }
    }

    


}
