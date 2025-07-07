using StrapiRestClient.Blocks.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using StrapiRestClient.Registration;

namespace StrapiRestClient.Blocks.Extensions
{
    public class ExtensibleBlocksConverter : JsonConverter<List<IBlockComponent>>
    {
        public override List<IBlockComponent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var blocks = new List<IBlockComponent>();

            if (reader.TokenType != JsonTokenType.StartArray)
                return blocks;

            var deserializationOptions = new JsonSerializerOptions(options)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var blockJson = JsonDocument.ParseValue(ref reader);
                    var componentName = blockJson.RootElement.GetProperty("__component").GetString();

                    if (string.IsNullOrEmpty(componentName))
                        continue;

                    // Try to get registered type
                    var blockType = BlockTypeRegistry.GetBlockType(componentName);

                    IBlockComponent? block;
                    if (blockType != null)
                    {
                        // Deserialize to the registered type
                        block = (IBlockComponent?)JsonSerializer.Deserialize(blockJson.RootElement.GetRawText(), blockType, options);
                    }
                    else
                    {
                        // Fall back to generic block
                        block = JsonSerializer.Deserialize<GenericBlock>(blockJson.RootElement.GetRawText(), options);
                    }

                    if (block != null)
                        blocks.Add(block);
                }
            }

            return blocks;
        }

        public override void Write(Utf8JsonWriter writer, List<IBlockComponent> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var block in value)
            {
                JsonSerializer.Serialize(writer, block, block.GetType(), options);
            }
            writer.WriteEndArray();
        }
    }
}
