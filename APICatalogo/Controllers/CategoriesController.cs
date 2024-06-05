using APICatalogo.Context;
using APICatalogo.DTOS;
using APICatalogo.DTOS.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using X.PagedList;

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
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            var categories = await _uof.CategoryRepository.GetAllAsync();

            if (categories == null) return NotFound("Don't exist categories");


            var categoriesDTO = categories.ToCategoryDTOList();

            return Ok(categoriesDTO);
        }


        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            //MIDDLEWARES
            //throw new Exception("Exceção ao retornar produto pelo id");
            //string[] teste = null;
            //if (teste.Length > 0) { }


            var category = await _uof.CategoryRepository.GetByIdAsync(c => c.Id == id);

            if (category is null)
            {
                _logger.LogWarning($"Category id ={id} not found");
                return NotFound($"Category id={id} not found...");
            }


            var categoryDTO = category.ToCategoryDTO();

            return Ok(categoryDTO);
        }

        [HttpGet("Pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoriesParameters categoriesParameters)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesAsync(categoriesParameters);

            return ObtainCategory(categories);
        }


        [HttpGet("filter/name/pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoryFiltereds([FromQuery] CategoriesFilterName categoryFilter)
        {
            var categoriesFiltered = await _uof.CategoryRepository.GetCategoryFilterNameAsync(categoryFilter);


            return ObtainCategory(categoriesFiltered);
        }

        private ActionResult<IEnumerable<CategoryDTO>> ObtainCategory(IPagedList<Category> categories)
        {
            var metadata = new
            {
                categories.Count,
                categories.PageSize,
                categories.PageCount,
                categories.TotalItemCount,
                categories.HasNextPage,
                categories.HasPreviousPage,
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriesDTO = categories.ToCategoryDTOList();

            return Ok(categoriesDTO);
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
        public async Task<ActionResult<CategoryDTO>> Post(CategoryDTO categoryDTO)
        {
            if (categoryDTO is null) 
            {
                _logger.LogWarning($"Invalid...");
                return BadRequest("Invalid...");
            }
            var category = categoryDTO.ToCategory();

            var categoryCreated = _uof.CategoryRepository.Add(category);
            await _uof.CommitAsync();

            var newCategoryDTO = categoryCreated.ToCategoryDTO();

            return new CreatedAtRouteResult("GetCategory", new { id = newCategoryDTO.Id }, newCategoryDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> Put(int id,CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.Id)
            {
                _logger.LogWarning($"Invalid...");
                return BadRequest();
            }

            var category = categoryDTO.ToCategory();

            var categoryModified = _uof.CategoryRepository.Update(category);
            await _uof.CommitAsync();

            var newCategoryModified = categoryModified.ToCategoryDTO();

            return Ok(newCategoryModified);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> Delete(int id)
        {

            var category = await _uof.CategoryRepository.GetByIdAsync(c => c.Id == id);

            if (category is null)
            {
                _logger.LogWarning($"Category id = {id} not found...");
                return BadRequest();
            }

            var categoryDeleted = _uof.CategoryRepository.Delete(category);
            await _uof.CommitAsync();

            var categoryDeletedDTO = categoryDeleted.ToCategoryDTO();

            return Ok(categoryDeletedDTO);
        }
    }
}
