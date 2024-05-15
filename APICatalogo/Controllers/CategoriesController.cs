using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILogger<CategoriesController> logger, IUnitOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        //[HttpGet("ReadingFileConfiguration")]

        ////Ao invés de criar uma requisição eu posso criar uma variável na classe program, recebendo o builder.configuration[chave]
        //public string GetConfiguration()
        //{
        //    var valor1 = _configuration["chave1"];
        //    var valor2 = _configuration ["chave2"];
        //    var secao1 = _configuration ["secao1:chave2"];

        //    return $"Chave1 = {valor1}\n Chave2 = {valor2}\n secao1 => valor2 = {secao1}";
        //}

        //[HttpGet("UsandoFromServices/{name}")]

        //public ActionResult<string> GetMyServices([FromServices] IMyServices myServices, string name)
        //{
        //    return myServices.Salutation(name);
        //}

        //[HttpGet("SemFromServices/{name}")]

        //public ActionResult<string> GetMySemServices(IMyServices myServices, string name)
        //{
        //    return myServices.Salutation(name);
        //}

        [HttpGet]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Category>> Get()
        {
            var categories = _uof.CategoryRepository.GetAll();
            return Ok(categories);
        }


        [HttpGet("{id:int}", Name = "GetCategory")]
        public ActionResult<Category> Get(int id)
        {
            //MIDDLEWARES
            //throw new Exception("Exceção ao retornar produto pelo id");
            //string[] teste = null;
            //if (teste.Length > 0) { }


            var category = _uof.CategoryRepository.GetById(c => c.Id == id);

            if (category is null)
            {
                _logger.LogWarning($"Category id ={id} not found");
                return NotFound($"Category id={id} not found...");
            }
            return Ok(category);          
        }

        //[HttpGet("products")]
        //public ActionResult<IEnumerable<Category>> GetCategoriesProducts()
        //{
        //    // metodo include permite carregar entidades relacionadas, ou seja, carregar a tabela products

        //    // -> Exemplo de otimização nas consultas gets

        //    //return _context.Categories.Include(p => p.Products).AsNoTracking().ToList();
        //    //return _context.Categories.Include(p => p.Products).Take(5).ToList();


        //    _logger.LogInformation(" ==================== GET CATEGORIES/PRODUCTS =========================== ");

        //    return _context.Categories.Include(p => p.Products).Where(c => c.Id <= 5).ToList();
        //}

        [HttpPost]
        public ActionResult Post(Category category)
        {
            if (category is null) 
            {
                _logger.LogWarning($"Invalid...");
                return BadRequest("Invalid...");
            }

            var categoryCreated = _uof.CategoryRepository.Add(category);
            _uof.Commit();

            return new CreatedAtRouteResult("GetCategory", new { id = categoryCreated.Id }, categoryCreated);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id,Category category)
        {
            if (id != category.Id)
            {
                _logger.LogWarning($"Invalid...");
                return BadRequest();
            }

            var categoryModified = _uof.CategoryRepository.Update(category);
            _uof.Commit();
            return Ok(categoryModified);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {

            var category = _uof.CategoryRepository.GetById(c => c.Id == id);

            if (category is null)
            {
                _logger.LogWarning($"Category id = {id} not found...");
                return BadRequest();
            }

            var categoryDeleted = _uof.CategoryRepository.Delete(category);
            _uof.Commit();
            return Ok(categoryDeleted);
        }
    }
}
