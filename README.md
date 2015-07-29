# SitecoreCaching

Current Version built using Sitecore 7.2 rev 140228

This library utilises Sitecore caching to set up a custom named cache, accessible from /sitecore/admin/cache.aspx page. This means you can clear it when you like using this page.

It allows for a Sitecore data template item to be created and used to configure your custom Sitecore cache, using the following template fields:

- SitecoreCacheMaxSize - SingleLineText (set using a string and the size following it, EG. "5MB")
- SitecoreCacheName - SintleLineText
- SitecoreCachingDays - SingleLineText
- SitecoreCachingHours - SingleLineText
- SitecoreCachingMunutes - SingleLineText
- SitecoreCachingSeconds - SingleLineText

It will default to 12 hours if this is not set and use a default name of "DefaultCustomCache" and default max size of "5MB".

## Usage
For each custom Sitecore cache you would like to configure, you should create a new Sitecore item based off the template (described above) and set up a new Repository inheriting from SitecoreCacheRepository where you simply get your DataSourceitem and pass it through (see SpecificCacheRepository.cs for example implementation).

This library can be easily extended to set the DataSourceItem another way or to just hard and fast set your name and size and keep alive time should you wish.
