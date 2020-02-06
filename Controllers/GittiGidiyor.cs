using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.GittiGidiyor.Models;
using Nop.Plugin.Misc.GittiGidiyor.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.GittiGidiyor.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class GittiGidiyorController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly GittiGidiyorSettings _gittigidiyorSettings;
        private readonly GittiGidiyorManager _gittigidiyormanager;

        public GittiGidiyorController(ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            GittiGidiyorSettings gittigidiyorSettings,
            GittiGidiyorManager gittigidiyormanager)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _gittigidiyorSettings = gittigidiyorSettings;
            _gittigidiyormanager = gittigidiyormanager;
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel()
            {
                ApiKey = _gittigidiyorSettings.ApiKey,
                SecretKey = _gittigidiyorSettings.SecretKey,
                RoleName = _gittigidiyorSettings.RoleName,
                RolePass = _gittigidiyorSettings.RolePass,
                ForceToSpecEntry = _gittigidiyorSettings.ForceToSpecEntry.ToString(),
                NextDateOption = _gittigidiyorSettings.NextDateOption.ToString(),
                Lang = _gittigidiyorSettings.Lang,
                CargoCompany = _gittigidiyorSettings.CargoCompany,
                CityPrice = _gittigidiyorSettings.CityPrice,
                CountryPrice = _gittigidiyorSettings.CountryPrice,
                ListingDays = _gittigidiyorSettings.ListingDays,
                ShippingTimeDays = _gittigidiyorSettings.ShippingTimeDays,
                DefaultCategoryId = _gittigidiyorSettings.DefaultCategoryId,
                CategoryIdMapping = _gittigidiyorSettings.CategoryIdMapping,
                DefaultBrandName = _gittigidiyorSettings.DefaultBrandName,
                BrandIdMapping = _gittigidiyorSettings.BrandIdMapping
            };

            return View(@"~/Plugins/Misc.GittiGidiyor/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _gittigidiyorSettings.ApiKey = model.ApiKey;
            _gittigidiyorSettings.SecretKey = model.SecretKey;
            _gittigidiyorSettings.RoleName = model.RoleName;
            _gittigidiyorSettings.RolePass = model.RolePass;
            _gittigidiyorSettings.ForceToSpecEntry = bool.Parse(model.ForceToSpecEntry);
            _gittigidiyorSettings.NextDateOption = bool.Parse(model.NextDateOption);
            _gittigidiyorSettings.Lang = model.Lang;
            _gittigidiyorSettings.CargoCompany = model.CargoCompany;
            _gittigidiyorSettings.CityPrice = model.CityPrice;
            _gittigidiyorSettings.CountryPrice = model.CountryPrice;
            _gittigidiyorSettings.ListingDays = model.ListingDays;
            _gittigidiyorSettings.ShippingTimeDays = model.ShippingTimeDays;
            _gittigidiyorSettings.DefaultCategoryId = model.DefaultCategoryId;
            _gittigidiyorSettings.CategoryIdMapping = model.CategoryIdMapping;
            _gittigidiyorSettings.DefaultBrandName = model.DefaultBrandName;
            _gittigidiyorSettings.BrandIdMapping = model.BrandIdMapping;
            _settingService.SaveSetting(_gittigidiyorSettings);

            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View(@"~/Plugins/Misc.GittiGidiyor/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [AuthorizeAdmin]
        [FormValueRequired("exportexcel-categories-all")]
        public virtual IActionResult ExportExcelAllCategories()
        {
            try
            {
                var bytes = _gittigidiyormanager.ExportGittiGidiyorCategoriesToXlsx();
                return File(bytes, MimeTypes.TextXlsx, "gittigidiyorcategories.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return Configure();
            }
        }
    }
}