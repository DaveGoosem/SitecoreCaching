using System;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using SitecoreCaching.SC.Framework.Extensions;

namespace SitecoreCaching.SC.Framework.Helpers
{
    public static class SitecoreItem
    {
        /// <summary>
        /// Gets the current sitecore database.
        /// </summary>
        public static Database Db
        {
            get { return Context.Database; }
        }

        /// <summary>
        /// Gets the single item.
        /// </summary>
        /// <param name="xPath">The x path.</param>
        /// <returns></returns>
        public static Item GetSingleItem(string xPath)
        {
            using (new SecurityDisabler())
            {
                return Db.SelectSingleItem(xPath);
            }
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        public static Item CurrentItem
        {
            get { return Context.Item; }
        }

        /// <summary>
        /// Gets the current language.
        /// </summary>
        public static Language CurrentLanguage
        {
            get { return Context.Language; }
        }

        /// <summary>
        /// Gets the name of the current language.
        /// </summary>
        /// <value>
        /// The name of the current language.
        /// </value>
        public static string CurrentLanguageName
        {
            get { return CurrentLanguage.Name.ToLower(); }
        }

        /// <summary>
        /// Gets the home item.
        /// </summary>
        public static Item HomeItem
        {
            get { return GetSingleItem(Context.Site.StartPath); }
        }

        /// <summary>
        /// Gets the home item.
        /// </summary>
        public static Item LoginItem
        {
            get { return GetSingleItem(Context.Site.StartPath + Context.Site.LoginPage); }
        }

        /// <summary>
        /// Gets the home item path.
        /// </summary>
        public static string HomeItemPath
        {
            get { return Context.Site.StartPath; }
        }

        /// <summary>
        /// Gets the context start item.
        /// </summary>
        public static Item WebsiteItem
        {
            get { return GetSingleItem(Context.Site.ContentStartPath); }
        }

        /// <summary>
        /// Gets the website item path.
        /// </summary>
        public static string WebsiteItemPath
        {
            get { return Context.Site.ContentStartPath; }
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        public static string Title
        {
            get { return CurrentItem.GetFieldValue("Title", CurrentItem.DisplayName); }
        }

        /// <summary>
        /// Gets the meta title.
        /// </summary>
        public static string MetaTitle
        {
            get { return CurrentItem.GetFieldValue("MetaTitle", Title); }
        }

        public static bool IsPageEditorEditing
        {
            get { return Context.PageMode.IsPageEditorEditing; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is page preview.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page preview; otherwise, <c>false</c>.
        /// </value>
        public static bool IsPagePreview
        {
            get { return Context.PageMode.IsPreview; }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Item GetItem(string id, Language language = null)
        {
            using (new SecurityDisabler())
            {
                if (language == null)
                {
                    return Db.GetItem(id);
                }
                return Db.GetItem(id, language);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Item GetItem(ID id, Language language = null)
        {
            using (new SecurityDisabler())
            {
                if (language == null)
                {
                    return Db.GetItem(id);
                }
                return Db.GetItem(id, language);
            }
        }

        /// <summary>
        /// Gets the navigation title.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static string GetNavigationTitle(Item item, string fieldName = "NavigationTitle")
        {
            using (new SecurityDisabler())
            {
                return item.GetFieldValue(fieldName, item.GetItemNameForDisplay());
            }
        }

        /// <summary>
        /// Gets the breadcrumb title.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static string GetBreadcrumbTitle(Item item, string fieldName = "BreadcrumbTitle")
        {
            using (new SecurityDisabler())
            {
                return item.GetFieldValue(fieldName, GetNavigationTitle(item));
            }
        }

        /// <summary>
        /// Get translated text from dictionary item
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetTranslateText(string key)
        {
            string itemValue = Translate.Text(key);
            if (!String.IsNullOrEmpty(itemValue))
                return itemValue;

            return String.Empty;
        }

        /// <summary>
        /// Gets the item level from web site item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static int GetItemLevelFromWebSiteItem(Item item)
        {
            return item.Axes.Level - WebsiteItem.Axes.Level;
        }

        /// <summary>
        /// Gets the item level from home item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static int GetItemLevelFromHomeItem(Item item)
        {
            return item.Axes.Level - HomeItem.Axes.Level;
        }
    }
}