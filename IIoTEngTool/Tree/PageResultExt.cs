using System;
using System.Linq;
using IIoTEngTool.Data;


namespace IIoTEngTool.Data
{
    public static class PagedResultExtensions
    {
        public static PagedResult<T> GetPaged<T>(this PagedResult<T> query, int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Results.Count
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Results.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}