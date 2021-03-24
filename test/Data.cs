using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.model;
using trifenix.connect.ts_model.enums;

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

        public static EntityMetadata GetProductMetadata => new EntityMetadata
        {
            AutoNumeric = true,
            StringData= new Dictionary<int, PropertyMetadata> {
                { 1, new PropertyMetadata{ 
                    AutoNumeric = false,
                    KindProperty = KindProperty.STR,
                    ClassName = "ProductTest",                    
                    HasInput = true,
                    Index = 1,
                    IndexFather = 12,
                    Info = new EntitySearchDisplayInfo{ 
                        Column = "Nom",
                        Description = "Nombre genérico de una entidad",
                        PlaceHolder = "Indique el nomnbre",
                        ShortName = "Nom",
                        Title ="Nombre"
                        
                    },                    
                    isArray = false,
                    NameProp="Name",
                    
                    Required = true,
                    Unique = true,
                    TypeString = "string",
                    Visible = true
                   
                }},
                { 14, new PropertyMetadata{
                    AutoNumeric = false,
                    ClassName = "ProductTest",
                    KindProperty = KindProperty.STR,
                    HasInput = true,
                    Index = 14,
                    IndexFather = 12,
                    Info = new EntitySearchDisplayInfo{
                        Column = "Sag N°",
                        Description = "Código SAG",
                        PlaceHolder = "Indique el nombre dél código SAG",
                        ShortName = "SAG N",
                        Title ="Código SAG"

                    },
                    
                    isArray = false,
                    NameProp="SagCode",                    
                    Required = true,
                    Unique = true,
                    TypeString = "string",
                    Visible = true

                }},
                 { 13, new PropertyMetadata{
                    AutoNumeric = true,
                    ClassName = "ProductTest",
                    KindProperty=KindProperty.STR,
                    HasInput = false,
                    Index = 13,
                    IndexFather = 12,
                    Info = new EntitySearchDisplayInfo{
                        Column = "N°",
                        Description = "Identificador de producto",
                        PlaceHolder = "",
                        ShortName = "N°",
                        Title ="Id Producto"

                    },
                    
                    isArray = false,
                    NameProp="ClientId",                    
                    Required = false,
                    Unique = false,
                    TypeString = "string",
                    Visible = true
                    

                }},
            },
            EnumData = new Dictionary<int, PropertyMetadadataEnum> {
                { 0, new PropertyMetadadataEnum {
                    AutoNumeric = false,
                    ClassName = "ProductTest",
                    KindProperty = KindProperty.ENUM,
                    EnumData = new Dictionary<int, string>{
                        { 0, "gr."},
                        { 1, "cc."},
                    },
                    IndexFather = 12,
                    Index = 0,
                    HasInput = true,
                    Info =  new EntitySearchDisplayInfo{
                        Column = "Tipo M",
                        Description = "Tipo de medida para producto",
                        PlaceHolder = "Indique el tipo de medida",
                        ShortName = "Tipo",
                        Title ="Tipo de medida de producto"

                    },
                    isArray = false,
                    NameProp="MeasureType",                    
                    Required = true,
                    TypeString = "MeasureType",
                    Unique = false,
                    Visible = true,

                }},
           

            },
            relData = new Dictionary<int, RelatedPropertyMetadata> {
                { 7, new RelatedPropertyMetadata{ 
                    AutoNumeric = false,
                    Info = new EntitySearchDisplayInfo{ 
                        Column = "Ingrediente",
                        Description="Ingrediente de productos",
                        PlaceHolder = "Ingrese ingrediente",
                        ShortName = "ING",
                        Title = "Ingrediente Activo"
                    },
                    ClassName = "ProductTest",                    
                    ClassNameTarget="IngredientTest",                    
                    IsReference = true,
                    HasInput = true,
                    Index = 7,
                    IndexFather = 12,
                    isArray = false,
                    NameProp="IdActiveIngredient",
                    RealIndex = 7,
                    Required=false,
                    TypeString = "string",
                    Unique = false,
                    Visible = true
                } },
                { 32, new RelatedPropertyMetadata{
                    AutoNumeric = false,
                    Info = new EntitySearchDisplayInfo{
                        Column = "Marca",
                        Description="Marca de producto",
                        PlaceHolder = "Ingrese marca",
                        ShortName = "marca",
                        Title = "Marca"
                    },
                    ClassName = "ProductTest",
                    ClassNameTarget = "BrandTest",
                    IsReference = true,
                    HasInput = true,
                    Index = 32,
                    IndexFather = 12,
                    isArray = false,
                    NameProp="IdBrand",
                    RealIndex = 32,
                    Required=false,
                    TypeString = "string",
                    Unique = false,
                    Visible = true
                } },
            },
            Menus = new GroupMenu[] { },
            IsGlobalFilterValue = false,
            PathName="products",
            Info = new EntitySearchDisplayInfo { 
                Column="Producto",
                Description="Producto",
                PlaceHolder ="Ingrese Producto",
                ShortName="Prod",
                Title = "Producto"
            },
            Index = 12,
            ReadOnly=false,
            FiltersProcess = new FilterProcess[] { },
            FiltersAvailable = new RelatedItem[] { },
            EntityKind = EntityKind.CUSTOM_ENTITY,
            ClassInputName="ProductInputTest",
            ClassName="ProductTest",
            Description="Producto",
            ShortName="Producto",
            DeleteItems = new DeleteItem[] { },
            Title="Producto",
            ToProcessClass = new ToProcessClass[] { },
            Visible = true,
            InputDetails = new InputDetails { 
                InputPropsDetails = new Dictionary<string, InputPropDetails> {
                    { "Name",  new InputPropDetails{
                            AutoNumeric = false,
                            KindProperty = KindProperty.STR,
                            ClassName = "ProductTest",
                            ClassInput="ProductInputTest",
                            HasInput = true,
                            Index = 1,
                            IndexFather = 12,                            
                            Info = new EntitySearchDisplayInfo{
                                Column = "Nom",
                                Description = "Nombre genérico de una entidad",
                                PlaceHolder = "Indique el nomnbre",
                                ShortName = "Nom",
                                Title ="Nombre"
                            },
                            isArray = false,
                            NameProp="Name",
                            Required = true,
                            Unique = true,
                            TypeString = "string",
                            Visible = true,
                            ModelPropName = "Name"
                            
                      } },
                      { "SagCode", new InputPropDetails{
                            AutoNumeric = false,
                            ClassName = "ProductTest",
                            ClassInput="ProductInputTest",
                            HasInput = true,
                            Index = 14,
                            KindProperty = KindProperty.STR,
                            IndexFather = 12,
                            Info = new EntitySearchDisplayInfo{
                                Column = "Sag N°",
                                Description = "Código SAG",
                                PlaceHolder = "Indique el nombre dél código SAG",
                                ShortName = "SAG N",
                                Title ="Código SAG"

                            },
                            isArray = false,
                            NameProp="SagCode",
                            Required = true,
                            Unique = true,
                            TypeString = "string",
                            Visible = true,
                            ModelPropName ="SagCode"

                        }},
                      { "MeasureType", new InputPropDetails{
                           AutoNumeric = false,
                            ClassName = "ProductTest",
                            ClassInput="ProductInputTest",    
                            KindProperty = KindProperty.ENUM,
                            IndexFather = 12,
                            Index = 0,
                            HasInput = true,
                            Info =  new EntitySearchDisplayInfo{
                                Column = "Tipo M",
                                Description = "Tipo de medida para producto",
                                PlaceHolder = "Indique el tipo de medida",
                                ShortName = "Tipo",
                                Title ="Tipo de medida de producto"

                            },
                            isArray = false,
                            NameProp="MeasureType",
                            Required = true,
                            TypeString = "MeasureType",
                            Unique = false,
                            Visible = true,
                            ModelPropName="MeasureType"
                        }}
                },
                InputRelatedDetails=new Dictionary<string, InputPropRelatedDetails> {
                    { "IdActiveIngredient", new InputPropRelatedDetails{
                        AutoNumeric = false,
                        Info = new EntitySearchDisplayInfo{
                            Column = "Ingrediente",
                            Description="Ingrediente de productos",
                            PlaceHolder = "Ingrese ingrediente",
                            ShortName = "ING",
                            Title = "Ingrediente Activo"
                        },
                        ClassName = "ProductTest",
                        ClassNameTarget="IngredientTest",
                        ClassInput="ProductTestInput",
                        ClassInputTarget="IngredientTestInput",
                        HasInput = true,
                        Index = 7,
                        IndexFather = 12,
                        isArray = false,
                        NameProp="IdActiveIngredient",
                        RealIndex = 12,
                        Required=false,
                        TypeString = "string",
                        Unique = false,
                        Visible = true,
                        IsReference = true,
                        ModelPropName="IdActiveIngredient"
                        

                    } },
                    { "IdBrand", new InputPropRelatedDetails{
                            AutoNumeric = false,
                            Info = new EntitySearchDisplayInfo{
                                Column = "Marca",
                                Description="Marca de producto",
                                PlaceHolder = "Ingrese marca",
                                ShortName = "marca",
                                Title = "Marca"
                            },
                            ClassName = "ProductTest",
                            ClassNameTarget="BrandTest",
                            ClassInput="ProductTestInput",
                            ClassInputTarget="BrandInputTest",
                            IsReference = true,
                            ModelPropName="IdBrand",
                            HasInput = true,
                            Index = 32,
                            IndexFather = 12,
                            isArray = false,
                            NameProp="IdBrand",
                            RealIndex = 12,
                            Required=false,
                            TypeString = "string",
                            Unique = false,
                            Visible = true

                    }},
                     { "Doses", new InputPropRelatedDetails{
                            AutoNumeric = false,
                            Info = new EntitySearchDisplayInfo{
                                Column = "Doses",
                                Description="Doses",
                                PlaceHolder = "Doses",
                                ShortName = "Doses",
                                Title = "Doses"
                            },
                            ClassName = "ProductTest",
                            ClassNameTarget="DosesTest",
                            HasInput = true,
                            Index = 6,
                            IndexFather = 12,
                            isArray = true,
                            NameProp="Doses",
                            RealIndex = 6,
                            Required=false,
                            TypeString = "Doses",
                            Unique = false,
                            Visible = true,
                            IsReference = false,
                            ClassInputTarget="DosesInputTest",
                            ClassInput="ProductInputTest",                            
                            ModelPropName=string.Empty
                    }},

                }, 
                RelatedInputs = new Dictionary<string, InputDetails> {
                    { "Doses", new InputDetails{ 
                        InputRelatedDetails = new Dictionary<string, InputPropRelatedDetails>{
                            {"IdVarieties", new InputPropRelatedDetails {
                                AutoNumeric = false,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Variedad",
                                    Description="Variedad",
                                    PlaceHolder = "Variedad",
                                    ShortName = "Variedad",
                                    Title = "Variedad"
                                },
                                ClassName = "DosesTest",
                                ClassNameTarget= "VarietyTest",
                                ClassInput="DosesInputTest",                                
                                HasInput = true,
                                Index = 21,
                                IndexFather = 6,
                                isArray = true,
                                NameProp="IdVarieties",
                                RealIndex = 21,
                                Required=false,
                                TypeString = "string",
                                Unique = false,
                                ClassInputTarget="VarietyTestInput",
                                Visible = true,
                                
                                IsReference = true,
                                ModelPropName="idVarieties"
                            } },
                            {"IdSpecies", new InputPropRelatedDetails {
                                AutoNumeric = false,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Specie",
                                    Description="Specie",
                                    PlaceHolder = "Specie",
                                    ShortName = "Specie",
                                    Title = "Specie"
                                },
                                ClassName = "DosesTest",
                                ClassInput = "DosesInputTest",
                                ClassInputTarget="SpeciesInputTest",
                                ClassNameTarget="SpeciesTest",
                                HasInput = true,
                                Index = 30,
                                IndexFather = 6,
                                isArray = true,
                                NameProp="IdSpecies",
                                RealIndex = 30,
                                Required=false,
                                TypeString = "string",
                                Unique = false,
                                Visible = true,
                                IsReference = true,
                                ModelPropName="IdSpecies"
                            } },
                             {"IdsApplicationTarget", new InputPropRelatedDetails {
                                AutoNumeric = false,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Objetivo",
                                    Description="Objetivo aplicación",
                                    PlaceHolder = "Objetivo",
                                    ShortName = "Objetivo",
                                    Title = "Objetivo"
                                },
                                ClassName = "DosesTest",
                                ClassInput= "DosesInputTest",
                                ClassInputTarget="TargetInpuTest",
                                ClassNameTarget="TargetTest",
                                HasInput = true,
                                Index = 18,
                                IndexFather = 6,
                                isArray = true,
                                NameProp="IdsApplicationTarget",
                                RealIndex = 18,
                                Required=false,
                                TypeString = "string",
                                Unique = false,
                                Visible = true,
                                IsReference = true,
                                ModelPropName="IdsApplicationTarget"
                            } },
                              {"WaitingToHarvest", new InputPropRelatedDetails {
                                AutoNumeric = false,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "WaitingToHarvest",
                                    Description="WaitingToHarvest",
                                    PlaceHolder = "WaitingToHarvest",
                                    ShortName = "WaitingToHarvest",
                                    Title = "WaitingToHarvest"
                                },
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                ClassNameTarget="WaitingHarvestTest",
                                ClassInputTarget="WaitingHarvestInputTest",
                                HasInput = true,
                                Index = 0,
                                IndexFather = 6,
                                isArray = true,
                                NameProp="WaitingToHarvest",
                                RealIndex = 0,
                                Required=false,
                                TypeString = "string",
                                Unique = false,
                                Visible = true,
                                IsReference = true,
                                ModelPropName="WaitingToHarvest"
                            } }

                        },
                        InputPropsDetails = new Dictionary<string, InputPropDetails>{
                            {"HoursToReEntryToBarrack", new InputPropDetails{ 
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                KindProperty = KindProperty.NUM32,
                                HasInput= true,
                                Index= 2,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{ 
                                    Column = "Horas",
                                    Description = "Horas de reingreso a cuartel",
                                    PlaceHolder = "Ingrese horas de reingreso",
                                    ShortName = "Horas",
                                    Title= "Horas de reingreso a cuartel"
                                },
                                isArray = false,
                                ModelPropName = "HoursToReEntryToBarrack",
                                NameProp="HoursToReEntryToBarrack",
                                Required = false,
                                TypeString = "int",
                                Unique=false,
                                Visible=true
                            } },
                             {"ApplicationDaysInterval", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                HasInput= true,
                                KindProperty = KindProperty.NUM32,
                                Index= 3,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Intevalo",
                                    Description = "Intevalo de horas entre aplicación en un cuartel",
                                    PlaceHolder = "Ingrese Intevalo de horas",
                                    ShortName = "Intevalo",
                                    Title= "Intevalo de horas entre aplicación"
                                },
                                isArray = false,
                                ModelPropName = "ApplicationDaysInterval",
                                NameProp="ApplicationDaysInterval",
                                Required = false,
                                TypeString = "int",
                                Unique=false,
                                Visible=true
                            } },
                              {"NumberOfSequentialApplication", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                HasInput= true,
                                KindProperty = KindProperty.NUM32,
                                Index= 4,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "N° secuencias",
                                    Description = "N° secuencias",
                                    PlaceHolder = "Ingrese Intevalo de horas",
                                    ShortName = "N° secuencias",
                                    Title= "N° secuencias"
                                },
                                isArray = false,
                                ModelPropName = "NumberOfSequentialApplication",
                                NameProp="NumberOfSequentialApplication",
                                Required = false,
                                TypeString = "int",
                                Unique=false,
                                Visible=true
                            } },
                              {"WettingRecommendedByHectares", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                HasInput= true,
                                KindProperty = KindProperty.NUM32,
                                Index= 5,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Mojamiento recomendado",
                                    Description = "Mojamiento recomendado de la etiqueta",
                                    PlaceHolder = "Ingrese Mojamiento",
                                    ShortName = "Mojamiento",
                                    Title= "Mojamiento"
                                },
                                isArray = false,
                                ModelPropName = "WettingRecommendedByHectares",
                                NameProp="WettingRecommendedByHectares",
                                Required = false,
                                TypeString = "int",
                                Unique=false,
                                Visible=true
                            } },
                            {"DosesQuantityMin", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                HasInput= true,
                                KindProperty = KindProperty.DBL,
                                Index= 4,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Cantidad Mínima",
                                    Description = "Cantidad Mínima",
                                    PlaceHolder = "Ingrese Cantidad Mínima",
                                    ShortName = "Cantidad Mínima",
                                    Title= "Cantidad Mínima"
                                },
                                isArray = false,
                                ModelPropName = "DosesQuantityMin",
                                NameProp="DosesQuantityMin",
                                Required = false,
                                TypeString = "double",
                                Unique=false,
                                Visible=true
                            } },
                             {"DosesQuantityMax", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",                                
                                ClassInput="DosesInputTest",                                
                                HasInput= true,
                                KindProperty = KindProperty.DBL,
                                Index= 4,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Cantidad Máxima",
                                    Description = "Cantidad Máxima",
                                    PlaceHolder = "Ingrese Cantidad Máxima",
                                    ShortName = "Cantidad Máxima",
                                    Title= "Cantidad Máxima"
                                },
                                isArray = false,
                                ModelPropName = "DosesQuantityMax",
                                NameProp="DosesQuantityMax",
                                Required = false,
                                TypeString = "double",
                                Unique=false,
                                Visible=true
                            } },
                             {"WaitingDaysLabel", new InputPropDetails{
                                AutoNumeric = false,
                                ClassName = "DosesTest",
                                ClassInput="DosesInputTest",
                                HasInput= true,
                                Index= 0,
                                KindProperty = KindProperty.NUM32,
                                IndexFather = 6,
                                Info = new EntitySearchDisplayInfo{
                                    Column = "Días de cosecha",
                                    Description = "Días de cosecha",
                                    PlaceHolder = "Ingrese Días de cosecha",
                                    ShortName = "Días de cosecha",
                                    Title= "Días de cosecha"
                                },
                                isArray = false,
                                ModelPropName = "WaitingDaysLabel",
                                NameProp="WaitingDaysLabel",
                                Required = false,
                                TypeString = "int",
                                Unique=false,
                                Visible=true
                            } }
                        },
                        RelatedInputs = new Dictionary<string, InputDetails> { 
                            { "WaitingToHarvest",  new InputDetails {
                                InputPropsDetails = new Dictionary<string, InputPropDetails>{
                                    { "Ppm", new InputPropDetails{ 
                                        AutoNumeric = false,
                                        ClassInput = "WaitingHarvestInputTest",
                                        ClassName = "WaitingHarvestTest",
                                        HasInput = true,
                                        Index = 1,
                                        IndexFather = 0,
                                        Info = new EntitySearchDisplayInfo
                                        {
                                            Column = "PPM",
                                            Description = "Partes Por millón",
                                            PlaceHolder = "Ingrese PPM",
                                            ShortName = "PPM",
                                            Title = "PPM"
                                        },
                                        isArray = false,
                                        KindProperty= KindProperty.DBL,
                                        ModelPropName= "Ppm",
                                        NameProp = "Ppm",
                                        Required = true,
                                        TypeString = "double",
                                        Unique = false,
                                        Visible = true
                                    } },
                                    { "WaitingDays", new InputPropDetails{
                                        AutoNumeric = false,
                                        ClassInput = "WaitingHarvestInputTest",
                                        ClassName = "WaitingHarvestTest",
                                        HasInput = true,
                                        Index = 1,
                                        IndexFather = 0,
                                        Info = new EntitySearchDisplayInfo
                                        {
                                            Column = "Días de Cosecha",
                                            Description = "Días de Cosecha",
                                            PlaceHolder = "Ingrese Días de Cosecha",
                                            ShortName = "Días de Cosecha",
                                            Title = "Días de Cosecha"
                                        },
                                        isArray = false,
                                        KindProperty= KindProperty.DBL,
                                        ModelPropName= "WaitingDays",
                                        NameProp = "WaitingDays",
                                        Required = true,
                                        TypeString = "double",
                                        Unique = false,
                                        Visible = true
                                    } },

                                },
                                InputRelatedDetails  = new Dictionary<string, InputPropRelatedDetails>{
                                    { "IdCertifiedEntity", new InputPropRelatedDetails{ 
                                        AutoNumeric = false,
                                        Visible = true,
                                        Unique = false,
                                        ClassInput = "WaitingHarvestInputTest",
                                        ClassInputTarget = "CertifiedEntityInputTest",
                                        ClassName = "WaitingHarvestTest",
                                        ClassNameTarget = "CertifiedEntityTest",
                                        HasInput = true,
                                        Index = 4,
                                        IndexFather = 0,
                                        Info = new EntitySearchDisplayInfo { 
                                            Column = "Mercado",
                                            Description = "Mercado (Entidad Certificadora)",
                                            PlaceHolder = "Ingrese Mercado",
                                            ShortName = "Mercado",
                                            Title = "Mercado"
                                        },
                                        isArray = false,
                                        IsReference = true,
                                        ModelPropName = "IdCertifiedEntity",
                                        NameProp = "IdCertifiedEntity",
                                        RealIndex = 4,
                                        TypeString = "string",
                                        Required = true
                                    } }
                                },
                                ValidationsGroup = new Dictionary<int, string[][]>{
                                    {(int)Validation.REQUIRED, new string[][]{ 
                                        new string[]{
                                            "Ppm",
                                            "WaitingDays",
                                            "IdCertifiedEntity"
                                        }
                                    } }
                                }
                            } } 
                        
                        },
                        ValidationsGroup = new Dictionary<int, string[][]> {
                            {(int)Validation.REQUIRED, new string[][]{
                                                new string[]{
                                                    "HoursToReEntryToBarrack",
                                                    "ApplicationDaysInterval",
                                                    "NumberOfSequentialApplication",
                                                    "WettingRecommendedByHectares",
                                                    "DosesQuantityMin",
                                                }
                                            } },
                            
                        }
                    } }
                },
                ValidationsGroup = new Dictionary<int, string[][]> {
                    {(int)Validation.REQUIRED, new string[][]{
                                        new string[]{
                                            "Name",
                                            "MeasureType",
                                            "SagCode"
                                        }
                                    } },
                    {(int)Validation.UNIQUE, new string[][]{
                                        new string[]{
                                            "Name",
                                            "SagCode"
                                        }
                                    } }
                }

            }

        };
    }
}
