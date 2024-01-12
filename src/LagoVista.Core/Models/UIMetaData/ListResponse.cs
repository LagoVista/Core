﻿using LagoVista.Core.Attributes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Models.UIMetaData
{
    public interface IListResponse
    {
        string Title { get; }
        string ModelName { get; }
        string Help { get; }
        List<ListColumn> Columns { get; }
        List<string> ColumnFilters { get; }

        string Icon { get; set; }
        int PageSize { get; }
        int RecordCount { get; }
        int PageIndex { get; }
        int PageCount { get; }
        string NextPartitionKey { get; }
        string NextRowKey { get; }
        bool HasMoreRecords { get; }
        string FactoryUrl { get; }
        string DeleteUrl { get; }
        string GetUrl { get; }
        string GetListUrl { get; }
        string HelpUrl { get; set; }
    }

    public class ListResponse<TModel> : InvokeResult, IListResponse where TModel : class
    {
        public string Title { get; set; }
        public string ModelName { get; set; }
        public string Help { get; set; }

        public List<ListColumn> Columns { get; set; }

        public List<string> ColumnFilters { get; set; }

        public IEnumerable<TModel> Model { get; set; }

        public int RecordCount { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public string Icon { get; set; }
        public string NextPartitionKey { get; set; }
        public string NextRowKey { get; set; }
        public bool HasMoreRecords { get; set; }

        public string FactoryUrl { get; set; }
        public string GetUrl { get; set; }
        public string GetListUrl { get; set; }

        public string DeleteUrl { get; set; }

        public string HelpUrl { get; set; }

        public static new ListResponse<TModel> FromError(string errorMessage, string errorCode = "")
        {
            var response = new ListResponse<TModel>();
            response.Errors.Add(new ErrorMessage(errorCode, errorMessage));
            return response;
        }

        public static new ListResponse<TModel> FromErrors(params ErrorMessage[] errMessages)
        {
            var response = new ListResponse<TModel>();
            response.Errors.Concat(errMessages);
            return response;
        }

        public static ListResponse<TModel> Create(IEnumerable<TModel> model, IListResponse original)
        {
            var response = Create(model);
            response.HasMoreRecords = original.HasMoreRecords;
            response.NextRowKey = original.NextRowKey;
            response.PageSize = original.PageSize;
            response.PageCount = original.PageCount;
            response.PageIndex = original.PageIndex;
            return response;
        }

        public static ListResponse<TModel> Create(IEnumerable<TModel> model)
        {
            var response = new ListResponse<TModel>();
            /* Make sure the enumeration is populated before sending to the client */
            response.Model = model.ToList();
            response.ModelName = typeof(TModel).Name;

            var attr = typeof(TModel).GetTypeInfo().GetCustomAttribute<EntityDescriptionAttribute>();

            if (attr != null)
            {
                var titleProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.TitleResource);
                response.Title = titleProperty != null ? titleProperty.GetValue(titleProperty.DeclaringType, null) as String : typeof(TModel).Name;
                response.GetUrl = attr.GetUrl;
                response.FactoryUrl = attr.FactoryUrl;
                response.GetListUrl = attr.GetListUrl;
                response.DeleteUrl = attr.DeleteUrl;
                response.GetUrl = attr.GetUrl;
                response.HelpUrl = attr.HelpUrl;
                response.Icon = attr.Icon;

                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.UserHelpResource);
                if (helpProperty != null)
                {
                    response.Help = helpProperty.GetValue(helpProperty.DeclaringType, null) as String;
                }
            }
            else
            {
                response.Title = typeof(TModel).Name;
            }

            var columns = new List<ListColumn>();
            var properties = typeof(TModel).GetRuntimeProperties();
            foreach (var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<ListColumnAttribute>();
                if (fieldAttributes.Any())
                {
                    columns.Add(ListColumn.Create(property.Name.ToLower(), fieldAttributes.First()));
                }
            }

            response.Columns = columns;
            response.RecordCount = model.Count();
            return response;
        }
        
        public static ListResponse<TModel> Create(ListRequest request, IEnumerable<TModel> model)
        {
            var response = ListResponse<TModel>.Create(model);
            response.HasMoreRecords = request.PageSize == model.Count();
            response.PageIndex = request.PageIndex;
            response.PageSize = request.PageSize;
            response.GetListUrl = request.Url;
            return response;
        }

        public override string ToString()
        {
            return $@"[List Response]
\tTitle            : {Title}
\tGetList Url      : {GetListUrl}
\tFactory Url      : {FactoryUrl}
\tPage Count       : {PageCount}
\tRecord Count     : {RecordCount}
\tPage Index       : {PageIndex}
\tPage Size        : {PageSize}
\tNext parition key: {NextPartitionKey}
\tNext row key     : {NextRowKey}
\tHas More Records : {PageCount}";
        }
    }

    public static class ListResponseExtensions
    {
        /// <summary>
        /// Override to create a model from an existing list response (used to transform
        /// between collection types while maintaining parameters such as page size, page count etc...).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ListResponse<TModel> Create<TModel>(this IListResponse source, IEnumerable<TModel> items) where TModel : class
        {
            return new ListResponse<TModel>()
            {
                Model = items,
                Columns = source.Columns,

                Help = source.Help,
                Title = source.Title,
                FactoryUrl = source.FactoryUrl,
                GetListUrl = source.GetListUrl,
                DeleteUrl = source.DeleteUrl,
                HelpUrl = source.HelpUrl,
                GetUrl = source.GetUrl,
                PageCount = source.PageCount,
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                RecordCount = source.RecordCount,
                NextPartitionKey = source.NextPartitionKey,
                NextRowKey = source.NextRowKey,
                HasMoreRecords = source.HasMoreRecords,
            };
        }
    }

}
