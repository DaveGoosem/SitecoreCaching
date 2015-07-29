using System;
using SitecoreCaching.SC.Framework.Extensions;
using SitecoreCaching.SC.Framework.Helpers;
using SitecoreCaching.Service.Caching;
using Sitecore.Data.Items;

namespace SitecoreCaching.Business.Caching
{
    public class SitecoreCacheRepository
    {
        private static SitecoreCacheService _sitecoreCacheService;
        public static SitecoreCacheService Instance
        {
            get
            {
                if (_sitecoreCacheService == null)
                {
                    var dataSourceItem = DataSourceItem;
                    if (dataSourceItem != null)
                    {
                        string defaultSize = dataSourceItem.GetFieldValue("SitecoreCacheMaxSize");
                        string defaultName = dataSourceItem.GetFieldValue("SitecoreCacheName");
                        if (string.IsNullOrEmpty(defaultSize))
                            defaultSize = "5MB";
                        if (string.IsNullOrEmpty(defaultName))
                            defaultName = "DefaultCustomCache";
                        _sitecoreCacheService = new SitecoreCacheService(defaultName, defaultSize)
                        {
                            DataSourceItem = dataSourceItem
                        };
                    }
                }
                return _sitecoreCacheService;
            }
        }


        /// <summary>
        /// This can be passed in or set inside of a class that inherits this one (for situations where dependency injection gets in the way)
        /// </summary>
        public static Item DataSourceItem { get; set; }

        // This class exists to use as a base class, allowing repositories that
        // inherit from it to maintain their own private static objects
        public T GetCached<T>(bool isCachingEnabled, string cacheKey, Func<T> getterFunction)
        {
            if (!isCachingEnabled || typeof(T).IsValueType)
                return getterFunction();

            var cacheValue = Instance.GetData<T>(cacheKey);
            if (cacheValue != null)
                return cacheValue;

            var value = getterFunction();
            Instance.AddData<T>(cacheKey, value);
            return value;
        }

        public T GetCached<T, TInput1>(bool isCachingEnabled, string cacheKey, Func<TInput1, T> getterFunction, TInput1 input1)
        {
            if (!isCachingEnabled || typeof(T).IsValueType)
                return getterFunction(input1);

            var cacheValue = Instance.GetData<T>(cacheKey);
            if (cacheValue != null)
                return cacheValue;

            var value = getterFunction(input1);
            Instance.AddData<T>(cacheKey, value);
            return value;
        }

        public T GetCached<T, TInput1, TInput2>(bool isCachingEnabled, string cacheKey, Func<TInput1, TInput2, T> getterFunction, TInput1 input1, TInput2 input2)
        {
            if (!isCachingEnabled || typeof(T).IsValueType)
                return getterFunction(input1, input2);

            var cacheValue = Instance.GetData<T>(cacheKey);
            if (cacheValue != null)
                return cacheValue;

            var value = getterFunction(input1, input2);
            Instance.AddData<T>(cacheKey, value);
            return value;
        }
    }
}
