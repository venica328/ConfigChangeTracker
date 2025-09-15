using ConfigChangeTracker.Models;
using System;
using Xunit;

namespace ConfigChangeTracker.Tests
{
    public class InMemoryConfigChangeStorage
    {

        [Fact]
        public void Add_ShouldStoreConfigChange()
        {
            // Arrange
            var storage = new ConfigChangeTracker.Storage.InMemoryConfigChangeStorage();
            var change = new ConfigChangeTracker.Models.ConfigChange
            {
                Id = Guid.NewGuid(),
                RuleName = "TestRule",
                ChangeType = "add",
                ChangedAt = DateTime.UtcNow,
                IsCritical = false
            };

            // Act
            storage.Add(change);
            var result = storage.Get(change.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestRule", result!.RuleName);
        }

        [Fact]
        public void Update_ShouldModifyExisting()
        {
            // Arrange
            var storage = new ConfigChangeTracker.Storage.InMemoryConfigChangeStorage();
            var change = new ConfigChangeTracker.Models.ConfigChange
            {
                Id = Guid.NewGuid(),
                RuleName = "Before",
                ChangeType = "add",
                ChangedAt = DateTime.UtcNow,
                IsCritical = false
            };
            storage.Add(change);

            // Act
            change.RuleName = "After";
            storage.Update(change);
            var result = storage.Get(change.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("After", result!.RuleName);
        }

        [Fact]
        public void Delete_ShouldRemove()
        {
            // Arrange
            var storage = new ConfigChangeTracker.Storage.InMemoryConfigChangeStorage();
            var change = new ConfigChangeTracker.Models.ConfigChange
            {
                Id = Guid.NewGuid(),
                RuleName = "ToDelete",
                ChangeType = "add",
                ChangedAt = DateTime.UtcNow,
                IsCritical = false
            };
            storage.Add(change);

            // Act
            storage.Delete(change.Id);
            var result = storage.Get(change.Id);

            // Assert
            Assert.Null(result);
        }
    }
}