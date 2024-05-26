using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using APICatalogo.Repositories;
using APICatalogo.DTOS;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using APICatalogo.Pagination;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        // eu poderia comentar a injeção do repositório genérico, uma vez que na injeção do repositório específico <product> já é herdado...
        // todos os métodos criados no repositório genérico

        public ProductsController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
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
            
            if (products == null) return NotFound();

            // var destino = _mapper.map<destino>(origem);
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDto);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        public ActionResult<ProductDTO> GetId(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.Id == id);
            if (product is null) return NotFound("Product not found...");

            var productDto = _mapper.Map<ProductDTO>(product);

            return productDto;
        }

        [HttpGet("products/{id}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsPerCategory(int idCategory)
        {
            var products = _uof.ProductRepository.GetProductsPerCategory(idCategory);
            if (products is null) return NotFound();

            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDto);
        }


        [HttpGet("pagination")]

        public ActionResult<IEnumerable<ProductDTO>> GetProducts([FromQuery] ProductsParameters productsParameters)
        {
            var products = _uof.ProductRepository.GetProducts(productsParameters);

            // objeto anonimo
            var metadata = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.hasNext,
                products.hasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDTO);
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

            var product = _mapper.Map<Product>(productDTO);

            var newProduct = _uof.ProductRepository.Add(product);
            _uof.Commit();

            var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

            return new CreatedAtRouteResult("GetProduct", new { id = newProductDTO.Id }, newProductDTO);

        }


        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProductDTOUpdateResponse> Patch(int id,
            JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if (patchProductDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var product = _uof.ProductRepository.GetById(c => c.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            var productUpdateReq = _mapper.Map<ProductDTOUpdateRequest>(product);

            patchProductDTO.ApplyTo(productUpdateReq, ModelState);


            if (!ModelState.IsValid || !TryValidateModel(productUpdateReq)) return BadRequest(ModelState);

            _mapper.Map(productUpdateReq, product);

            _uof.ProductRepository.Update(product);
            _uof.Commit();

            return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
        }





        [HttpPut("{id:int}")] // Put precisa ser uma atualização completa, ou seja, preciso informar todos os campos do json pra realizar a modificação

        public ActionResult<ProductDTO> Put(int id, Product productDTO)
        {
            if (id != productDTO.Id) return BadRequest();

            var product = _mapper.Map<Product>(productDTO);

            var productModified = _uof.ProductRepository.Update(product);
            _uof.Commit();

            var newProductDto = _mapper.Map<ProductDTO>(productModified);

            return Ok(newProductDto);
        }

        [HttpDelete("{id:int}")]

        public ActionResult<ProductDTO> Delete(int id)
        {
            var product = _uof.ProductRepository.GetById(p => p.Id == id);

            if (product is null) return NotFound();

            var productDeleted = _uof.ProductRepository.Delete(product);
            _uof.Commit();

            var productDeletedDto = _mapper.Map<ProductDTO>(productDeleted);

            return Ok(productDeletedDto);
        }
    }
}
