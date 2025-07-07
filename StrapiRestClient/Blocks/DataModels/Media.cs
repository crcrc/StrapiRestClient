using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrapiRestClient.Blocks.DataModels
{
    public class Media
    {
        public int? id { get; set; }
        public string? documentId { get; set; }
        public string? name { get; set; }
        public string? alternativeText { get; set; }
        public string? caption { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public Formats? formats { get; set; }
        public string? hash { get; set; }
        public string? ext { get; set; }
        public string? mime { get; set; }
        public float? size { get; set; }
        public string? url { get; set; }
        public object? previewUrl { get; set; }
        public string? provider { get; set; }
        public object? provider_metadata { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? publishedAt { get; set; }
    }

    public class Formats
    {
        public Thumbnail? thumbnail { get; set; }
        public Large? large { get; set; }
        public Medium? medium { get; set; }
        public Small? small { get; set; }
    }

    public class Thumbnail
    {
        public string? name { get; set; }
        public string? hash { get; set; }
        public string? ext { get; set; }
        public string? mime { get; set; }
        public object? path { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public float? size { get; set; }
        public int? sizeInBytes { get; set; }
        public string? url { get; set; }
    }

    public class Large
    {
        public string? name { get; set; }
        public string? hash { get; set; }
        public string? ext { get; set; }
        public string? mime { get; set; }
        public object? path { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public float? size { get; set; }
        public int? sizeInBytes { get; set; }
        public string? url { get; set; }
    }

    public class Medium
    {
        public string? name { get; set; }
        public string? hash { get; set; }
        public string? ext { get; set; }
        public string? mime { get; set; }
        public object? path { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public float? size { get; set; }
        public int? sizeInBytes { get; set; }
        public string? url { get; set; }
    }

    public class Small
    {
        public string? name { get; set; }
        public string? hash { get; set; }
        public string? ext { get; set; }
        public string? mime { get; set; }
        public object? path { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public float? size { get; set; }
        public int? sizeInBytes { get; set; }
        public string? url { get; set; }
    }
}
