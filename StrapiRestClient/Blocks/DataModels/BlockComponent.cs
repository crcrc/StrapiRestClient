using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using StrapiRestClient;
using StrapiRestClient.Models;

namespace StrapiRestClient.Blocks.DataModels
{
    public interface IBlockComponent
    {
        public int? id { get; set; }
        public string? __component { get; set; }
    }

    public class RichTextBlockComponent : RichText, IBlockComponent
    {
        public string? __component { get; set; }
    }

    public class QuoteBlockComponent : Quote, IBlockComponent
    {
        public string? __component { get; set; }
    }

    public class MediaBlockComponent : IBlockComponent
    {
        public int? id { get; set; }
        public string? __component { get; set; }
        public Media? file { get; set; }
    }

    public class SliderBlockComponent: IBlockComponent
    {
        public string? __component { get; set; }
        public int? id { get; set; }
        public List<Media>? files { get; set; }
    }

    public class GenericBlock : IBlockComponent
    {
        public string? __component { get; set; }
        public int? id { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; } = new();
    }



}
