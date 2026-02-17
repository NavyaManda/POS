namespace MenuService.API.Models;

public class MenuItemRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int Calories { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsSpicy { get; set; }
}

public class MenuItemResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public int Calories { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsSpicy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class CategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
