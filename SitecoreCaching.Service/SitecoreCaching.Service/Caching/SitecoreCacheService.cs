using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using SitecoreCaching.SC.Framework.Extensions;

namespace SitecoreCaching.Service.Caching
{
    public class SitecoreCacheService : CustomCache
    {
        public SitecoreCacheService(string name, string maxSize)
            : base(name, StringUtil.ParseSizeString(maxSize))
        {
            int days = 0, hours = 12, mins = 0, seconds = 0;
            Item cacheSettingsItem = DataSourceItem;

            if (cacheSettingsItem != null)
            {
                days = GetTimePeriod(cacheSettingsItem, "SitecoreCachingDays");
                hours = GetTimePeriod(cacheSettingsItem, "SitecoreCachingHours");
                mins = GetTimePeriod(cacheSettingsItem, "SitecoreCachingMinutes");
                seconds = GetTimePeriod(cacheSettingsItem, "SitecoreCachingSeconds");
            }
            CacheDuration = new TimeSpan(days, hours, mins, seconds); //defaults to 12 hours

        }

        /// <summary>
        /// Cache duration is the keep alive time for items stored in this Sitecore Caching instance.
        /// It has a default value of 12 hours unless specified by data source item or in code.
        /// </summary>
        public TimeSpan CacheDuration { get; set; }

        /// <summary>
        /// Add new data to the cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddData<T>(object key, object value)
        {
            InnerCache.Add(key, value, GetObjectCacheSize(value), CacheDuration);
        }

        /// <summary>
        /// Get data from cache with the supplied cacheKey value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>        
        public T GetData<T>(object cacheKey)
        {
            var retVal = default(T);
            try
            {
                retVal = (T)InnerCache.GetValue(cacheKey);
            }
            catch (Exception)
            {
            }
            return retVal;
        }

        /// <summary>
        /// remove data with specific key from Sitecore Cache
        /// </summary>
        /// <param name="key"></param>
        public void RemoveData(object key)
        {
            InnerCache.Remove(key);
        }

        #region helpers

        public Item DataSourceItem { get; set; }

        /// <summary>
        /// used to convert string value from sitecore item fields into integers to populate CacheDuration etc.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldValue"></param>        
        private int GetTimePeriod(Item item, string fieldValue)
        {
            int timePeriod = 0;

            if (item != null)
            {
                var timePeriodString = item.GetFieldValue(fieldValue);
                if (!string.IsNullOrEmpty(timePeriodString))
                {
                    try
                    {
                        timePeriod = int.Parse(timePeriodString);
                    }
                    catch (Exception e)
                    {
                        Log.Error("unable to convert caching time period to an int value: ", e);
                    }
                }
                else
                {
                    //field is empty, so return default time period int value
                    return timePeriod;
                }
            }
            return timePeriod;
        }

        /// <summary>
        /// Get the size of an object as a long type (to appease the Sitecore caching gods)
        /// </summary>
        /// <param name="obj"></param>        
        public long GetObjectCacheSize(object obj)
        {
            var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms.Length;
        }

        #endregion
    }
}
