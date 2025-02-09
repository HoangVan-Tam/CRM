using System.ComponentModel.DataAnnotations;

namespace Entities.DTO;

public class TestAPIPageModel
{
    [Required(ErrorMessage = "Mobile No is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number.")]
    public string MobileNo { get; set; } = "0385154427";

    [Required(ErrorMessage = "Message is required.")]
    public string Message { get; set; }

    [Required(ErrorMessage = "Date Entry is required.")]
    public DateTime? DateEntry { get; set; } = DateTime.UtcNow;
}
