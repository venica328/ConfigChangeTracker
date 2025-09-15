using System;
using System.ComponentModel.DataAnnotations;

namespace ConfigChangeTracker.Models
{
    public class ConfigChange
    {
        /// <summary>
        /// Unique identifier for the configuration change.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the rule that was changed.
        /// </summary>
        [Required(ErrorMessage = "RuleName is required.")]
        [StringLength(20, ErrorMessage = "RuleName cannot be longer than 20 characters.")]
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// Type of change: add, update, or delete.
        /// </summary>
        [Required(ErrorMessage = "ChangeType is required.")]
        [RegularExpression("add|update|delete", ErrorMessage = "ChangeType must be 'add', 'update', or 'delete'.")]
        public string ChangeType { get; set; } = string.Empty; // add/update/delete

        /// <summary>
        /// Indicates if the change is critical. Critical changes are logged with warning level.
        /// </summary>
        public bool IsCritical { get; set; } = false;

        /// <summary>
        /// The timestamp when the change was made.
        /// Must not be in the future.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(ConfigChange), nameof(ValidateChangedAt))]
        public DateTime ChangedAt { get; set; }

        public static ValidationResult? ValidateChangedAt(DateTime changedAt, ValidationContext context)
        {
            if (changedAt > DateTime.UtcNow)
            {
                return new ValidationResult("ChangedAt cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
