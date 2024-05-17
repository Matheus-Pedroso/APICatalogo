using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using APICatalogo.Repositories;
using APICatalogo.DTOS;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        // eu poderia comentar a injeção do repositório genérico, uma vez que na injeção do repositório específico <product> já é herdado...
        // todos os métodos criados no repositório genérico

        public ProductsController(IUnitOfWork uof)
        {
            _uof = uof;
        }

        // Posso criar várias endpoints distintos que vai chamar apenas um metodo
        //[HttpGet("/first")]
        //[HttpGet("teste")]
        //[HttpGet("/primeito")]

        //[HttpGet("{valor:alpha:length(5)}")]
        //public ActionResult<Product> GetFirst()
        //{
        //    var product = _context.Products.FirstOrDefault();
        //    if (product == null) return NotFound("There are no products registered");
        //    return product;
        //}

        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> Get()
        {
            var products = _uof.ProductRepository.GetAll();
            if (products is null) return NotFound();
            return Ok(products);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        public ActionResult<Product> GetId(int id)
        {
            var produto = _uof.ProductRepository.GetById(p => p.Id == id);
            if (produto is null) return NotFound("Product not found...");

            return produto;
        }

        [HttpGet("products/{id}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsPerCategory(int idCategory)
        {
            var products = _uof.ProductRepository.GetProductsPerCategory(idCategory);
            if (products is null) return NotFound();

            return Ok(products);
        }

        //[HttpGet("{id:int}", Name = "GetProduct")]
        //public async Task<ActionResult<Product>> Get([FromQuery]int id)
        //{
        //    var produto = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        //    if (produto is null) return NotFound("Product not found...");

        //    return produto;
        //}

        [HttpPost]
        public ActionResult<ProductDTO> Post(ProductDTO productDTO)
        {
            if (productDTO is null) return BadRequest();

            var newProduct = _uof.ProductRepository.Add(product);
            _uof.Commit();

            return new CreatedAtRouteResult("GetProduct", new { id = newProduct.Id }, newProduct);

        }

        [HttpPut("{id:int}")] // Put precisa ser uma atualização completa, ou seja, preciso informar todos os campos do json pra realizar a modificação

        public ActionResult<ProductDTO> Put(int id, Product productDTO)
        {
            if (id != productDTO.Id) return BadRequest();

            var productModified = _uof.ProductRepository.Update(product);
            _uof.Commit();

            return Ok(productModified);
        }

        [HttpDelete("{id:int}")]

        public ActionResult<ProductDTO> Delete(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.Id == id);

            if (product is null) return NotFound();

            var productDeleted = _uof.ProductRepository.Delete(product);
            _uof.Commit();

            return Ok(productDeleted);
        }
    }
}
