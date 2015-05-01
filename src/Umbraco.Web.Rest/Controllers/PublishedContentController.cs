﻿using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Web.Rest.Links;
using Umbraco.Web.Rest.Models;

namespace Umbraco.Web.Rest.Controllers
{
    /// <summary>
    /// REST service for querying against Published content
    /// </summary>    
    public class PublishedContentController : UmbracoHalController<int, IPublishedContent>
    {
        //TODO: We need to make a way to return IPublishedContent from either the cache or from Examine, then convert that to the output
        // this controller needs to support both data sources in one way or another - either base classes, etc...

        /// <summary>
        /// Default ctor
        /// </summary>
        public PublishedContentController()
        {
        }

        /// <summary>
        /// All dependencies
        /// </summary>
        /// <param name="umbracoContext"></param>
        /// <param name="umbracoHelper"></param>
        public PublishedContentController(
            UmbracoContext umbracoContext,
            UmbracoHelper umbracoHelper)
            : base(umbracoContext, umbracoHelper)
        {
        }

        protected override IEnumerable<IPublishedContent> GetRootContent()
        {
            return Umbraco.TypedContentAtRoot();
        }

        protected override ContentMetadataRepresentation GetMetadataForItem(int id)
        {
            var found = Umbraco.TypedContent(id);
            if (found == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            var result = new ContentMetadataRepresentation(LinkTemplate, id)
            {
                Fields = GetDefaultFieldMetaData(),
                // NOTE: we cannot determine this from IPublishedContent
                Properties = null, 
                // NOTE: null because IPublishedContent is readonly
                CreateTemplate = null
            };
            return result;
        }

        protected override IPublishedContent GetItem(int id)
        {
            return Umbraco.TypedContent(id);
        }

        protected override IEnumerable<IPublishedContent> GetChildContent(int id)
        {
            var content = Umbraco.TypedContent(id);
            if (content == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return content.Children;
        }

        protected override IContentLinkTemplate LinkTemplate
        {
            get { return new PublishedContentLinkTemplate(CurrentVersionRequest); }
        }
    }
}