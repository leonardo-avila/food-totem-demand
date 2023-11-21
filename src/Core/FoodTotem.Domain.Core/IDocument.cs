using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FoodTotem.Domain.Core;
public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    ObjectId Id { get; set; }

    DateTime CreatedAt { get; }
}