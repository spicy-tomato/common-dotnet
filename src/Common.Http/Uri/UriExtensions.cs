﻿using System;
using System.Collections.Specialized;
using System.Web;

namespace Utconnect.Common.Http.Uri
{
    /// <summary>
    /// Provides extension methods for <see cref="System.Uri"/> objects.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Adds the specified parameter to the Query String.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName">Name of the parameter to add.</param>
        /// <param name="paramValue">Value for the parameter to add.</param>
        /// <returns>Url with added parameter.</returns>
        public static System.Uri AddParameter(this System.Uri url, string paramName, string paramValue)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
    }
}