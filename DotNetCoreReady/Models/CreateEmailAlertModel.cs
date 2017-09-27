using System.ComponentModel.DataAnnotations;

namespace DotNetCoreReady.Models
{
    public class CreateEmailAlertModel
    {
        [MinLength(1)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MinLength(1)]
        [Required]
        public string PackageId { get; set; }

        [Required]
        public bool OptedInToMarketing { get; set; }
    }
}