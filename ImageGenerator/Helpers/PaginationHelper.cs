using System.Text.Json;
using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGenerator.Helpers;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
/// <typeparam name="U">The type of the DTO.</typeparam>
public class PagedList<T,U> where T: ModelBase where U: ActionBaseDto
{
    /// <summary>
    /// The current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The total number of items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The list of items on the current page.
    /// </summary>
    public List<U> Items { get; set; } = [];

    /// <summary>
    /// Creates a new paginated list from a queryable source.
    /// </summary>
    /// <param name="source">The queryable source.</param>
    /// <param name="param">The pagination parameters.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <returns>A new paginated list.</returns>
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

/// <summary>
/// A helper class for adding pagination headers to HTTP responses.
/// </summary>
public static class PaginationHeaderHelper
{
    /// <summary>
    /// Adds a pagination header to the response.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="U">The type of the DTO.</typeparam>
    /// <param name="headers">The response headers.</param>
    /// <param name="pagedList">The paginated list.</param>
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