using Microsoft.AspNetCore.Mvc;
using ConfigChangeTracker.Models;
using ConfigChangeTracker.Storage;
using Microsoft.Extensions.Logging;

namespace ConfigChangeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigChangeController : ControllerBase
    {
        private readonly IConfigChangeStorage _storage;
        private readonly ILogger<ConfigChangeController> _logger;

        public ConfigChangeController(IConfigChangeStorage storage, ILogger<ConfigChangeController> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        /// <summary>
        /// POST METHOD
        /// Creates a new configuration change based on the provided ConfigChange model.
        /// Validates the input, assigns a new GUID and timestamp, stores it in memory,
        /// and logs a warning.
        /// </summary>
        /// <param name="change"></param>
        /// <returns>The method returns a new ConfigChange object with its generated ID and timestamp.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] ConfigChange change)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            change.Id = Guid.NewGuid();
            change.ChangedAt = DateTime.UtcNow;
            _storage.Add(change);

            if (change.IsCritical)
                _logger.LogWarning("Critical config change created: {@ConfigChange}", change);
            else
                _logger.LogInformation("Config change created: {RuleName}", change.RuleName);

            return CreatedAtAction(nameof(GetById), new { id = change.Id }, change);
        }


        /// <summary>
        /// UPDATE METHOD
        /// This method updates an existing ConfigChange object identified by the provided ID.
        /// Method validates the input, applies changes from the updatedChange model,
        /// updates the timestamp, and logs a warning.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to update.</param>
        /// <param name="updatedChange">The updated data to apply to the existing ConfigChange.</param>
        /// <returns>The updated ConfigChange object with new values and timestamp.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] ConfigChange updatedChange)
        {
            var existing = _storage.Get(id);
            if (existing == null)
            {
                _logger.LogError("Update failed. Config change with ID {Id} not found.", id);
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            updatedChange.Id = id;
            updatedChange.ChangedAt = DateTime.UtcNow;

            _storage.Update(updatedChange);

            if (updatedChange.IsCritical)
                _logger.LogWarning("Critical change updated: {@ConfigChange}", updatedChange);
            else
                _logger.LogInformation("Config change updated: {RuleName}", updatedChange.RuleName);

            return Ok(updatedChange);
        }

        /// <summary>
        /// DELETE METHOD
        /// This method deletes an existing ConfigChange object identified by the provided ID.
        /// Method also logs information about the deletion.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to delete.</param>
        /// <returns>Returns a 204 No Content response if deletion is successful,
        /// or 404 Not Found if the object with the given ID does not exist.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var existing = _storage.Get(id);
            if (existing == null)
            {
                _logger.LogError("Delete failed. Config change with ID {Id} not found.", id);
                return NotFound();
            }

            _storage.Delete(id);
            _logger.LogInformation("Config change with ID {Id} was deleted.", id);

            return NoContent();
        }


        /// <summary>
        /// GET by ID
        /// Retrieves a ConfigChange object identified by the provided ID.
        /// Validates the input and logs information. 
        /// If the object does not exist, a 404 Not Found response is returned.
        /// </summary>
        /// <param name="id">The ID of the ConfigChange to retrieve.</param>
        /// <returns>Returns the ConfigChange object if found, or a 404 Not Found response if the object does not exist.</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var change = _storage.Get(id);
            if (change == null)
            {
                _logger.LogWarning("Config change with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved config change with ID {Id}.", id);
            return Ok(change);
        }

        /// <summary>
        /// GET ALL
        /// Method retrieves all ConfigChange objects stored in memory and method also
        /// logs the total count of retrieved changes.
        /// </summary>
        /// <returns>A collection of ConfigChange objects.</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var changes = _storage.GetAll();
            if (!changes.Any())
            {
                _logger.LogWarning("GetAll returned no config changes.");
                return Ok(changes);
            }

            _logger.LogInformation("GetAll returned {Count} config changes.", changes.Count());
            return Ok(changes);
        }

        /// <summary>
        /// GET list
        /// Retrieves a list of ConfigChange objects stored in memory, 
        /// optionally filtered by change type and/or a date range.
        /// </summary>
        /// <param name="type">Optional. Filter by the ChangeType (e.g., "add", "update", "delete").</param>
        /// <param name="from">Optional. Filter to include changes with ChangedAt >= this date.</param>
        /// <param name="to">Optional. Filter to include changes with ChangedAt <= this date.</param>
        /// <returns>A collection of ConfigChange objects that match the specified filters.</returns>
        [HttpGet("list")]
        public IActionResult List([FromQuery] string? type, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var changes = _storage.List(type, from, to);
            if(!changes.Any())
            {
                _logger.LogWarning("List returned no config changes. Filters: Type={Type}, From={From}, To={To}", type, from, to);
                return Ok(changes);
            }

            _logger.LogInformation("Retrieved list of config changes. Count: {Count}", changes.Count());
            return Ok(changes);
        }

    }
}
