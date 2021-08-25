using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System.Collections.Generic;


namespace TaskHelix.Feature.Search.Models
{
    public class BlogCard
    {
        public string Id { get; set; }

        public string Tittle { get; set; }

        public string ArticleSmalImage { get; set; }

        public string date { get; set; }

        public string Category { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string PostCardButtonText { get; set; }

        public Sitecore.Data.Items.Item sitecoreItem { get; set; }

        public string BlogURL { get; set; }


    }

}