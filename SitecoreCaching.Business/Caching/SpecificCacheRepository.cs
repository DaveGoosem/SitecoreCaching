using Sitecore.Data.Items;
using SitecoreCaching.SC.Framework.Extensions;
using SitecoreCaching.SC.Framework.Helpers;

namespace SitecoreCaching.Business.Caching
{
    public class SpecificCacheRepository : SitecoreCacheRepository
    {
        public SpecificCacheRepository()
        {
            DataSourceItem = CacheConfigItem;
        }

        private Item _cacheConfigItem;
        public Item CacheConfigItem
        {
            get { return _cacheConfigItem ?? (_cacheConfigItem = GetCacheConfigItem()); }
            set { _cacheConfigItem = value; }
        }

        /// <summary>
        /// This method is to locate the caching datasource item from wherever it is located in your solution
        /// </summary>
        /// <returns></returns>
        private Item GetCacheConfigItem()
        {
            var configItem = SitecoreItem.WebsiteItem.GetFieldValueAsItem("ConfigurationItem");
            if (configItem != null)
            {
                var cacheConfigItem = configItem.GetFieldValueAsItem("CacheConfigItem");
                if (cacheConfigItem != null)
                    return cacheConfigItem.GetFieldValueAsItem("CachingSettingsItem") ?? Sitecore.Context.Item;
            }
            return Sitecore.Context.Item;
        }
    }
}
