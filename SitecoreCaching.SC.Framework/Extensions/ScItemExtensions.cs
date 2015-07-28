using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.WebControls;
using SitecoreCaching.SC.Framework.Helpers;

namespace SitecoreCaching.SC.Framework.Extensions
{
    public static class ScItemExtensions
    {
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string GetFieldValue(this Item item, string fieldName, string defaultValue = "")
        {
            using (new SecurityDisabler())
            {
                if (item != null && !string.IsNullOrEmpty(item[fieldName]))
                    return item[fieldName];

                return defaultValue;
            }
        }

        public static string GetItemNameForDisplay(this Item item)
        {
            string name = string.Empty;

            if (item.Fields["Title"] != null && !string.IsNullOrWhiteSpace(item.Fields["Title"].Value))
                name = item.Fields["Title"].Value;
            else if (!string.IsNullOrWhiteSpace(item.DisplayName))
                name = item.DisplayName;
            else
                name = item.Name;

            return name.Trim();
        }

        public static string GetItemNameForNavigation(this Item item)
        {
            string name = string.Empty;

            if (item.Fields["NavigationTitle"] != null && !string.IsNullOrWhiteSpace(item.Fields["NavigationTitle"].Value))
                name = item.Fields["NavigationTitle"].Value;
            else
                name = item.GetItemNameForDisplay();
            return name;
        }

        public static ChildList GetChildrenFiltered(this Item item)
        {
            return new ChildList(item, item.GetChildren().Where(x => x.Visualization.Layout != null).ToList());
        }

