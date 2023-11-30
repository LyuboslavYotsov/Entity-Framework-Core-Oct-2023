namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Artillery.Extensions;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportCountryDto[]? countriesDtos = xmlString.DeserializeXml<ImportCountryDto[]>("Countries");

            ICollection<Country> validCountries = new List<Country>();

            foreach (var countryDto in countriesDtos)
            {
                if (!IsValid(countryDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Country newCountry = new Country()
                {
                    CountryName = countryDto.CountryName,
                    ArmySize = countryDto.ArmySize
                };

                validCountries.Add(newCountry);
                result.AppendLine(string.Format(SuccessfulImportCountry, newCountry.CountryName, newCountry.ArmySize));
            }

            context.Countries.AddRange(validCountries);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportManufacturerDto[]? manufacturersDtos = xmlString.DeserializeXml<ImportManufacturerDto[]>("Manufacturers");

            ICollection<Manufacturer> validManufacturers = new List<Manufacturer>();

            foreach (var manufacturerDto in manufacturersDtos)
            {
                if (!IsValid(manufacturerDto) || validManufacturers.Any(vm => vm.ManufacturerName == manufacturerDto.ManufacturerName))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer newManufacturer = new Manufacturer()
                {
                    ManufacturerName = manufacturerDto.ManufacturerName,
                    Founded = manufacturerDto.Founded
                };

                string[] foundedInfo = newManufacturer.Founded.Split(", ");
                string townName = foundedInfo[foundedInfo.Length - 2];
                string countryName = foundedInfo[foundedInfo.Length - 1];

                validManufacturers.Add(newManufacturer);
                result.AppendLine(string.Format(SuccessfulImportManufacturer, newManufacturer.ManufacturerName, $"{townName}, {countryName}"));
            }

            context.Manufacturers.AddRange(validManufacturers);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportShellDto[]? shellsDtos = xmlString.DeserializeXml<ImportShellDto[]>("Shells");

            ICollection<Shell> validShells = new List<Shell>();

            foreach (var shellDto in shellsDtos)
            {
                if (!IsValid(shellDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Shell newShell = new Shell()
                {
                    ShellWeight = shellDto.ShellWeight,
                    Caliber = shellDto.Caliber
                };

                validShells.Add(newShell);
                result.AppendLine(string.Format(SuccessfulImportShell, newShell.Caliber, newShell.ShellWeight));
            }

            context.Shells.AddRange(validShells);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportGunDto[]? gunsDtos = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

            ICollection<Gun> validGuns = new List<Gun>();

            foreach (var gunDto in gunsDtos)
            {
                if (!IsValid(gunDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                try
                {
                    Gun newGun = new Gun()
                    {
                        ManufacturerId = gunDto.ManufacturerId,
                        GunWeight = gunDto.GunWeight,
                        BarrelLength = gunDto.BarrelLength,
                        NumberBuild = gunDto.NumberBuild,
                        Range = gunDto.Range,
                        GunType = (GunType)Enum.Parse(typeof(GunType), gunDto.GunType),
                        ShellId = gunDto.ShellId
                    };


                    foreach (var countryId in gunDto.Countries.Select(c => c.Id).ToArray())
                    {
                        newGun.CountriesGuns.Add(new CountryGun()
                        {
                            CountryId = countryId
                        });
                    }
                    validGuns.Add(newGun);
                    result.AppendLine(string.Format(SuccessfulImportGun, newGun.GunType.ToString(), newGun.GunWeight, newGun.BarrelLength));
                }
                catch (Exception ex)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }
            }

            context.Guns.AddRange(validGuns);

            context.SaveChanges();

            return result.ToString();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}