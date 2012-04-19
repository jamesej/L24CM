using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Specialized;
using L24CM.Models;

namespace L24CM.Utility
{
    public static class LinqX
    {
        /// <summary>
        /// If a list is nonempty, passes it though unchanged.  If it is empty, substitutes it with another list.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="subst"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> IfEmpty<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> subst)
        {
            bool isEmpty = true;
            foreach (TSource element in source)
            {
                isEmpty = false;
                yield return element;
            }
            if (isEmpty)
                foreach (TSource element in subst)
                    yield return element;
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult ifEmpty)
        {
            if (source.Any<TSource>())
                return source.Max(selector);
            else
                return ifEmpty;
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            if (source != null)
                foreach (TSource element in source)
                    yield return element;
            // don't append item if it's default for the type TSource
            if (!EqualityComparer<TSource>.Default.Equals(item,default(TSource)))
                yield return item;
        }

        private static Expression<Func<TElement, bool>> GetWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();
            if (values == null || !values.Any())
                return e => false;

            var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        /// <summary> 
        /// Return the element that the specified property's value is contained in the specific values 
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }

        /// <summary> 
        /// Return the element that the specified property's value is contained in the specific values 
        /// </summary> 
        /// <typeparam name="TElement">The type of the element.</typeparam> 
        /// <typeparam name="TValue">The type of the values.</typeparam> 
        /// <param name="source">The source.</param> 
        /// <param name="propertySelector">The property to be tested.</param> 
        /// <param name="values">The accepted values of the property.</param> 
        /// <returns>The accepted elements.</returns> 
        public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return source.Where(GetWhereInExpression(propertySelector, values));
        }

        public static void Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (T item in source)
            {
                if (first)
                    first = false;
                else
                    sb.Append(separator);
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> ToKeyValues(this NameValueCollection nvc)
        {
            return nvc.Cast<string>().SelectMany(key => nvc[key].Split(',').Select(val => new KeyValuePair<string, string>(key, val)));
        }

        public static TValue FirstSelectOrDefault<TElement, TValue>(this IEnumerable<TElement> source, Func<TElement, bool> test, Func<TElement, TValue> selector)
        {
            TElement el = source.FirstOrDefault(test);
            if (el == null || el.Equals(default(TElement)))
                return default(TValue);
            else
                return selector(el);
        }
        public static TValue FirstSelectOrDefault<TElement, TValue>(this IQueryable<TElement> source, Func<TElement, bool> test, Func<TElement, TValue> selector)
        {
            TElement el = source.FirstOrDefault(test);
            if (el.Equals(default(TElement)))
                return default(TValue);
            else
                return selector(el);
        }


        public static IEnumerable<TSource> PartialOrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return PartialOrderBy(source, keySelector, null);
        }

        public static IEnumerable<TSource> PartialOrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)   // the '0' value of this comparer is interpreted as 'don't know relationship'
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (comparer == null) comparer = (IComparer<TKey>)Comparer<TKey>.Default;

            return PartialOrderByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> PartialOrderByIterator<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            var values = source.ToArray();
            var keys = values.Select(keySelector).ToArray();
            int count = values.Length;
            var notYieldedIndexes = System.Linq.Enumerable.Range(0, count).ToArray();
            int valuesToGo = count;

            while (valuesToGo > 0)
            {
                //Start with first value not yielded yet
                int minIndex = notYieldedIndexes.First(i => i >= 0);

                //Find minimum value amongst the values not yielded yet
                for (int i = 0; i < count; i++)
                    if (notYieldedIndexes[i] >= 0)
                        if (comparer.Compare(keys[i], keys[minIndex]) < 0)
                        {
                            minIndex = i;
                        }

                //Yield minimum value and mark it as yielded
                yield return values[minIndex];
                notYieldedIndexes[minIndex] = -1;
                valuesToGo--;
            }
        }

    }
}
