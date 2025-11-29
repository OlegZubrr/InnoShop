using System.Security.Claims;
using AutoMapper;
using InnoShop.Products.Application.DTOs;
using InnoShop.Products.Application.Interfaces.Services;
using InnoShop.Products.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService,
        IMapper mapper,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _mapper = mapper;
        _logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        return Ok(dto);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        var dto = _mapper.Map<ProductResponseDto>(product);
        return Ok(dto);
    }


    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetByUserId(Guid userId)
    {
        var products = await _productService.GetByUserIdAsync(userId);
        var dto = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        return Ok(dto);
    }


    [HttpGet("search")]
    public async Task<ActionResult<object>> Search([FromQuery] ProductSearchDto searchDto)
    {
        var (products, totalCount) = await _productService.SearchAsync(
            searchDto.SearchTerm,
            searchDto.MinPrice,
            searchDto.MaxPrice,
            searchDto.IsAvailable,
            searchDto.UserId,
            searchDto.PageNumber,
            searchDto.PageSize);

        var productsDto = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

        return Ok(new
        {
            Products = productsDto,
            TotalCount = totalCount,
            searchDto.PageNumber,
            searchDto.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize)
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductCreateDto createDto)
    {
        var userId = GetCurrentUserId();
        var productEntity = _mapper.Map<Product>(createDto);

        var createdProduct = await _productService.CreateAsync(productEntity, userId);
        var dto = _mapper.Map<ProductResponseDto>(createdProduct);

        _logger.LogInformation("Product created: {ProductId} by user: {UserId}", createdProduct.Id, userId);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] ProductUpdateDto updateDto)
    {
        var userId = GetCurrentUserId();
        var productEntity = _mapper.Map<Product>(updateDto);

        var updatedProduct = await _productService.UpdateAsync(id, productEntity, userId);
        var dto = _mapper.Map<ProductResponseDto>(updatedProduct);

        _logger.LogInformation("Product updated: {ProductId} by user: {UserId}", id, userId);
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        await _productService.DeleteAsync(id, userId);

        _logger.LogInformation("Product deleted: {ProductId} by user: {UserId}", id, userId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Failed to retrieve user ID from the token");
        return userId;
    }
}