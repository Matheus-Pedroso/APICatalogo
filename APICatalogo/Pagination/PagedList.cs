﻿namespace APICatalogo.Pagination;

public class PagedList<T> : List<T> where T : class
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set;}
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public bool hasPrevious => CurrentPage > 1;
    public bool hasNext => CurrentPage < TotalCount;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count; // total de elementos
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    



}
