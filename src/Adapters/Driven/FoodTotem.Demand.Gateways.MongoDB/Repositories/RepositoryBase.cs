using FoodTotem.Data.Core;
using FoodTotem.Demand.Gateways.MongoDB.Setup;
using FoodTotem.Domain.Core;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FoodTotem.Demand.Gateways.MongoDB.Repositories
{
	public abstract class RepositoryBase<TDocument> : IRepository<TDocument> where TDocument: IDocument
	{
        private readonly IMongoCollection<TDocument> _collection;

		protected RepositoryBase(IConfiguration configuration)
        {
            var settings = configuration.GetSection("DemandDatabaseSettings").Get<DemandDatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private static protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }
        public virtual async Task<IEnumerable<TDocument>> GetAll() => await _collection.Find(_ => true).ToListAsync();

        public virtual async Task<TDocument> Get(string id) {
            var objectId = ObjectId.Parse(id);
            return await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync();
        
        }

        public virtual async Task<TDocument> Get(ObjectId id) => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        
		public virtual async Task<bool> Create(TDocument entity)
		{
            try {
                await _collection.InsertOneAsync(entity);
                return true;
            } catch {
                return false;
            }
		}

		public virtual async Task<bool> Update(TDocument entity)
		{
            try {
                await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return true;
            } catch {
                return false;
            }
        }

		public virtual async Task<bool> Delete(TDocument entity)
		{
            try {
                await _collection.DeleteOneAsync(x => x.Id == entity.Id);
                return true;
            } catch {
                return false;
            }
		}
    }
}