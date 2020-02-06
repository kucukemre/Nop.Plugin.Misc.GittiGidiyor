using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GittiGidiyor.Domain;
using Nop.Plugin.Misc.GittiGidiyor.Models;
using Nop.Plugin.Misc.GittiGidiyor.Rest.Net;
using Nop.Plugin.Misc.GittiGidiyor.Rest.Net.Authenticators;
using Nop.Plugin.Misc.GittiGidiyor.Rest.Net.Interfaces;
using Nop.Services.Configuration;
using Nop.Services.ExportImport.Help;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.GittiGidiyor.Services
{
    public class GittiGidiyorManager
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly GittiGidiyorSettings _gittigidiyorSettings;
        private readonly Nop.Services.Catalog.IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private BlockingCollection<List<ProductTypeModel>> BlockingCollection { get; set; }
        private List<(NotifyType, string)> Messages { get; set; }
        private int MaxDegreeOfParallelGittiGidiyorImport
        {
            get
            {
                int count = 3;
                return count;
            }
        }

        #endregion

        #region Ctor

        public GittiGidiyorManager(IStoreContext storeContext, ISettingService settingService, GittiGidiyorSettings gittigidiyorSettings, Nop.Services.Catalog.IProductService productService, CatalogSettings catalogSettings)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._gittigidiyorSettings = gittigidiyorSettings;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
            Messages = new List<(NotifyType, string)>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Synchronize contacts 
        /// </summary>
        /// <returns>List of messages</returns>
        public IList<(NotifyType Type, string Message)> Synchronize()
        {
            try
            {
                //gittigidiyor plugin is configured
                var config = _gittigidiyorSettings;
                if (string.IsNullOrEmpty(config.ApiKey) || string.IsNullOrEmpty(config.SecretKey) || string.IsNullOrEmpty(config.RoleName) || string.IsNullOrEmpty(config.RolePass))
                    throw new NopException($"Plugin not configured");

                var client = new RestClient("https://dev.gittigidiyor.com:8443/");
                client.Authentication = new BasicAuthenticator(config.RoleName, config.RolePass);

                BlockingCollection = new BlockingCollection<List<ProductTypeModel>>();
                var taskArray = new Task[]
                {
                    Task.Factory.StartNew(() => Producer()),
                    Task.Factory.StartNew(() => Consumer(client, config))
                };
                Task.WaitAll(taskArray);
            }
            catch (Exception exception)
            {
                Messages.Add((NotifyType.Error, $"GittiGidiyor ProductImport error: {exception.Message}"));
            }

            return Messages;
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <returns>Result in XLSX format</returns>
        public virtual byte[] ExportGittiGidiyorCategoriesToXlsx()
        {
            var config = _gittigidiyorSettings;
            var categories = GetCategories(config);
            categories = categories.Where(x => x.CategoryCode.StartsWith("cnm")).ToList();
            var manager = new PropertyManager<Domain.Category>(new[]
            {
                new PropertyByName<Domain.Category>("Category Code", p => p.CategoryCode),
                new PropertyByName<Domain.Category>("Category Name", p => p.CategoryName),
                new PropertyByName<Domain.Category>("Deepest", p => p.Deepest),
                new PropertyByName<Domain.Category>("Has Catalog", p => p.HasCatalog)
            }, _catalogSettings);

            return manager.ExportToXlsx(categories);
        }

        #endregion

        #region Utilities

        private void Producer()
        {
            var pageSize = 10000;
            var totalProduct = _productService.GetNumberOfProductsInCategory();
            var pageCount = (totalProduct / pageSize) + 1;
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var sw = Stopwatch.StartNew();

            for (int index = 0; index < pageCount; index++)
            {
                try
                {
                    var gittigidiyorSettings = _settingService.LoadSetting<GittiGidiyorSettings>(storeId);

                    GetProductTypeModelAndBlockingCollection(index, pageSize, gittigidiyorSettings);

                    if (!String.IsNullOrEmpty(gittigidiyorSettings.FailedProductIds))
                    {
                        try
                        {
                            var productIds = gittigidiyorSettings.FailedProductIds.Split(";").Where(x => !String.IsNullOrEmpty(x)).ToArray();

                            GetProductTypeModelAndBlockingCollection(index, pageSize, gittigidiyorSettings, productIds);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Messages.Add((NotifyType.Success, $"Producer index:{index} - Time:{sw.Elapsed.TotalSeconds} s"));
                }
                catch (Exception ex)
                {
                    Messages.Add((NotifyType.Error, String.Format($"Producer index:{index} - Error: {0}", ex.Message)));
                }
            }

            BlockingCollection.CompleteAdding();
        }

        private void Consumer(RestClient client, GittiGidiyorSettings config)
        {
            Parallel.ForEach(BlockingCollection.GetConsumingEnumerable(), new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelGittiGidiyorImport }, (item, state, index) =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    foreach (var productTypeModel in item)
                    {
                        var lastAddedProductId = InsertProduct(client, config, productTypeModel);

                        SetLastAddedProductIdSetting(lastAddedProductId);
                    }

                    sw.Stop();
                }
                catch (Exception exception)
                {
                    Messages.Add((NotifyType.Error, $"GittiGidiyor ProductImport error: {exception.Message}"));
                }
            });
        }

        private void GetProductTypeModelAndBlockingCollection(int index, int size, GittiGidiyorSettings gittigidiyorSettings, string[] productIds = null)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                IEnumerable<Nop.Core.Domain.Catalog.Product> products = null;

                if (productIds == null)
                {
                    products = _productService.SearchProducts(
                    pageIndex: index,
                    pageSize: size
                    ).Where(x => x.Id > gittigidiyorSettings.LastAddedProductId);
                }
                else
                {
                    products = _productService.SearchProducts(
                    pageIndex: index,
                    pageSize: size
                    ).Where(x => productIds.Contains(x.Id.ToString()));
                }

                if (products.Any())
                {
                    var productTypeModelList = new List<ProductTypeModel>();

                    foreach (var product in products)
                    {
                        var productTypeModel = PrepareProductTypeModel(product, gittigidiyorSettings);

                        productTypeModelList.Add(productTypeModel);
                    }

                    BlockingCollection.Add(productTypeModelList);
                }

                Messages.Add((NotifyType.Success, $"#{Thread.CurrentThread.ManagedThreadId.ToString()} Product count:{products.Count()} added {sw.Elapsed.TotalSeconds}"));
            }
            catch (Exception ex)
            {
                Messages.Add((NotifyType.Error, String.Format($"GetProductTypeModelAndBlockingCollection index:{index} - Error: {0}", ex.Message)));
            }
        }

        private ProductTypeModel PrepareProductTypeModel(Core.Domain.Catalog.Product nopProduct, GittiGidiyorSettings config)
        {
            #region prepareProductType

            #region prepare Product Specs

            string targetBrandName = "";

            var brandIdMapping = config.BrandIdMapping;

            if (!String.IsNullOrEmpty(brandIdMapping))
            {
                try
                {
                    var results = brandIdMapping.Split(";");

                    for (int i = 0; i < results.Length; i++)
                    {
                        var nopBrand = results[i].Split("=>")[0];

                        if (nopProduct.ProductManufacturers.Select(x => x.Id).Contains(int.Parse(nopBrand)))
                        {
                            targetBrandName = results[i].Split("=>")[1];
                            break;
                        }
                    }

                    if (targetBrandName == "")
                        targetBrandName = config.DefaultBrandName;
                }
                catch (Exception)
                {
                    targetBrandName = config.DefaultBrandName;
                }
            }
            else
            {
                targetBrandName = config.DefaultBrandName;
            }

            var brandSpec = new Models.Spec();
            brandSpec.Name = "Marka";
            brandSpec.Type = "Combo";
            brandSpec.Value = targetBrandName;
            brandSpec.Required = "true";

            var statusSpec = new Models.Spec();
            statusSpec.Name = "Durum";
            statusSpec.Type = "Combo";
            statusSpec.Value = "Sıfır";
            statusSpec.Required = "true";

            var specTypeList = new Models.Specs();
            specTypeList.Spec = new List<Models.Spec>();
            specTypeList.Spec.Add(brandSpec);
            specTypeList.Spec.Add(statusSpec);

            #endregion

            #region prepare Product Photo

            var photo = new Models.Photo();
            photo.PhotoId = "0";
            //photo.Url = _pictureService.GetPictureUrl(_pictureService.GetPicturesByProductId(nopProduct.Id).FirstOrDefault());
            photo.Url = "https://www.dakikdizayn.com/deneme.jpeg";

            var photoTypeList = new Photos();
            photoTypeList.Photo = photo;

            #endregion

            #region prepare Cargo Detail

            var cargoDetail = new Models.CargoDetail();
            cargoDetail.City = "34";

            var cargoCompanyList = new Models.CargoCompanies();
            cargoCompanyList.CargoCompany = new List<string>();
            cargoCompanyList.CargoCompany.Add(config.CargoCompany);
            cargoDetail.CargoCompanies = cargoCompanyList;

            var cargoCompanyDetailList = new Models.CargoCompanyDetails();
            cargoCompanyDetailList.CargoCompanyDetail = new List<CargoCompanyDetail>();
            cargoCompanyDetailList.CargoCompanyDetail.Add(new CargoCompanyDetail { Name = config.CargoCompany, CityPrice = config.CityPrice, CountryPrice = config.CountryPrice });
            cargoDetail.CargoCompanyDetails = cargoCompanyDetailList;

            cargoDetail.ShippingPayment = "B";
            cargoDetail.ShippingWhere = "country";

            var cargoShippingTime = new Models.ShippingTime() { Days = config.ShippingTimeDays };
            cargoDetail.ShippingTime = cargoShippingTime;

            #endregion

            #region Category Mapping

            string targetCategory = "";

            var categoryIdMapping = config.CategoryIdMapping;

            if (!String.IsNullOrEmpty(categoryIdMapping))
            {
                try
                {
                    var results = categoryIdMapping.Split(";");

                    for (int i = 0; i < results.Length; i++)
                    {
                        var nopCat = results[i].Split("=>")[0];

                        if (nopProduct.ProductCategories.Select(x => x.Id).Contains(int.Parse(nopCat)))
                        {
                            targetCategory = results[i].Split("=>")[1];
                            break;
                        }
                    }

                    if (targetCategory == "")
                        targetCategory = config.DefaultCategoryId;
                }
                catch (Exception)
                {
                    targetCategory = config.DefaultCategoryId;
                }
            }
            else
            {
                targetCategory = config.DefaultCategoryId;
            }

            #endregion

            #endregion

            var request = new Models.Request();

            var product = new Models.Product
            {
                CategoryCode = targetCategory,
                Title = nopProduct.Name,
                Specs = specTypeList,
                Photos = photoTypeList,
                PageTemplate = "1",
                Description = nopProduct.FullDescription,
                Format = "S",
                BuyNowPrice = nopProduct.Price.ToString(),
                ListingDays = config.ListingDays,
                ProductCount = nopProduct.StockQuantity <= 9999 ? nopProduct.StockQuantity.ToString() : "9999",
                CargoDetail = cargoDetail,
                StartDate = DateTime.Now.AddMinutes(1).ToString("yyyy-MM-dd HH':'mm':'ss"),
                AffiliateOption = "false",
                BoldOption = "false",
                CatalogOption = "false",
                VitrineOption = "false"
            };

            request.Product = product;

            var productTypeModel = new ProductTypeModel
            {
                Request = request,
                ProductId = nopProduct.Id.ToString()
            };

            return productTypeModel;
        }

        private int InsertProduct(RestClient client, GittiGidiyorSettings config, ProductTypeModel productTypeModel)
        {
            try
            {
                var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var sign = EncryptWithMD5(String.Concat(config.ApiKey, config.SecretKey, time));

                var result = client.PostAsync<ProductServiceResponse>(String.Format("/listingapi/rlws/individual/product?method=insertProductWithNewCargoDetail&outputCT=json&inputCT=xml&apiKey={0}&sign={1}&time={2}&itemId={3}&lang={4}&forceToSpecEntry={5}&nextDateOption={6}", config.ApiKey, sign, time, productTypeModel.ProductId.ToString(), config.Lang, config.ForceToSpecEntry, config.NextDateOption), productTypeModel.Request).Result;

                if (result != null && result.Data.AckCode.Equals("success"))
                {
                    CheckFailedProductIdsSetting(int.Parse(productTypeModel.ProductId));
                    Messages.Add((NotifyType.Success, $"gittigidiyor insertproductasync success"));
                    return int.Parse(productTypeModel.ProductId);
                }
                else
                {
                    SetFailedProductIdsSetting(int.Parse(productTypeModel.ProductId));
                    Messages.Add((NotifyType.Error, $"gittigidiyor insertproductasync error: {result.Data.Error.Message}"));
                    return 0;
                }
            }
            catch (Exception)
            {
                throw new NopException($"GittiGidiyor Product Service Error");
            }
        }

        private List<Domain.Category> GetCategories(GittiGidiyorSettings config)
        {
            var client = new RestClient("http://dev.gittigidiyor.com:8080/");
            client.Authentication = new BasicAuthenticator(config.RoleName, config.RolePass);

            var list = new List<Domain.Category>();
            var startOffSet = 0;
            var baseResult = GetCategoriesPaging(client, startOffSet);
            int.TryParse(baseResult.Data.CategoryCount, out int categoryCount);

            if (baseResult.Data != null)
            {
                try
                {
                    CategoriesResultAddRange(list, baseResult.Data.Categories.Category.ToList());
                }
                catch (Exception)
                {
                }
            }

            for (int i = 1; i < (categoryCount / 100) + 1; i++)
            {
                var result = GetCategoriesPaging(client, i * 100);

                try
                {
                    CategoriesResultAddRange(list, result.Data.Categories.Category.ToList());
                }
                catch (Exception)
                {
                }
            }

            return list;
        }

        private IRestResponse<CategoryServiceResponse> GetCategoriesPaging(RestClient client, int startOffSet)
        {
            return client.GetAsync<CategoryServiceResponse>("/listingapi/rlws/anonymous/category?method=getCategories&outputCT=json&startOffSet=" + startOffSet + "&rowCount=100&withSpecs=true&withDeepest=true&withCatalog=true&lang=tr").Result;
        }

        private List<Domain.Category> CategoriesResultAddRange(List<Domain.Category> list, List<Domain.Category> listToAdd)
        {
            list.AddRange(listToAdd);
            return list;
        }

        private void SetLastAddedProductIdSetting(int lastAddedProductId)
        {
            if (lastAddedProductId > 0)
            {
                var storeId = _storeContext.ActiveStoreScopeConfiguration;
                var gittigidiyorSettings = _settingService.LoadSetting<GittiGidiyorSettings>(storeId);
                if (lastAddedProductId > gittigidiyorSettings.LastAddedProductId)
                {
                    gittigidiyorSettings.LastAddedProductId = lastAddedProductId;
                    _settingService.SaveSetting(gittigidiyorSettings, settings => settings.LastAddedProductId, clearCache: false);
                    _settingService.ClearCache();
                }
            }
        }

        private void SetFailedProductIdsSetting(int failedProductId)
        {
            if (failedProductId > 0)
            {
                var storeId = _storeContext.ActiveStoreScopeConfiguration;
                var gittigidiyorSettings = _settingService.LoadSetting<GittiGidiyorSettings>(storeId);
                gittigidiyorSettings.FailedProductIds += (failedProductId + ";");
                _settingService.SaveSetting(gittigidiyorSettings, settings => settings.LastAddedProductId, clearCache: false);
                _settingService.ClearCache();
            }
        }

        private void CheckFailedProductIdsSetting(int productId)
        {
            if (productId > 0)
            {
                var storeId = _storeContext.ActiveStoreScopeConfiguration;
                var gittigidiyorSettings = _settingService.LoadSetting<GittiGidiyorSettings>(storeId);
                gittigidiyorSettings.FailedProductIds = gittigidiyorSettings.FailedProductIds.Replace(productId + ";", "");
                _settingService.SaveSetting(gittigidiyorSettings, settings => settings.FailedProductIds, clearCache: false);
                _settingService.ClearCache();
            }
        }

        public string EncryptWithMD5(string clearString)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(clearString);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}