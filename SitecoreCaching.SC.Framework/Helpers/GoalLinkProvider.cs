using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace SitecoreCaching.SC.Framework.Helpers
{
    public class GoalLinkProvider : Sitecore.Links.LinkProvider
    {
        public string GetItemUrl(Sitecore.Data.Items.Item item, Sitecore.Links.UrlOptions options, string goalId)
        {
            Assert.ArgumentNotNull(item, "item");
            string baseUrl = base.GetItemUrl(item, options);
            Item goalItem = Sitecore.Context.Database.GetItem(goalId);
            if (goalItem != null)
            {
                // build the href
                string key = Settings.GetSetting("Analytics.EventQueryStringKey", "sc_trk");
                return string.Format("{0}{1}{2}={3}", baseUrl, baseUrl.IndexOf('?') > -1 ? '&' : '?', key, goalItem.Name);
            }
            return baseUrl;
        }
    }
}
