using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.GittiGidiyor
{
    public class GittiGidiyorPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public GittiGidiyorPlugin(ISettingService settingService, IWebHelper webHelper, ILocalizationService localizationService, IScheduleTaskService scheduleTaskService)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        public override void Install()
        {
            //settings
            var settings = new GittiGidiyorSettings
            {
                ApiKey = "",
                SecretKey = "",
                RoleName = "",
                RolePass = "",
                ForceToSpecEntry = false,
                NextDateOption = false,
                Lang = "",
                ListingDays = "",
                CargoCompany = "",
                CityPrice = "",
                CountryPrice = "",
                ShippingTimeDays = "",
                CategoryIdMapping = "",
                DefaultCategoryId = "",
                BrandIdMapping = "",
                DefaultBrandName = "",
                LastAddedProductId = 0,
                FailedProductIds = ""

            };
            _settingService.SaveSetting(settings);

            //install synchronization task
            if (_scheduleTaskService.GetTaskByType("Nop.Plugin.Misc.GittiGidiyor.Services.SynchronizationTask") == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = 12 * 60 * 60,
                    Name = "Synchronization (GittiGidiyor plugin)",
                    Type = "Nop.Plugin.Misc.GittiGidiyor.Services.SynchronizationTask",
                });
            }

            //locales
            #region locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Title", "GittiGidiyor Plugin");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ApiKey", "GittiGidiyor API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ApiKey.Hint", "Enter GittiGidiyor API Key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.SecretKey", "GittiGidiyor Secret Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.SecretKey.Hint", "Enter GittiGidiyor Secret Key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RoleName", "GittiGidiyor Role Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RoleName.Hint", "Enter GittiGidiyor Role Name.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RolePass", "GittiGidiyor Role Pass");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RolePass.Hint", "Enter GittiGidiyor Role Pass.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ForceToSpecEntry", "GittiGidiyor Force To Spec Entry");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ForceToSpecEntry.Hint", "Enter GittiGidiyor Force To Spec Entry.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.NextDateOption", "GittiGidiyor Next Date Option");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.NextDateOption.Hint", "Enter GittiGidiyor Next Date Option.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Lang", "GittiGidiyor Lang");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Lang.Hint", "Enter GittiGidiyor Lang.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CargoCompany", "GittiGidiyor Cargo Company");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CargoCompany.Hint", "Enter GittiGidiyor Cargo Company(aras: Aras Kargo, yurtici: Yurtiçi Kargo, ups: UPS Kargo, mng: MNG Kargo, ptt: PTT Kargo, other: Diğer kargo firması).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CityPrice", "GittiGidiyor City Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CityPrice.Hint", "Enter GittiGidiyor City Price(10.00).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CountryPrice", "GittiGidiyor Country Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CountryPrice.Hint", "Enter GittiGidiyor Country Price(10.00).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ListingDays", "GittiGidiyor Listing Days");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ListingDays.Hint", "Enter GittiGidiyor Listing Days(30,60,180,360).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ShippingTimeDays", "GittiGidiyor Shipping Time Days");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ShippingTimeDays.Hint", "Enter GittiGidiyor Shipping Time Days(today,tomorrow,2-3days,3-7days(Yalnızca mobilya kategorisi için)).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultCategoryId", "GittiGidiyor Default Category Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultCategoryId.Hint", "Enter GittiGidiyor Default Category Id.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CategoryIdMapping", "GittiGidiyor Category Id Mapping");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CategoryIdMapping.Hint", "Enter GittiGidiyor Category Id Mapping. {nopcommerce-category-id}=>{gittigidiyor-category-id};{nopcommerce-category-id}=>{gittigidiyor-category-id} Example: 1=>cnch4;2=>cnch6");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultBrandName", "GittiGidiyor Default Brand Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultBrandName.Hint", "Enter GittiGidiyor Default Brand Name.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.BrandIdMapping", "GittiGidiyor Brand Id Mapping");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.BrandIdMapping.Hint", "Enter GittiGidiyor Brand Id Mapping. {nopcommerce-brand-id}=>{gittigidiyor-brand-name};{nopcommerce-brand-id}=>{gittigidiyor-brand-name} Example: 1=>Diğer;2=>Acar");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Categories.ExportToExcel.All", "Export All GittiGidiyor Categories To Xlsx");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.CategoriesAttributes.ExportToExcel.All", "Export All GittiGidiyor Categories Attributes To Xlsx");
            #endregion

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GittiGidiyorSettings>();

            //schedule task
            var task = _scheduleTaskService.GetTaskByType("Nop.Plugin.Misc.GittiGidiyor.Services.SynchronizationTask");
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //locales
            #region locales
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ApiKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.SecretKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.SecretKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RoleName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RoleName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RolePass");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.RolePass.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ForceToSpecEntry");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ForceToSpecEntry.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.NextDateOption");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.NextDateOption.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Lang");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.Lang.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CargoCompany");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CargoCompany.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CityPrice");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CityPrice.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CountryPrice");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CountryPrice.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ListingDays");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ListingDays.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ShippingTimeDays");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.ShippingTimeDays.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultCategoryId");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultCategoryId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CategoryIdMapping");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.CategoryIdMapping.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultBrandName");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.DefaultBrandName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.BrandIdMapping");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Fields.BrandIdMapping.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.Categories.ExportToExcel.All");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.GittiGidiyor.Admin.CategoriesAttributes.ExportToExcel.All");
            #endregion

            base.Uninstall();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/GittiGidiyor/Configure";
        }

        #endregion
    }
}