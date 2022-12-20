using System.ComponentModel.DataAnnotations;

namespace Etape1.Models;

public class Articles
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Body { get; set; }

    
}