        /// <summary>
        /// Gets the friendly URL.
        /// </summary>
        public static string GetFriendlyUrl(this Item item, bool includeServerName = false, string goalId = null)
        {
            try
            {
                var options = LinkManager.GetDefaultUrlOptions();
                options.AlwaysIncludeServerUrl = includeServerName;
                if (!string.IsNullOrEmpty(goalId))
                {
                    return new GoalLinkProvider().GetItemUrl(item, options, goalId);
                }
                else
                    return LinkManager.GetItemUrl(item, options);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns></returns>
        public static string GetMediaUrl(this MediaItem item, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0)
        {
            if (item == null)
            {
                return string.Empty;
            }

            // specify options and properties if any
            MediaUrlOptions options = new MediaUrlOptions();
            if (width != 0) options.Width = width; // set width if specified
            if (height != 0) options.Height = height; // set height if specified
            if (maxWidth != 0) options.MaxWidth = maxWidth; // set MaxWidth if specified
            if (maxHeight != 0) options.MaxHeight = maxHeight; // set MaxHeight if specified

            // return media url
            return MediaManager.GetMediaUrl(item, options);
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <param name="maxHeight">Height of the max.</param>
        /// <returns></returns>
        public static string GetMediaUrl(this Item item, string fieldName, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0)
        {
            ImageField field = item.Fields[fieldName];
            if (field == null)
            {
                return string.Empty;
            }

            // get media item
            MediaItem mediaItem = field.MediaItem;

            // return media url
            return mediaItem.GetMediaUrl(width, height, maxWidth, maxHeight);
        }

        public static string GetFieldValue(this Item item, string fieldName)
        {
            if (item != null && !string.IsNullOrWhiteSpace(fieldName) && item.Fields[fieldName] != null)
            {
                return item.Fields[fieldName].Value;
            }

            return string.Empty;
        }

        public static string GetRenderedFieldValue(this Item item, string fieldName)
        {
            if (item != null && !string.IsNullOrWhiteSpace(fieldName) && item.Fields[fieldName] != null)
            {
                return FieldRenderer.Render(item, fieldName);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets Sitecore items from Multilist Field.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static Item[] GetFieldItems(this Item item, string fieldName)
        {
            MultilistField ctrl = item.Fields[fieldName];
            if (ctrl != null)
                return ctrl.GetItems();

            return (new Item[] { });
        }

        public static string GetGeneralLink(this Item item, string fieldName, bool includeServerName = true)
        {
            using (new SecurityDisabler())
            {
                if (item.Fields[fieldName] == null)
                {
                    return string.Empty;
                }

                LinkField field = (LinkField)item.Fields[fieldName];

                if (field.IsMediaLink)
                {
                    var targetItem = GetFieldValueAsTargetItem(item, fieldName);
                    if (targetItem != null)
                    {
                        var mediaItem = (MediaItem)targetItem;
                        if (mediaItem != null)
                            return GetMediaUrl(mediaItem);
                    }
                }

                if (field.IsInternal)
                {
                    if (field.TargetItem == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return field.TargetItem.GetFriendlyUrl(includeServerName, field.GetAttribute("goalId"));
                    }
                }
                else
                {
                    return field.Url;
                }
            }
        }

        public static DateTime? GetDateTime(this Item item, string fieldName)
        {
            DateTime? dateTime = null;
            if (item.Fields[fieldName] != null && !string.IsNullOrEmpty(item.Fields[fieldName].Value))
            {
                DateField field = (DateField)item.Fields[fieldName];
                dateTime = field.DateTime;
            }
            return dateTime;
        }

        public static string GetLinkDescription(this Item item, string fieldName, bool fallbackToItemName = false)
        {
            string description = string.Empty;
            Field field = item.Fields[fieldName];

            if (field != null)
            {
                LinkField linkField = (LinkField)field;

                description = linkField.Text;

                if (string.IsNullOrEmpty(description) && fallbackToItemName && linkField.IsInternal && linkField.TargetItem != null)
                {
                    description = linkField.TargetItem.DisplayName;
                }
            }
            return description;
        }

        public static Item[] GetMultilistItems(this Item item, string fieldName)
        {
            // Create empty list
            Item[] list = new Item[0];

            // Get multilist items
            Field field = item.Fields[fieldName];
            if (field != null)
            {
                MultilistField multilistField = (MultilistField)field;
                if (multilistField != null)
                {
                    list = multilistField.GetItems();
                }
            }

            return list;
        }

        /// <summary>
        /// Determines whether [is media item] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [is media item] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMediaItem(this Item item)
        {
            if (item != null && item.Paths != null)
            {
                return item.Paths.IsMediaItem;
            }

            return false;
        }

        public static bool IsExcelItem(this Item item)
        {
            var isExcelItem = false;
            var ext = Sitecore.Resources.Media.MediaManager.GetMedia(item).Extension.ToLower();

            if (ext == "xls" || ext == "xlsx")
            {
                isExcelItem = true;
            }

            return isExcelItem;
        }

        public static bool IsPdfItem(this Item item)
        {
            var isPdfItem = false;
            var ext = Sitecore.Resources.Media.MediaManager.GetMedia(item).Extension.ToLower();

            if (ext == "pdf")
            {
                isPdfItem = true;
            }

            return isPdfItem;
        }

        public static bool IsWordItem(this Item item)
        {
            var isWordItem = false;
            var ext = Sitecore.Resources.Media.MediaManager.GetMedia(item).Extension.ToLower();

            if (ext == "doc" || ext == "docx")
            {
                isWordItem = true;
            }

            return isWordItem;
        }


        public static bool IsChecked(this Item datasource, string fieldName)
        {
            bool check = false;
            if (datasource != null)
            {
                Field field = datasource.Fields[fieldName];
                if (field != null)
                {
                    CheckboxField checkbox = (CheckboxField)field;
                    check = checkbox != null && checkbox.Checked;
                }
            }
            return check;
        }


        public static bool IsHiddenPage(this Item item)
        {
            Boolean hidePage = false;
            if (item.Fields["HidePage"] != null)
            {
                hidePage = ((CheckboxField)item.Fields["HidePage"]).Checked;
            }
            return hidePage;
        }

        public static bool IsPageHiddenFromSearchResults(this Item item)
        {
            var hidePageFromSearchResults = false;
            if (item.Fields["HidePageFromSearchResults"] != null)
            {
                hidePageFromSearchResults = ((CheckboxField)item.Fields["HidePageFromSearchResults"]).Checked;
            }
            return hidePageFromSearchResults;
        }


        /// <summary>
        /// Gets the field value item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static Item GetFieldValueAsItem(this Item item, string fieldName)
        {
            using (new SecurityDisabler())
            {
                string fieldValue = item.GetFieldValue(fieldName, string.Empty);
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    return SitecoreItem.Db.GetItem(fieldValue);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the field value item from a specified database.
        /// </summary>
        public static Item GetFieldValueAsItem(this Item item, string fieldName, Database db)
        {
            using (new SecurityDisabler())
            {
                string fieldValue = item.GetFieldValue(fieldName, string.Empty);
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    return db.GetItem(fieldValue);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the field value as target item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static Item GetFieldValueAsTargetItem(this Item item, string fieldName)
        {
            using (new SecurityDisabler())
            {
                if (item != null)
                {
                    var field = (LinkField)item.Fields[fieldName];

                    if (field != null && field.TargetItem != null && (field.IsInternal || field.IsMediaLink))
                    {
                        return field.TargetItem;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the field value as image field.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static ImageField GetFieldValueAsImageField(this Item item, string fieldName)
        {
            ImageField imageField = item.Fields[fieldName];
            return imageField;
        }

        /// <summary>
        /// Gets the field value as media item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static MediaItem GetFieldValueAsMediaItem(this Item item, string fieldName)
        {
            MediaItem mediaItem = item.GetFieldValueAsItem(fieldName);
            if (mediaItem == null)
            {
                return null;
            }

            // return media item
            return mediaItem;
        }

        /// <summary>
        /// Gets the field value as media SRC.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="hasImageOrFileType">if set to <c>true</c> [has image or file type].</param>
        /// <returns></returns>
        public static MediaItem GetFieldValueAsMediaItem(this Item item, string fieldName, bool hasImageOrFileType)
        {
            if (hasImageOrFileType)
            {
                ImageField imageField = item.GetFieldValueAsImageField(fieldName);
                if (imageField == null)
                {
                    return null;
                }
                return imageField.MediaItem;
            }

            return GetFieldValueAsMediaItem(item, fieldName);
        }

        /// <summary>
        /// Gets the children for navigation.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static Item[] GetChildrenForNavigation(this Item item)
        {
            using (new SecurityDisabler())
            {
                if (item != null && item.HasChildren)
                {
                    // HideFromAll and HideFromNavigation must not selected
                    Item[] children = item.GetChildren().ToArray();

                    return children.Where(c => c["HideFromAll"] != "1" && c["HideFromNavigation"] != "1").ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the children for Sitemap.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static Item[] GetChildrenForSitemap(this Item item)
        {
            using (new SecurityDisabler())
            {
                if (item != null && item.HasChildren)
                {
                    // HideFromAll and HideFromNavigation must not selected
                    Item[] children = item.GetChildren().ToArray();

                    return children.Where(c => c["HideFromAll"] != "1" && c["HideFromSitemap"] != "1").ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the breadcrumb items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static List<Item> GetBreadcrumbItems(this Item item)
        {
            using (new SecurityDisabler())
            {
                // HideFromAll and HideFromBreadcrumb must not selected
                return item.Axes.GetAncestors().Where(c => c.Axes.Level >= SitecoreItem.HomeItem.Axes.Level && c["HideFromAll"] != "1" && c["HideFromBreadcrumb"] != "1").ToList();
            }
        }

    }
}