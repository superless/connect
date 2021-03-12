using System.ComponentModel;

namespace trifenix.connect.test.enums
{
    public enum NotificationType
    {
        [Description("otro")]
        Default = 1,

        [Description("Evento fenológico")]
        Phenological = 0,
    }

}
