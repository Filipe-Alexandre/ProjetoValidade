using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.Models.Mappings
{
    public static class ValidadeHorasMapper
    {
        public static int ParaHoras(ValidadeHoras validade)
        {
            return validade switch
            {
                ValidadeHoras.Horas2 => 2,
                ValidadeHoras.Horas3 => 3,
                ValidadeHoras.Horas12 => 12,
                ValidadeHoras.Horas24 => 24,
                _ => throw new ArgumentOutOfRangeException(nameof(validade))
            };
        }
    }
}
