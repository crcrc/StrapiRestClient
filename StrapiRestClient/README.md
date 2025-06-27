# StrapiRestClient Usage Guide

StrapiRestClient is specifically designed for **Strapi v5** to get content via the REST API and provides support for the new populate syntax.

This guide provides instructions and examples on how to implement and use the `StrapiRestClient` library in your .NET applications.

## 1. Installation

First, add the `StrapiRestClient` NuGet package to your project:

```bash
dotnet add package StrapiRestClient
```

## 2. Configuration

### a. `appsettings.json`

Add your Strapi API base URL and optional API key to your `appsettings.json` file:

```json
{
  "StrapiConnect": {
    "BaseUrl": "http://localhost:1337/api",
    "ApiKey": "your-strapi-api-key-if-any"
  }
}
```

### b. Dependency Injection (`Program.cs` or `Startup.cs`)

Register the `StrapiRestClient` in your `Program.cs` (for .NET 6+ minimal APIs) or `Startup.cs` (for older .NET versions) using the provided extension method:

```csharp
// Program.cs
using StrapiRestClient.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add StrapiRestClient
builder.Services.AddStrapiConnect(builder.Configuration);

// ... other services

var app = builder.Build();

// ...

app.Run();
```

## 3. Usage Examples

Once configured, you can inject `IStrapiRestClient` into your services or controllers and use it to interact with your Strapi API.

### a. Defining Your Models

Before making requests, define C# classes that match the structure of your Strapi v5 content types. **Strapi v5** uses a flatter structure with `documentId` instead of nested attributes:

```csharp
public class Article
{
    public string? DocumentId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public Category? Category { get; set; }  // Populated relation
    public Author? Author { get; set; }      // Populated relation
}

public class Category
{
    public int Id { get; set; }
    public string? DocumentId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
}
```

#### 💡 **Model Generation Tip**
Use the new `GetRawJsonAsync` method to get actual JSON responses for easy model generation:

```csharp
// Get raw JSON for model generation
var request = StrapiRequest.Get("articles").WithPopulate("category").WithPopulate("author");
string rawJson = await _strapiRestClient.GetRawJsonAsync(request);
Console.WriteLine(rawJson);
```


### b. Making GET Requests

#### Get All Entries

```csharp
using StrapiRestClient.Request;
using StrapiRestClient.RestClient;
using StrapiRestClient.Models;

public class MyService
{
    private readonly IStrapiRestClient _strapiRestClient;

    public MyService(IStrapiRestClient strapiRestClient)
    {
        _strapiRestClient = strapiRestClient;
    }

    public async Task<List<Article>?> GetArticlesAsync()
    {
        var request = StrapiRequest.Get("articles");
        var response = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request);

        if (response.IsSuccess)
        {
            return response.Data?.Data?.ToList();
        }
        else
        {
            // Handle error: response.Error, response.StatusCode
            Console.WriteLine($"Error: {response.Error?.Message} (Status: {response.StatusCode})");
            return null;
        }
    }
}
```

#### Get Entry by Document ID (Strapi v5)

```csharp
var request = StrapiRequest.Get("articles", "/abc123def456"); // Get article with documentId 'abc123def456'
var response = await _strapiRestClient.ExecuteAsync<StrapiResponse<Article>>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Article Title: {response.Data?.Data?.Title}");
}
```

#### Get Entry by Slug

```csharp
var request = StrapiRequest.Get("articles", "/my-first-article"); // Get article with slug 'my-first-article'
var response = await _strapiRestClient.ExecuteAsync<StrapiResponse<Article>>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Article Title: {response.Data?.Data?.Title}");
}
```

#### Debug URL Generation

Use the new `GetQueryUrl` method to see exactly what URL will be generated:

```csharp
var request = StrapiRequest.Get("articles")
    .WithPopulate("category")
    .WithPopulate("author");

string url = request.GetQueryUrl("http://localhost:1337/api");
Console.WriteLine(url);
// Output: http://localhost:1337/api/articles?populate[0]=category&populate[1]=author
```

#### Filtering Data

Use the fluent `WithFilter` method to apply various filters. You can chain multiple filters.

