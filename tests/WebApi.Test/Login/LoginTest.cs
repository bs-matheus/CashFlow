using CashFlow.Communication.Requests;
using CashFlow.Exception.ErrorMessages;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login;

public class LoginTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string ROUTE = "api/login";

    private readonly HttpClient _httpClient;
    private readonly string _name;
    private readonly string _email;
    private readonly string _password;

    public LoginTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _name = webApplicationFactory.GetName();
        _email = webApplicationFactory.GetEmail();
        _password = webApplicationFactory.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        var response = await _httpClient.PostAsJsonAsync(ROUTE, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStreamAsync();

        var doc = await JsonDocument.ParseAsync(responseBody);
        doc.RootElement.GetProperty("name").GetString().ShouldBe(_name);
        doc.RootElement.GetProperty("token").GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Invalid_Login(string languageCulture)
    {
        var request = RequestLoginJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(languageCulture));

        var response = await _httpClient.PostAsJsonAsync(ROUTE, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var responseBody = await response.Content.ReadAsStreamAsync();

        var doc = await JsonDocument.ParseAsync(responseBody);

        var errors = doc.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(languageCulture));

        errors.ShouldSatisfyAllConditions(
            c => c.ShouldHaveSingleItem(),
            c => c.ShouldContain(error => error.GetString()!.Equals(expectedMessage)));
    }
}
