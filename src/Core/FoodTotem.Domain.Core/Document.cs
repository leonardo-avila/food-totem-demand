using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace FoodTotem.Domain.Core;
public abstract class Document : IDocument
{
    [JsonIgnore]
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}