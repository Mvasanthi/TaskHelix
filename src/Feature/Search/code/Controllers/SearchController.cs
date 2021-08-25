using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Linq.Expressions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using TaskHelix.Feature.Search.Models;

using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;


namespace TaskHelix.Feature.Search.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult getBlogCard()
        {

            List<Sitecore.Data.Items.Item> blogItems = new List<Sitecore.Data.Items.Item>();
            List<BlogCard> BlogCardsCollection = new List<BlogCard>();
            var rootitem = Sitecore.Context.Database.GetItem("{6477F8F3-68FD-4899-A25D-BDAC129DEF92}");
            var Websitesettings = Sitecore.Context.Database.GetItem("{D87CA97C-B541-4371-B04C-9606296DC26C}");
            //var item1 = Sitecore.Context.Database.GetItem("{D87CA97C-B541-4371-B04C-9606296DC26C}");


            blogItems = rootitem.Axes.GetDescendants().ToList();

            foreach (Sitecore.Data.Items.Item items in blogItems.ToList())
            {
                if (items.TemplateID.ToString() != "{26FF5C6E-800A-46CF-8337-B8D11793C5ED}")
                {
                    blogItems.Remove(items);
                }
            }

            for (int i = 0; i < blogItems.Count; i++)
            {
                BlogCard BlogModel = new BlogCard();
                var imageUrl = string.Empty;

                Sitecore.Data.Fields.ImageField imageField = blogItems[i].Fields["ArticleSmalImage"];
                if (imageField?.MediaItem != null)
                {
                    var image = new MediaItem(imageField.MediaItem);
                    imageUrl = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
                    BlogModel.ArticleSmalImage = imageUrl;
                }
                BlogModel.Category = blogItems[i].Fields["Category"].Value;
                BlogModel.Tittle = blogItems[i].Fields["Tittle"].Value;

                Sitecore.Data.Fields.DateField dateTimeField = blogItems[i].Fields["PostedDate"];

                string dateTimeString = dateTimeField.Value;

                DateTime dateTimeStruct = Sitecore.DateUtil.IsoDateToDateTime(dateTimeString);
                BlogModel.date = dateTimeStruct.ToShortDateString();

                BlogModel.ShortDescription = blogItems[i].Fields["ShortDescription"].Value;
                BlogModel.PostCardButtonText = Websitesettings.Fields["PostCardButtonText"].Value;
                BlogModel.sitecoreItem = blogItems[i];
                BlogModel.BlogURL = Sitecore.Links.LinkManager.GetItemUrl(blogItems[i]);


                BlogCardsCollection.Add(BlogModel);
            }


            ViewBag.BlogCards = BlogCardsCollection;
            return View("~/Views/Search/Mainbody.cshtml", ViewBag.BlogCards);

        }







        public ActionResult DoSearch(string searchText)
        {
            var myResults = new SearchResults
            {
                Results = new List<SearchResult>()
            };
            var searchIndex = ContentSearchManager.GetIndex("sitecore_web_index"); // Get the search index
            var searchPredicate = GetSearchPredicate(searchText); // Build the search predicate
            using (var searchContext = searchIndex.CreateSearchContext()) // Get a context of the search index
            {
                //Select * from Sitecore_web_index Where Author="searchText" OR Description="searchText" OR Title="searchText"
                //var searchResults = searchContext.GetQueryable<SearchModel>().Where(searchPredicate); // Search the index for items which match the predicate
                var searchResults = searchContext.GetQueryable<SearchModel>()
                    .Where(x => x.LongDescription.Contains(searchText) || x.ShortDescription.Contains(searchText) || x.LongDescription.Contains(searchText) || x.Category.Contains(searchText));   //LINQ query

                var fullResults = searchResults.GetResults();

                // This is better and will get paged results - page 1 with 10 results per page
                //var pagedResults = searchResults.Page(1, 10).GetResults();
                foreach (var hit in fullResults.Hits)
                {
                    myResults.Results.Add(new SearchResult
                    {
                        LongDescription = hit.Document.LongDescription,
                        PostedDate = hit.Document.PostedDate,
                        Category = hit.Document.Category,
                        ShortDescription = hit.Document.ShortDescription
                    });
                }
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = myResults };
            }

        }

        /// <summary>
        /// Search logic
        /// </summary>
        /// <param name="searchText">Search term</param>
        /// <returns>Search predicate object</returns>
        public static Expression<Func<SearchModel, bool>> GetSearchPredicate(string searchText)
        {
            var predicate = PredicateBuilder.True<SearchModel>(); // Items which meet the predicate
                                                                  // Search the whole phrase - LIKE
                                                                  // predicate = predicate.Or(x => x.DispalyName.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Description.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Title.Like(searchText)).Boost(1.2f);
                                                                  // Search the whole phrase - CONTAINS
            predicate = predicate.Or(x => x.ShortDescription.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.LongDescription.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.PostedDate.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.Category.Contains(searchText));
            //Where Author="searchText" OR Description="searchText" OR Title="searchText"
            return predicate;
        }






    }
}