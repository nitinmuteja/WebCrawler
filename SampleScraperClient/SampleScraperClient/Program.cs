using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScrapySharp.Core;
using ScrapySharp.Html.Parsing;
using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Forms;
using System.Net;

namespace SampleScraperClient
{

    /// <summary>
    /// Website with the loaded page and the navigation links
    /// </summary>
    public class WebSite {

        private WebPage _parentPage;
        public WebPage ParentPage { get { return _parentPage; } set{  var listUrls = value.Html.SelectNodes("//a[@href and not(contains(@href,\"#\"))]").ToList();
                LinkedPages = new Stack<HtmlNode>(listUrls); ; _parentPage = value; } }
        private Stack<HtmlNode> LinkedPages;

        public HtmlNode popLink()
        {
            if (LinkedPages == null || LinkedPages.Count == 0)
            {
                return null;
            }
            else
                return LinkedPages.Pop();

        }

    }
    /// <summary>
    /// This program starts navigating a website and then proceed with the first link found for navigation
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //disable certificate verification
             ServicePointManager
            .ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) => true;
            // setup the browser
            ScrapingBrowser browser = new ScrapingBrowser();
            browser.AllowAutoRedirect = true; // Browser has many settings you can access in setup
            browser.AllowMetaRedirect = true;
            browser.AutoDownloadPagesResources = true;

            //go to the home page
            WebSite website = new WebSite();
            WebPage parentPage = browser.NavigateToPage(new Uri("http://nitinmuteja.com/"));
            // get first piece of data, the page title
            website.ParentPage = parentPage;
            string url="";
            HtmlNode linkNode;
            do
            {
                //pop links and proceed navigation to the first link
                linkNode=website.popLink();
                if (linkNode != null)
                {
                    Uri newuri = null;
                    url = linkNode.GetAttributeValue("href", string.Empty);
                    if(!string.IsNullOrWhiteSpace(url))
                    {
                        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            newuri = new Uri(website.ParentPage.AbsoluteUrl, url);
                        }
                        else
                        {
                            newuri = new Uri(url);
                        }

                        if (newuri.AbsolutePath != website.ParentPage.AbsoluteUrl.AbsolutePath)
                        {
                            try
                            {
                               website.ParentPage = browser.NavigateToPage(newuri);
                            }
                            catch (Exception ex)
                            {
                                //log exception
                            }
                        }
                    }
                }

            } while (linkNode != null);

            ////Scraping webpages and submiting forms for reference purpose
           // WebPage PageResult = Browser.NavigateToPage(new Uri("http://localhost:51621/"));
           // HtmlNode TitleNode = PageResult.Html.CssSelect(".navbar-brand").First();
           // string PageTitle = TitleNode.InnerText;
           // //get a list of data from a table
           // List<String> Names = new List<string>();
           // var Table = PageResult.Html.CssSelect("#PersonTable").First();
           // foreach (var row in Table.SelectNodes("tbody/tr"))
           // {
           //     foreach (var cell in row.SelectNodes("td[1]"))
           //     {
           //         Names.Add(cell.InnerText);
           //     }
           // }
           //// find a form and send back data
           //PageWebForm form = PageResult.FindFormById("dataForm");
           // // assign values to the form fields
           // form["UserName"] = "AJSON";
           // form["Gender"] = "M";
           // form.Method = HttpVerb.Post;
           // WebPage resultsPage = form.Submit();
        }
    }
}
