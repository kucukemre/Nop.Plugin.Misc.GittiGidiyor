using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.GittiGidiyor.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.RoleName")]
        public string RoleName { get; set; }
        public bool RoleName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.RolePass")]
        public string RolePass { get; set; }
        public bool RolePass_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.ForceToSpecEntry")]
        public string ForceToSpecEntry { get; set; }
        public bool ForceToSpecEntry_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.NextDateOption")]
        public string NextDateOption { get; set; }
        public bool NextDateOption_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.Lang")]
        public string Lang { get; set; }
        public bool Lang_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.CargoCompany")]
        public string CargoCompany { get; set; }
        public bool CargoCompany_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.CityPrice")]
        public string CityPrice { get; set; }
        public bool CityPrice_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.CountryPrice")]
        public string CountryPrice { get; set; }
        public bool CountryPrice_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.ListingDays")]
        public string ListingDays { get; set; }
        public bool ListingDays_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.ShippingTimeDays")]
        public string ShippingTimeDays { get; set; }
        public bool ShippingTimeDays_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultCategoryId")]
        public string DefaultCategoryId { get; set; }
        public bool DefaultCategoryId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.CategoryIdMapping")]
        public string CategoryIdMapping { get; set; }
        public bool CategoryIdMapping_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultBrandName")]
        public string DefaultBrandName { get; set; }
        public bool DefaultBrandName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.GittiGidiyor.Admin.Fields.BrandIdMapping")]
        public string BrandIdMapping { get; set; }
        public bool BrandIdMapping_OverrideForStore { get; set; }
    }
}