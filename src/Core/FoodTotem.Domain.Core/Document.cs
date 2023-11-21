using MongoDB.Bson;

namespace FoodTotem.Domain.Core;
public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}