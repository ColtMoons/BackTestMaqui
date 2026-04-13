using System.Net;
using System.Net.Http.Json;
using Application.Products.GetProductById;
using Domain.Products;
using FluentAssertions;
using Web.Api.Endpoints.Products.Create;
using Web.Api.Endpoints.Products.UpdateById;

namespace ArchitectureTests.IntegrationTests;

public class ProductsEndpointsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateProductRequest(
            Name: "Test Product",
            Stock: 10,
            Description: "Test Description",
            Price: 99.99m
        );

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Guid productId = await response.Content.ReadFromJsonAsync<Guid>();
        productId.Should().NotBeEmpty();
        
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain(productId.ToString());
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateProductRequest(
            Name: "", // Nombre vacío - inválido
            Stock: -1, // Stock negativo - inválido
            Description: "Test",
            Price: -10m // Precio negativo - inválido
        );

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductById_WithExistingId_ShouldReturnProduct()
    {
        // Arrange - Crear un producto primero
        var createRequest = new CreateProductRequest(
            Name: "Test Product1",
            Stock: 10,
            Description: "Test Description1",
            Price: 99.99m
        );
        HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("/products", createRequest);
        Guid productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        ProductResponse? product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        
        product.Should().NotBeNull();
        product.ProductId.Should().Be(productId);
        product.Name.Should().Be(createRequest.Name);
        product.Stock.Should().Be(createRequest.Stock);
        product.Description.Should().Be(createRequest.Description);
        product.Price.Should().Be(createRequest.Price);
    }

    [Fact]
    public async Task GetProductById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/products/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldReturnNoContent()
    {
        // Arrange - Crear un producto primero
        var createRequest = new CreateProductRequest(
            Name: "Original Product",
            Stock: 10,
            Description: "Original Description",
            Price: 50m
        );
        HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("/products", createRequest);
        Guid productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updateRequest = new UpdateProductRequest(
            Name: "Updated Product4",
            Stock: 20,
            Description: "Updated Description4",
            Price: 75m,
            Status: ProductStatus.Active
        );

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync($"/products/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que el producto fue actualizado
        HttpResponseMessage getResponse = await HttpClient.GetAsync($"/products/{productId}");
        ProductResponse? updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
        
        updatedProduct.Should().NotBeNull();
        updatedProduct.Name.Should().Be(updateRequest.Name);
        updatedProduct.Stock.Should().Be(updateRequest.Stock);
        updatedProduct.Price.Should().Be(updateRequest.Price);
    }

    [Fact]
    public async Task UpdateProduct_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest(
            Name: "Updated Product8",
            Stock: 20,
            Description: "Updated Description8",
            Price: 75m,
            Status: ProductStatus.Active
        );

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync($"/products/{nonExistingId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProduct_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Crear un producto primero
        var createRequest = new CreateProductRequest(
            Name: "Test Product9",
            Stock: 10,
            Description: "Test9",
            Price: 50m
        );
        HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("/products", createRequest);
        Guid productId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updateRequest = new UpdateProductRequest(
            Name: "", // Inválido
            Stock: -1, // Inválido
            Description: "Test",
            Price: -10m, // Inválido
            Status: ProductStatus.Active
        );

        // Act
        HttpResponseMessage response = await HttpClient.PutAsJsonAsync($"/products/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
