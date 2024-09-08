using Microsoft.EntityFrameworkCore;

namespace MultipleTestBot.Models;

public class PagedList<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public List<T> Data { get; set; } = new List<T>();

    public PagedList() { }
    
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Data.AddRange(items);
    }
    
    public PagedList(List<T> items, int count, Pagination pagination) : this(items, count, pagination.PageNumber, pagination.PageSize)
    {
    }

    public async static Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
    
    public static Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, Pagination pagination)
    {
        return ToPagedListAsync(source, pagination.PageNumber, pagination.PageSize);
    }
}
