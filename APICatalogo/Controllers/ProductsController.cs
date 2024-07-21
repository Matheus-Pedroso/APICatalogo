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
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace APICatalogo.Controllers
{
    [EnableCors("OrigensComAcessosPermitidos")]
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

        [HttpGet]
        //[Authorize(Policy = "UserOnly")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = await _uof.ProductRepository.GetAllAsync();
            
            if (products == null) return NotFound();

            // var destino = _mapper.map<destino>(origem);
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDto);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDTO>> GetId(int id)
        {
            var product = await _uof.ProductRepository.GetByIdAsync(p => p.Id == id);
            if (product is null) return NotFound("Product not found...");

            var productDto = _mapper.Map<ProductDTO>(product);

            return productDto;
        }

        [DisableCors]
        [HttpGet("products/{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsPerCategory(int idCategory)
        {
            var products = await _uof.ProductRepository.GetProductsPerCategoryAsync(idCategory);
            if (products is null) return NotFound();

            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDto);
        }


        [HttpGet("pagination")]

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts([FromQuery] ProductsParameters productsParameters)
        {
            var products = await _uof.ProductRepository.GetProductsAsync(productsParameters);

            return ObtainProducts(products);
        }

        [HttpGet("filter/price/pagination")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFilterPrice([FromQuery] ProductsFilterPrice productPriceParams)
        {
            var products = await _uof.ProductRepository.GetProductsFilterPriceAsync(productPriceParams);
            return ObtainProducts(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDTO)
        {
            if (productDTO is null) return BadRequest();

            var product = _mapper.Map<Product>(productDTO);

            var newProduct = _uof.ProductRepository.Add(product);
            await _uof.CommitAsync();

            var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

            return new CreatedAtRouteResult("GetProduct", new { id = newProductDTO.Id }, newProductDTO);

        }


        [HttpPatch("{id}/UpdatePartial")]
        public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id,
            JsonPatchDocument<ProductDTOUpdateRequest> patchProductDTO)
        {
            if (patchProductDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var product = await _uof.ProductRepository.GetByIdAsync(c => c.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            var productUpdateReq = _mapper.Map<ProductDTOUpdateRequest>(product);

            patchProductDTO.ApplyTo(productUpdateReq, ModelState);


            if (!ModelState.IsValid || !TryValidateModel(productUpdateReq)) return BadRequest(ModelState);

            _mapper.Map(productUpdateReq, product);

            _uof.ProductRepository.Update(product);
            await _uof.CommitAsync();

            return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
        }





        [HttpPut("{id:int}")] // Put precisa ser uma atualização completa, ou seja, preciso informar todos os campos do json pra realizar a modificação

        public async Task<ActionResult<ProductDTO>> Put(int id, Product productDTO)
        {
            if (id != productDTO.Id) return BadRequest();

            var product = _mapper.Map<Product>(productDTO);

            var productModified = _uof.ProductRepository.Update(product);
            await _uof.CommitAsync();

            var newProductDto = _mapper.Map<ProductDTO>(productModified);

            return Ok(newProductDto);
        }

        [HttpDelete("{id:int}")]

        public async Task<ActionResult<ProductDTO>> Delete(int id)
        {
            var product = await _uof.ProductRepository.GetByIdAsync(p => p.Id == id);

            if (product is null) return NotFound();

            var productDeleted = _uof.ProductRepository.Delete(product);
            await _uof.CommitAsync();

            var productDeletedDto = _mapper.Map<ProductDTO>(productDeleted);

            return Ok(productDeletedDto);
        }


        private ActionResult<IEnumerable<ProductDTO>> ObtainProducts(IPagedList<Product> products)
        {
            var metadata = new
            {
                products.Count,
                products.PageSize,
                products.PageCount,
                products.TotalItemCount,
                products.HasNextPage,
                products.HasPreviousPage,
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return Ok(productsDTO);
        }
    }
}
