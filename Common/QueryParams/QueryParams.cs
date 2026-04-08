using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.QueryParams
{
    public class QueryParams
    {
        private const int MaxPageSize = 100;

        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        // Search
        public string? Keyword { get; set; }

        // Sort
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
