using System.Collections.Generic;

namespace StrapiRestClient.Models
{
    public class StrapiCollectionResponse<T>
    {
        public T Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int Total { get; set; }
    }
}
