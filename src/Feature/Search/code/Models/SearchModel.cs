using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System.Collections.Generic;


namespace TaskHelix.Feature.Search.Models
{
    public class SearchModel : SearchResultItem
    {
        [IndexField("_name")]
        public virtual string Category { get; set; }
        [IndexField("ShortDescription_t")]
        public virtual string ShortDescription { get; set; } // Custom field on my template

        [IndexField("PostedDate_t")]
        public virtual string PostedDate { get; set; }
       

        [IndexField("description_t")]
        public virtual string LongDescription { get; set; } // Custom field on my template

        [IndexField("ArticleLargeImage_t")]
        public virtual string ArticleLargeImage { get; set; } // Custom field on my template
    }

    public class SearchResult
    {
        public string Category { get; set; }
        public string ShortDescription { get; set; }
        public string PostedDate { get; set; }
        public string ArticleLargeImage { get; set; }
        public string LongDescription { get; set; }
    }

    /// <summary>
    /// Custom search result model for binding to front end
    /// </summary>
    public class SearchResults
    {
        public List<SearchResult> Results { get; set; }
    }
}