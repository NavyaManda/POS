namespace AuthService.API.Models
{
    /// <summary>
    /// Generic API response wrapper
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public string TraceId { get; set; }
    }

    /// <summary>
    /// Generic API response wrapper with data
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public string TraceId { get; set; }
    }

    /// <summary>
    /// Paginated response
    /// </summary>
    public class PaginatedResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
