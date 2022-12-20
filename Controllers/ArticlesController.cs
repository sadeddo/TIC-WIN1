using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using Etape1.Data;
using Etape1.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
namespace Etape1.Controllers; 


[DisableCors]
[Route("/articles")]
[ApiController]

public class ArticlesController : ControllerBase
{
    private readonly DataDbContext dbContext;
    public ArticlesController(DataDbContext dbContext,IOptions<JwtSettings> options)
    {
        this.dbContext =dbContext;
    }

    [Route("")]
    [Authorize]
    [HttpGet]
    public  async Task<IActionResult> GetArticles()
    {
        var listArticles = await dbContext.Articles.ToListAsync();
        IEnumerable<Articles> articles =  listArticles.OrderBy(a => a.Title);
        return Ok( articles );
    }

    [Route("{id:guid}")]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetArticle([FromRoute] Guid id)
    {
        var articles = await dbContext.Articles.ToListAsync();
        var article = articles.Where(u => u.Id == id);

        if(article == null)
        {
            return NotFound();
        }
        return Ok(article);

    }

    [Route("create")]
    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<IActionResult> AddArticle(ArticleRequest ArticleRequest)
    {
        var article = new Articles()
        {
            Id = Guid.NewGuid(),
            
            Title = ArticleRequest.Title,
            Author = ArticleRequest.Author,
            Body = ArticleRequest.Body

        };
        await dbContext.Articles.AddAsync(article);
        await dbContext.SaveChangesAsync();

        return Ok(article);
    }

    [Route("update/{id:guid}")]
    [Authorize(Roles ="Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromRoute] Guid id,ArticleRequest ArticleRequest)
    {
        var article = await dbContext.Articles.FindAsync(id);
        if(article != null)
        {
            article.Title = ArticleRequest.Title;
            article.Author = ArticleRequest.Author;
            article.Body = ArticleRequest.Body;

            await dbContext.SaveChangesAsync();
            return Ok(article);
        }
        return NotFound();
    }

    [Route("delete/{id:guid}")]
    [Authorize(Roles ="Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteArticle([FromRoute] Guid id)
    {
        
        var article = await dbContext.Articles.FindAsync(id);

        if(article != null)
        {
            dbContext.Remove(article);
            await dbContext.SaveChangesAsync();
            return Ok(article);
        }
        return NotFound();
        
    }
}