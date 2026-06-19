using CashFlow.Exception.ErrorMessages;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Users.Register;

public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string ROUTE = "api/users";

    private readonly HttpClient _httpClient;

    public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var response = await _httpClient.PostAsJsonAsync(ROUTE, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStreamAsync();

        var doc = await JsonDocument.ParseAsync(responseBody);
        doc.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        doc.RootElement.GetProperty("token").GetString().ShouldNotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task Error_Empty_Name()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var response = await _httpClient.PostAsJsonAsync(ROUTE, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStreamAsync();

        var doc = await JsonDocument.ParseAsync(responseBody);

        var errors = doc.RootElement.GetProperty("errorMessages").EnumerateArray();
        errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.GetString()!.Equals(ResourceErrorMessages.NAME_CANNOT_BE_EMPTY)));
    }
}
