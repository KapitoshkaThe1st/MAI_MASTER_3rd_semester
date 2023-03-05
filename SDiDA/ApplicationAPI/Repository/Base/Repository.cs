using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ApplicationAPI.Requests;

// основано на статье https://medium.com/@marekzyla95/mongo-repository-pattern-700986454a0e

namespace ApplicationAPI.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class, IDocument 
    {
        protected readonly IMongoCollection<T> _collection;

        protected Repository(IMongoDbSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual IEnumerable<T> FilterBy(
            Expression<Func<T, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<T, bool>> filterExpression,
            Expression<Func<T, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual T FindOne(Expression<Func<T, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).ToEnumerable());
        }

        public virtual T FindById(string id)
        {
            return FindById(new ObjectId(id));
        }

        public virtual T FindById(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return _collection.Find(filter).SingleOrDefault();
        }

        public virtual Task<T> FindByIdAsync(string id)
        {
            return FindByIdAsync(new ObjectId(id));
        }

        public virtual Task<T> FindByIdAsync(ObjectId id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }

        public virtual void InsertOne(T document)
        {
            _collection.InsertOne(document);
        }

        public virtual Task InsertOneAsync(T document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }

        public void InsertMany(ICollection<T> documents)
        {
            _collection.InsertMany(documents);
        }

        public virtual async Task InsertManyAsync(ICollection<T> documents)
        {
            await _collection.InsertManyAsync(documents);
        }

        public void ReplaceOne(T document)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task ReplaceOneAsync(T document)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public void DeleteOne(Expression<Func<T, bool>> filterExpression)
        {
            _collection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
                _collection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<T, bool>> filterExpression)
        {
            _collection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return Task.Run(() => _collection.Find("{ }").ToEnumerable());
        }

        public Task<IEnumerable<T>> GetWithParameters(RequestParameters paramaters)
        {
            return Task.Run(() => _collection.Find("{ }")
                .Skip(paramaters.ToSkip)
                .Limit(paramaters.PageSize)
                .ToEnumerable());
        }

        public Task<IEnumerable<T>> GetWithParameters(Expression<Func<T, bool>> filterExpression, RequestParameters paramaters)
        {
            return Task.Run(() => _collection.Find(filterExpression)
                .Skip(paramaters.ToSkip)
                .Limit(paramaters.PageSize)
                .ToEnumerable());
        }
        public Task<IEnumerable<T>> GetWithParameters(Expression<Func<T, bool>> filterExpression, Expression<Func<T, object>> sortExpression, RequestParameters parameters)
        {
            return Task.Run(() => _collection.Find(filterExpression)
                .Skip(parameters.ToSkip)
                .Limit(parameters.PageSize)
                .SortByDescending(sortExpression)
                .ToEnumerable());
        }
    }
}
