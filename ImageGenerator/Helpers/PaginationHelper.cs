using System.Text.Json;
using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Helpers;

public class PagedList<T,U> where T: ModelBase where U: ActionBaseDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public List<U> Items { get; set; } = [];
    public static async Task<PagedList<T,U>> CreateAsync(IQueryable<T> source, PaginationBaseDto param, IMapper mapper) 
    {
        var totalCount = await source.CountAsync();

        var items = await source
            .Skip(param.PageNumber * param.PageSize)
            .Take(param.PageSize)
            .ToListAsync();

        return new PagedList<T,U>
        {
            PageNumber = param.PageNumber,
            PageSize = param.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)param.PageSize),
            Items = mapper.Map<List<U>>(items)
        };
    }
}

public static class PaginationHeaderHelper
{
    public static void AddPaginationHeader<T, U>(this IHeaderDictionary headers, PagedList<T, U> pagedList) where T: ModelBase where U: ActionBaseDto
    {
        headers.Append("X-Pagination", JsonSerializer.Serialize(new
        {
            pagedList.TotalCount,
            pagedList.PageSize,
            pagedList.PageNumber,
            pagedList.TotalPages
        }));
    }
}