```csharp
using StrapiRestClient.Enums;

// Get articles with title 'My Awesome Article'
var request = StrapiRequest.Get("articles")
                           .WithFilter(FilterType.EqualTo, "title", "My Awesome Article");
var response = await _strapiRestClient.ExecuteAsync<List<Article>>(request);

// Get articles published after a certain date and containing 'C#' in description
var request2 = StrapiRequest.Get("articles")
                            .WithFilter(FilterType.GreaterThan, "publishedAt", "2023-01-01T00:00:00.000Z")
                            .WithFilter(FilterType.Contains, "description", "C#");
var response2 = await _strapiRestClient.ExecuteAsync<List<Article>>(request2);

// Get articles where ID is in a list
var request3 = StrapiRequest.Get("articles")
                            .WithFilter(FilterType.In, "id", "1")
                            .WithFilter(FilterType.In, "id", "3")
                            .WithFilter(FilterType.In, "id", "5");
var response3 = await _strapiRestClient.ExecuteAsync<List<Article>>(request3);
```

#### Sorting Data

Use `WithSort` to order your results. You can specify multiple sort fields.

```csharp
// Sort articles by title ascending
var request = StrapiRequest.Get("articles")
                           .WithSort("title", SortDirection.Ascending);
var response = await _strapiRestClient.ExecuteAsync<List<Article>>(request);

// Sort articles by published date descending, then by title ascending
var request2 = StrapiRequest.Get("articles")
                            .WithSort("publishedAt", SortDirection.Descending)
                            .WithSort("title", SortDirection.Ascending);
var response2 = await _strapiRestClient.ExecuteAsync<List<Article>>(request2);
```

#### Pagination

Use `WithPage` and `WithPageSize` for pagination.

```csharp
// Get the second page of articles, with 10 articles per page
var request = StrapiRequest.Get("articles")
                           .WithPage(2)
                           .WithPageSize(10);
var response = await _strapiRestClient.ExecuteAsync<List<Article>>(request);
```

#### Populating Relations (Strapi v5 Syntax)

Use `WithPopulate` to eager-load related data. **This library automatically uses the correct Strapi v5 array-style populate syntax.**

```csharp
// Populate multiple relations - generates: populate[0]=category&populate[1]=author
var request = StrapiRequest.Get("articles")
                           .WithPopulate("category")
                           .WithPopulate("author");
var response = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request);

// Populate nested relations
var request2 = StrapiRequest.Get("articles")
                            .WithPopulate("category.author");
var response2 = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request2);

// Populate all relations (1 level deep)
var request3 = StrapiRequest.Get("articles")
                            .WithPopulateAll();
var response3 = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request3);

// Populate specific fields within a relation - generates: populate[category][fields]=name,slug
var request4 = StrapiRequest.Get("articles")
                            .WithPopulateFields("category", "name", "slug");
var response4 = await _strapiRestClient.ExecuteAsync<StrapiCollectionResponse<ICollection<Article>>>(request4);
```

#### ⚡ **Strapi v5 Populate Syntax**
This library automatically generates the correct Strapi v5 populate URLs:
- **Multiple relations**: `populate[0]=category&populate[1]=author`
- **Single relation**: `populate[0]=category`
- **With fields**: `populate[category][fields]=name,slug`
- **All relations**: `populate=*`

### c. Making POST Requests

For `POST` requests, provide a C# object as the body. The client will automatically serialize it to JSON.

```csharp
public class NewArticleData
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
}

// ... inside your service/controller

var newArticle = new NewArticleData
{
    Title = "My New Article",
    Description = "This is a new article created via the API.",
    Slug = "my-new-article"
};

var request = StrapiRequest.Post("articles", newArticle);
var response = await _strapiRestClient.ExecuteAsync<Article>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Successfully created article with ID: {response.Data?.Id}");
}
else
{
    Console.WriteLine($"Error creating article: {response.Error?.Message}");
}
```

### d. Making PUT Requests

Similar to `POST`, provide the updated C# object as the body.

```csharp
public class UpdateArticleData
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}

// ... inside your service/controller

var updatedArticleData = new UpdateArticleData
{
    Title = "Updated Article Title",
    Description = "This article has been updated."
};

var request = StrapiRequest.Put("articles", updatedArticleData, "/1"); // Update article with ID 1
var response = await _strapiRestClient.ExecuteAsync<Article>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Successfully updated article with ID: {response.Data?.Id}");
}
else
{
    Console.WriteLine($"Error updating article: {response.Error?.Message}");
}
```

### e. Making DELETE Requests

```csharp
var request = StrapiRequest.Delete("articles", "/1"); // Delete article with ID 1
var response = await _strapiRestClient.ExecuteAsync<object>(request); // Response data type can be 'object' or 'dynamic'

if (response.IsSuccess)
{
    Console.WriteLine("Article deleted successfully.");
}
else
{
    Console.WriteLine($"Error deleting article: {response.Error?.Message}");
}
```

## 4. Model Generation with GetRawJsonAsync

