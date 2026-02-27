using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.Models.Mappings
{
    public static class ValidadeDiasMapper
    {
        public static int ParaDias(ValidadeDias validade)
        {
            return validade switch
            {
                ValidadeDias.Dia1 => 1,
                ValidadeDias.Dias2 => 2,
                ValidadeDias.Dias3 => 3,
                ValidadeDias.Dias5 => 5,
                ValidadeDias.Dias7 => 7,
                ValidadeDias.Dias10 => 10,
                ValidadeDias.Dias15 => 15,
                ValidadeDias.Dias20 => 20,
                ValidadeDias.Dias30 => 30,
                ValidadeDias.Dias90 => 90,
                _ => throw new ArgumentOutOfRangeException(nameof(validade))
            };
        }
    }
}
