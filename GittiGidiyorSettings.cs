using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.GittiGidiyor
{
    public class GittiGidiyorSettings : ISettings
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string RoleName { get; set; }
        public string RolePass { get; set; }
        public bool ForceToSpecEntry { get; set; }
        public bool NextDateOption { get; set; }
        public string Lang { get; set; }
        public string CargoCompany { get; set; }
        public string CityPrice { get; set; }
        public string CountryPrice { get; set; }
        public string ListingDays { get; set; }
        public string ShippingTimeDays { get; set; }
        public string DefaultCategoryId { get; set; }
        public string CategoryIdMapping { get; set; }
        public string DefaultBrandName { get; set; }
        public string BrandIdMapping { get; set; }
        public int LastAddedProductId { get; set; }
        public string FailedProductIds { get; set; }
    }
}
