using System.ComponentModel;

namespace trifenix.connect.test.enums
{
    public enum EntitySection
    {
        [Description("Órdenes de aplicación")]
        ORDENES = 0,
        [Description("Usuarios")]
        USUARIOS = 1,
        [Description("Productos")]
        PRODUCTOS = 2,
        [Description("Terreno")]
        TERRENOS = 4,
    }
}
