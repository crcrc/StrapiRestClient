# C# .NET StrapiRestClient Usage Guide

StrapiRestClient is a C# .NET client specifically designed for **Strapi v5** to get content via the REST API from your Strapi instance.

This guide provides instructions and examples on how to implement and use the `StrapiRestClient` library in your .NET applications.

## Running the tests

To run the tests, in the "strapi-test-instance" is a demo instance with sqlite database populated with data

- npm install
- npm run dev

Strapi should be available on http://localhost:1337

- email: test@test.com
- password: Test1234

See the StrapiRestClient.Tests/StrapiRestClientTests.cs file for guidance on how to make requests to the Strapi API.


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
  "StrapiRestClient": {
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
services.AddStrapiRestClient(configuration);

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
        var request = new StrapiQueryRequest("articles");

        var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);
    }
}
```

#### Get Entry by Document ID (Strapi v5)

```csharp
var request = new StrapiQueryRequest("articles")
                   .AddFilter("id", 6)
                   .WithPopulateAll();

var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Article Title: {response.Data?.First()?.Title}");
}
```

#### Get Entry by Slug

```csharp
var request = new StrapiQueryRequest("articles")
                   .AddFilter("slug", "beautiful-picture")
                   .WithPopulateAll();

var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);

if (response.IsSuccess)
{
    Console.WriteLine($"Article Title: {response.Data?.First()?.Title}");
}
```

#### Debug URL Generation

Use the new `GetQueryUrl` method to see exactly what URL will be generated:

```csharp
var firstPageRequest = new StrapiQueryRequest("articles")
                                        .WithPagination(page: 1, pageSize: 1)
                                        .WithSort("id:asc");

var query = firstPageRequest.ToQueryString();
//sort[0]=id%3aasc&pagination[pageSize]=1&pagination[page]=1

var url = firstPageRequest.ToUrl();
//http://localhost:1337/articles?sort[0]=id%3aasc&pagination[pageSize]=1&pagination[page]=1

```

#### Filtering Data

Use the fluent `WithFilter` method to apply various filters. You can chain multiple filters.

```csharp
var request = new StrapiQueryRequest("articles")
                    .AddFilter("title", FilterBuilder.ContainsCaseInsensitive("internet"))
                    .AddRelationFilter("author", "name", FilterBuilder.Contains("David"))
                    .AddPopulate("author", new PopulateOptions { Fields = new[] { "name", "email" } })
                    .AddPopulate("category", new PopulateOptions { Fields = new[] { "name", "slug" } })
                    .WithFields("title", "slug", "publishedAt", "description")
                    .WithSort("publishedAt:desc")
                    .WithPagination(page: 1, pageSize: 10)
                    .WithStatus("published")
                    .WithLocale("en");

var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);
```

#### Sorting Data

Use `WithSort` to order your results.

```csharp
var request = new StrapiQueryRequest("articles")
                    .WithSort("publishedAt:desc");

var response = await _strapiRestClient.ExecuteAsync<ICollection<Article>>(request);
```

#### Pagination

Use `WithPage` and `WithPageSize` for pagination.

```csharp
var firstPageRequest = new StrapiQueryRequest("articles")
                            .WithPagination(page: 1, pageSize: 1)
                            .WithSort("id:asc");
```

#### Populating Relations

Use `WithPopulate` to load related data.

```csharp
//Populate All - all top level data
var request = new StrapiQueryRequest("articles")
                        .AddFilter("id", 6)
                        .WithPopulateAll();

//Populate all fields in relation
var request = new StrapiQueryRequest("articles")
                    .AddPopulate("author", new PopulateOptions { Fields = new[] { "*" } });

//Specific fields
var request = new StrapiQueryRequest("articles")
                           .AddPopulate("category", new PopulateOptions
                           {
                               Fields = new[] { "name", "slug" }
                           });

//Populate dynamic blocks
var request = new StrapiQueryRequest("articles")
                            .AddPopulateAll("blocks");
```


## 4. Error Handling

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