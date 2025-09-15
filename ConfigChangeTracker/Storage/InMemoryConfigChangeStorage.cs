using System;
using System.Collections.Generic;
using System.Linq;
using ConfigChangeTracker.Models;

namespace ConfigChangeTracker.Storage
{
    /// <summary>
    /// Interface for managing ConfigChange objects in storage.
    /// Defines methods for adding, updating, retrieving, listing, and deleting changes.
    /// </summary>
    public interface IConfigChangeStorage
    {
        /// <summary>
        /// Adds a new ConfigChange object to storage.
        /// </summary>
        /// <param name="change">The ConfigChange object to add.</param>
        void Add(ConfigChange change);

        /// <summary>
        /// Retrieves a ConfigChange object by its ID.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to retrieve.</param>
        /// <returns>The ConfigChange object if found, otherwise returns null.</returns>
        ConfigChange? Get(Guid id);

        /// <summary>
        /// Updates an existing ConfigChange object in storage.
        /// </summary>
        /// <param name="change">The updated ConfigChange object.</param>
        void Update(ConfigChange change);

        /// <summary>
        /// Deletes a ConfigChange object by its ID.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to delete.</param>
        void Delete(Guid id);

        /// <summary>
        /// Returns all ConfigChange objects in storage.
        /// </summary>
        /// <returns>A collection of all ConfigChange objects.</returns>
        IEnumerable<ConfigChange> GetAll();

        /// <summary>
        /// Returns a filtered list of ConfigChange objects.
        /// </summary>
        /// <param name="type">Optional filter by ChangeType.</param>
        /// <param name="from">Optional filter: only changes with ChangedAt >= this date.</param>
        /// <param name="to">Optional filter: only changes with ChangedAt <= this date.</param>
        /// <returns></returns>
        IEnumerable<ConfigChange> List(string? type = null, DateTime? from = null, DateTime? to = null);

    }
    public class InMemoryConfigChangeStorage : IConfigChangeStorage
    {
        private readonly List<ConfigChange> _changes = new();

        /// <summary>
        /// Adds a new ConfigChange object to the in-memory storage.
        /// </summary>
        /// <param name="change">The ConfigChange object to be added.</param>
        public void Add(ConfigChange change)
        {
            _changes.Add(change);
        }

        /// <summary>
        /// Retrieves a ConfigChange object by its ID.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to retrieve.</param>
        /// <returns>The ConfigChange object if found, otherwise null.</returns>
        public ConfigChange? Get(Guid id)
        {
            return _changes.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Updates an existing ConfigChange object in the in-memory storage.
        /// Replaces the object with the same ID as the provided updatedChange.
        /// </summary>
        /// <param name="change">The updated ConfigChange object with the same ID as an existing one.</param>
        public void Update(ConfigChange change)
        {
            var existing = Get(change.Id);
            if (existing != null)
            {
                existing.RuleName = change.RuleName;
                existing.ChangeType = change.ChangeType;
                existing.IsCritical = change.IsCritical;
                existing.ChangedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Deletes a ConfigChange object from the in-memory storage by its ID.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to delete.</param>
        public void Delete(Guid id)
        {
            var change = Get(id);
            if (change != null)
            {
                _changes.Remove(change);
            }
        }

        /// <summary>
        /// Returns all ConfigChange objects stored in memory.
        /// </summary>
        /// <returns>A list of all ConfigChange objects.</returns>
        IEnumerable<ConfigChange> IConfigChangeStorage.GetAll()
        {
            return _changes;
        }

        /// <summary>
        /// Returns a filtered list of ConfigChange objects based on type and/or a date range.
        /// </summary>
        /// <param name="type">Optional. Filter by ChangeType (e.g., "add", "update", "delete").</param>
        /// <param name="from">Optional. Include changes with ChangedAt >= this date.</param>
        /// <param name="to">Optional. Include changes with ChangedAt <= this date.</param>
        /// <returns>A collection of ConfigChange objects matching the filters.</returns>
        public IEnumerable<ConfigChange> List(string? type = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _changes.AsEnumerable();
            if (!string.IsNullOrEmpty(type)) query = query.Where(c => c.ChangeType == type);
            if (from.HasValue) query = query.Where(c => c.ChangedAt >= from.Value);
            if (to.HasValue) query = query.Where(c => c.ChangedAt <= to.Value);
            return query;
        }

    }
}
