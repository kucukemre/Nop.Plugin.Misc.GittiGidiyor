using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nop.Plugin.Misc.GittiGidiyor.Models
{
    public class ProductTypeModel
    {
        public string ProductId { get; set; }
        public Request Request { get; set; }
    }

    [XmlRoot(ElementName = "request")]
    public class Request
    {
        [XmlElement(ElementName = "product")]
        public Product Product { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {
        [XmlElement(ElementName = "categoryCode")]
        public string CategoryCode { get; set; }
        [XmlElement(ElementName = "storeCategoryId")]
        public string StoreCategoryId { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "subTitle")]
        public string SubTitle { get; set; }
        [XmlElement(ElementName = "specs")]
        public Specs Specs { get; set; }
        [XmlElement(ElementName = "photos")]
        public Photos Photos { get; set; }
        [XmlElement(ElementName = "pageTemplate")]
        public string PageTemplate { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "startDate")]
        public string StartDate { get; set; }
        [XmlElement(ElementName = "catalogId")]
        public string CatalogId { get; set; }
        [XmlElement(ElementName = "catalogDetail")]
        public string CatalogDetail { get; set; }
        [XmlElement(ElementName = "format")]
        public string Format { get; set; }
        [XmlElement(ElementName = "startPrice")]
        public string StartPrice { get; set; }
        [XmlElement(ElementName = "buyNowPrice")]
        public string BuyNowPrice { get; set; }
        [XmlElement(ElementName = "netearning")]
        public string Netearning { get; set; }
        [XmlElement(ElementName = "listingDays")]
        public string ListingDays { get; set; }
        [XmlElement(ElementName = "productCount")]
        public string ProductCount { get; set; }
        [XmlElement(ElementName = "cargoDetail")]
        public CargoDetail CargoDetail { get; set; }
        [XmlElement(ElementName = "affiliateOption")]
        public string AffiliateOption { get; set; }
        [XmlElement(ElementName = "boldOption")]
        public string BoldOption { get; set; }
        [XmlElement(ElementName = "catalogOption")]
        public string CatalogOption { get; set; }
        [XmlElement(ElementName = "vitrineOption")]
        public string VitrineOption { get; set; }
    }

    [XmlRoot(ElementName = "cargoDetail")]
    public class CargoDetail
    {
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "cargoCompanies")]
        public CargoCompanies CargoCompanies { get; set; }
        [XmlElement(ElementName = "shippingPayment")]
        public string ShippingPayment { get; set; }
        [XmlElement(ElementName = "cargoDescription")]
        public string CargoDescription { get; set; }
        [XmlElement(ElementName = "shippingWhere")]
        public string ShippingWhere { get; set; }
        [XmlElement(ElementName = "cargoCompanyDetails")]
        public CargoCompanyDetails CargoCompanyDetails { get; set; }
        [XmlElement(ElementName = "shippingTime")]
        public ShippingTime ShippingTime { get; set; }
    }

    [XmlRoot(ElementName = "cargoCompanies")]
    public class CargoCompanies
    {
        [XmlElement(ElementName = "cargoCompany")]
        public List<string> CargoCompany { get; set; }
    }

    [XmlRoot(ElementName = "shippingTime")]
    public class ShippingTime
    {
        [XmlElement(ElementName = "days")]
        public string Days { get; set; }
    }

    [XmlRoot(ElementName = "cargoCompanyDetails")]
    public class CargoCompanyDetails
    {
        [XmlElement(ElementName = "cargoCompanyDetail")]
        public List<CargoCompanyDetail> CargoCompanyDetail { get; set; }
    }

    [XmlRoot(ElementName = "cargoCompanyDetail")]
    public class CargoCompanyDetail
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "cityPrice")]
        public string CityPrice { get; set; }
        [XmlElement(ElementName = "countryPrice")]
        public string CountryPrice { get; set; }
    }

    [XmlRoot(ElementName = "photos")]
    public class Photos
    {
        [XmlElement(ElementName = "photo")]
        public Photo Photo { get; set; }
    }

    [XmlRoot(ElementName = "photo")]
    public class Photo
    {
        [XmlAttribute(AttributeName = "photoId")]
        public string PhotoId { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "specs")]
    public class Specs
    {
        [XmlElement(ElementName = "spec")]
        public List<Spec> Spec { get; set; }
    }

    [XmlRoot(ElementName = "spec")]
    public class Spec
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "required")]
        public string Required { get; set; }
    }
}