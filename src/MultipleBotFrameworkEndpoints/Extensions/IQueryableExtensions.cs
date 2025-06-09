/// <summary>
/// Расширения для работы с IQueryable.
/// Предоставляет методы для условной фильтрации и сортировки запросов.
/// </summary>

using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace MultipleBotFrameworkEndpoints.Extensions;

public static class IQueryableExtensions
{
    /// <summary>
    /// Применяет условие фильтрации к запросу только если указанное условие истинно
    /// </summary>
    /// <typeparam name="TSource">Тип элементов в запросе</typeparam>
    /// <param name="source">Исходный запрос</param>
    /// <param name="condition">Условие применения фильтра</param>
    /// <param name="predicate">Предикат фильтрации</param>
    /// <returns>Отфильтрованный запрос или исходный запрос, если условие ложно</returns>
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, bool>> predicate)
    {
        if (condition)
            return source.Where(predicate);
        else
            return source;
    }
    
    /// <summary>
    /// Применяет условную трансформацию к запросу
    /// </summary>
    /// <typeparam name="T">Тип элементов в запросе</typeparam>
    /// <param name="query">Исходный запрос</param>
    /// <param name="condition">Условие применения трансформации</param>
    /// <param name="whenTrue">Функция трансформации при истинном условии</param>
    /// <param name="whenFalse">Опциональная функция трансформации при ложном условии</param>
    /// <returns>Трансформированный запрос</returns>
    public static IQueryable<T> When<T>(this IQueryable<T> query, bool condition,
        Func<IQueryable<T>, IQueryable<T>> whenTrue, 
        Func<IQueryable<T>, IQueryable<T>>? whenFalse = null)
    {
        if (condition)
        {
            query = whenTrue.Invoke(query);
        }
        else if(whenFalse is not null)
        {
            query = whenFalse.Invoke(query);
        }

        return query;
    }

    /// <summary>
    /// Применяет сортировку к запросу на основе строки с параметрами сортировки
    /// </summary>
    /// <typeparam name="T">Тип элементов в запросе</typeparam>
    /// <param name="source">Исходный запрос</param>
    /// <param name="order">Строка с параметрами сортировки в формате "+PropertyName1,-PropertyName2,+PropertyName3"</param>
    /// <returns>Отсортированный запрос</returns>
    /// <exception cref="Exception">Выбрасывается при неверном формате строки сортировки</exception>
    public static IQueryable<T> Order<T>(this IQueryable<T> source, string? order)
    {
        if (string.IsNullOrEmpty(order)) return source;
        
        Regex regex = new(@"[+-]?\w+");

        MatchCollection matches = regex.Matches(order);

        if (matches is null || matches.Any() == false) throw new Exception("Неверный формат строки сортировки!");

        List<SortParam> sortParams = new();
        foreach (Match match in matches)
        {
            string propertyName = "";
            SortParam param = new();
            if (match.Value.StartsWith("+"))
            {
                param.IsAscending = true;
                param.PropertyName = match.Value.Substring(1);
            }
            else if (match.Value.StartsWith("-"))
            {
                param.IsAscending = false;
                param.PropertyName = match.Value.Substring(1);
            }
            else
            {
                param.IsAscending = true;
                param.PropertyName = match.Value;
            }
            sortParams.Add(param);
        }

        return Order(source, sortParams);
    }
    
    /// <summary>
    /// Применяет сортировку к запросу на основе списка параметров сортировки
    /// </summary>
    /// <typeparam name="T">Тип элементов в запросе</typeparam>
    /// <param name="source">Исходный запрос</param>
    /// <param name="sorting">Список параметров сортировки</param>
    /// <returns>Отсортированный запрос</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается при null запросе</exception>
    /// <exception cref="ArgumentException">Выбрасывается при пустом списке параметров сортировки</exception>
    public static IQueryable<T> Order<T>(this IQueryable<T> source, List<SortParam> sorting)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (sorting.Count == 0)
            throw new ArgumentException("Сортировка не может быть пустой", nameof(sorting));

        for (var i = 0; i < sorting.Count; i++)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(param, sorting[i].PropertyName);
            var lambda = Expression.Lambda(property, param);

            var methodName = sorting[i].IsAscending 
                ? (i == 0 ? "OrderBy" : "ThenBy")
                : (i == 0 ? "OrderByDescending" : "ThenByDescending");

            var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { source.ElementType, property.Type },
                source.Expression, Expression.Quote(lambda));

            source = source.Provider.CreateQuery<T>(resultExpression);
        }

        return source;
    }
    
    /// <summary>
    /// Параметры сортировки для запроса
    /// </summary>
    public class SortParam
    {
        /// <summary>
        /// Наименование поля для сортировки
        /// </summary>
        public string PropertyName { get; set; } = null!;

        /// <summary>
        /// По возрастанию (true) или по убыванию (false)
        /// </summary>
        public bool IsAscending { get; set; } = true;
    }
}