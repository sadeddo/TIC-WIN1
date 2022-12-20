using System.ComponentModel.DataAnnotations;
namespace Etape1.Models;

public class ArticleRequest
{
    [Required(ErrorMessage ="Title is required.")]
    public string  Title { get; set; }

    [Required(ErrorMessage ="Author is required.")]
    public string Author { get; set; }

    [Required(ErrorMessage ="Body is required.")]
    public string Body { get; set; }
}