The library includes a `GetRawJsonAsync` method specifically designed to help with model generation. This method returns the raw JSON response from Strapi, which you can then use with Visual Studio's "Paste JSON as Classes" feature.

### Basic Usage

```csharp
// Get raw JSON for basic articles
var request = StrapiRequest.Get("articles");
string rawJson = await _strapiRestClient.GetRawJsonAsync(request);
Console.WriteLine(rawJson);
// Output: {"data":[{"documentId":"abc123","title":"Sample Article",...}],"meta":{...}}
```

### With Relations Populated

```csharp
// Get raw JSON with populated relations for complete model structure
var request = StrapiRequest.Get("articles")
    .WithPopulate("category")
    .WithPopulate("author")
    .WithPopulate("tags");

string rawJson = await _strapiRestClient.GetRawJsonAsync(request);
// This gives you the complete structure including all relation objects
```

### Model Generation Workflow

1. **Create the request** with all the relations you need populated
2. **Get raw JSON** using `GetRawJsonAsync`
3. **Copy the JSON response**
4. **In Visual Studio**: Edit → Paste Special → Paste JSON as Classes
5. **Rename the generated classes** to match your needs

### Example Workflow

```csharp
// Step 1: Create comprehensive request
var request = StrapiRequest.Get("articles")
    .WithPopulate("category")
    .WithPopulate("author")
    .WithPopulate("featuredImage")
    .WithPage(1)
    .WithPageSize(1); // Get just one item for model structure

// Step 2: Get raw JSON
string rawJson = await _strapiRestClient.GetRawJsonAsync(request);

// Step 3: Copy rawJson and use "Paste JSON as Classes" in Visual Studio
// Step 4: Clean up generated class names and properties as needed
```

This approach ensures your C# models match your exact Strapi v5 content structure, including all populated relations.

## 5. Error Handling

The `ExecuteAsync` method returns a `StrapiResponse<T>` object, which provides comprehensive information about the API call's outcome.

-   **`response.IsSuccess`**: A boolean indicating if the HTTP status code was in the 2xx range.
-   **`response.Data`**: Contains the deserialized data if `IsSuccess` is `true`. Otherwise, it will be `null`.
-   **`response.Error`**: Contains detailed error information (status, name, message, details) if `IsSuccess` is `false`. Otherwise, it will be `null`.
-   **`response.StatusCode`**: The raw HTTP status code returned by the Strapi API.

Always check `response.IsSuccess` before attempting to access `response.Data`.

```csharp
var response = await _strapiRestClient.ExecuteAsync<Article>(someRequest);

if (response.IsSuccess)
{
    // Process successful data
    var article = response.Data;
}
else
{
    // Handle API error
    Console.WriteLine($"API Error: {response.Error?.Name} - {response.Error?.Message}");
    Console.WriteLine($"Status Code: {response.StatusCode}");
    // You can also inspect response.Error.Details for more specific error information
}
```

## 6. What's New in v2.0 - Strapi v5 Support

### 🚀 **Strapi v5 Compatibility**
- **Correct populate syntax**: Automatically generates `populate[0]=category&populate[1]=author` format
- **Document Service API support**: Works with Strapi v5's new document-based structure  
- **Flat response structure**: Handles the new flatter JSON response format

### 🛠️ **New Methods**
- **`GetRawJsonAsync()`**: Get raw JSON responses for model generation
- **`GetQueryUrl()`**: Debug and inspect generated URLs  

### 🔧 **Enhanced Testing**
- **Comprehensive unit tests**: 12+ new test cases covering all scenarios
- **Mock support**: Full Moq integration for testing
- **Integration tests**: Real Strapi v5 API testing

### 📦 **Updated Dependencies**
- **.NET 9.0 support**: Latest .NET framework compatibility
- **Enhanced error handling**: Better exception management
- **Improved logging**: More detailed debug information

### 🔄 **Migration from v1.x**
If you're upgrading from a previous version:

1. **Update your models** to use the flatter Strapi v5 structure (no nested `attributes`)
2. **Update response types** to use `StrapiCollectionResponse<T>` for collections
3. **Use `documentId`** instead of `id` for document references
4. **Test your populate calls** - they now use the correct v5 syntax automatically

### 🎯 **Performance Improvements**
- **Reduced memory allocation**: More efficient JSON handling
- **Better connection management**: Enhanced HTTP client usage
- **Streamlined URL building**: Optimized query parameter generation

This guide should help you get started with the `StrapiRestClient` library for Strapi v5. For more advanced scenarios, refer to the XML documentation comments within the library's code.