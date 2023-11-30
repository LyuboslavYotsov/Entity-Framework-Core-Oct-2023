
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Artillery.Extensions;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shellsWithGuns = context.Shells
                .Where(s => s.ShellWeight > shellWeight)
                .Select(s => new ExportShellDto()
                {
                    ShellWeight = s.ShellWeight,
                    Caliber = s.Caliber,
                    Guns = s.Guns
                            .Where(g => (int)g.GunType == 3)
                            .Select(g => new ExportGunDto()
                            {
                                GunType = g.GunType.ToString(),
                                GunWeight = g.GunWeight,
                                BarrelLength = g.BarrelLength,
                                Range = g.Range > 3000 ? "Long-range" : "Regular range"
                            })
                            .OrderByDescending(g => g.GunWeight)
                            .ToArray()
                })
                .OrderBy(s => s.ShellWeight)
                .ToArray();

            string result = JsonConvert.SerializeObject(shellsWithGuns, Formatting.Indented);

            return result;
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var gunsWithCountries = context.Guns
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .Select(g => new ExportGunXmlDto()
                {
                    Manufacturer = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns
                                .Where(cg => cg.Country.ArmySize > 4500000)
                                .Select(cg => new ExportCountryXmlDto()
                                {
                                    Country = cg.Country.CountryName,
                                    ArmySize = cg.Country.ArmySize
                                })
                                .OrderBy(ec => ec.ArmySize)
                                .ToArray()
                })
                .OrderBy(g => g.BarrelLength)
                .ToArray();

            string result = gunsWithCountries.SerializeXml("Guns");

            return result;
        }
    }
}
