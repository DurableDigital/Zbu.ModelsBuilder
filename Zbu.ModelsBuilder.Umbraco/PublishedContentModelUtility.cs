﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Zbu.ModelsBuilder.Umbraco
{
    public static class PublishedContentModelUtility
    {
        // looks safer but probably useless... ppl should not call these methods directly
        // and if they do... they have to take care about not doing stupid things

        //public static PublishedPropertyType GetModelPropertyType2<T>(Expression<Func<T, object>> selector)
        //    where T : PublishedContentModel
        //{
        //    var type = typeof (T);
        //    var s1 = type.GetField("ModelTypeAlias", BindingFlags.Public | BindingFlags.Static);
        //    var alias = (s1.IsLiteral && s1.IsInitOnly && s1.FieldType == typeof(string)) ? (string)s1.GetValue(null) : null;
        //    var s2 = type.GetField("ModelItemType", BindingFlags.Public | BindingFlags.Static);
        //    var itemType = (s2.IsLiteral && s2.IsInitOnly && s2.FieldType == typeof(PublishedItemType)) ? (PublishedItemType)s2.GetValue(null) : 0;

        //    var contentType = PublishedContentType.Get(itemType, alias);
        //    // etc...
        //}

        public static PublishedPropertyType GetModelPropertyType<TModel, TValue>(PublishedContentType contentType, Expression<Func<TModel, TValue>> selector)
            where TModel : PublishedContentModel
        {
            var expr = selector.Body as MemberExpression;
            
            if (expr == null)
                throw new ArgumentException("Not a property expression.", "selector");

            // there _is_ a risk that contentType and T do not match
            // see note above : accepted risk...

            var attr = expr.Member
                .GetCustomAttributes(typeof (ImplementPropertyTypeAttribute), false)
                .OfType<ImplementPropertyTypeAttribute>()
                .SingleOrDefault();

            if (attr == null || string.IsNullOrWhiteSpace(attr.Alias))
                throw new InvalidOperationException(
                    string.Format("Could not figure out property alias for property \"{0}\".", expr.Member.Name));

            return contentType.GetPropertyType(attr.Alias);
        }
    }
}
