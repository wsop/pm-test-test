namespace Test.Back.Tests;

using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class ApiTests
{
    private readonly HttpClient _client;

    public ApiTests()
    {
        // Настройка клиента с базовым URL (замените на актуальный адрес вашего приложения)
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:62629") };
    }

    [Fact]
    public async Task RegisterUser_ShouldAppearInUsersList()
    {
        // Шаг 1. Зарегистрировать нового пользователя
        var registerRequest = new
        {
            username = "testuser@example.com",
            password = "Password123",
            confirmPassword = "Password123",
            agree = true,
            country = 1,
            city = 1
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/register", registerRequest);

        // Убедиться, что регистрация прошла успешно (статус 201)
        registerResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        // Шаг 2. Проверить, что пользователь появился в списке
        var usersResponse = await _client.GetAsync("/api/users");

        // Убедиться, что запрос прошёл успешно
        usersResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var users = await usersResponse.Content.ReadFromJsonAsync<UserResponse[]>();

        // Убедиться, что пользователь присутствует в списке
        users.Should().Contain(u => u.Username == "testuser@example.com");
    }

    [Fact]
    public async Task GetCountries_ShouldReturnList()
    {
        // Получить список стран
        var response = await _client.GetAsync("/api/countries");

        // Убедиться, что запрос успешен
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var countries = await response.Content.ReadFromJsonAsync<CountryResponse[]>();

        // Убедиться, что список стран не пустой
        countries.Should().NotBeEmpty();
    }
}

// DTO для пользователей
public record UserResponse
{
    public string Username { get; init; }
    public int CountryId { get; init; }
    public int CityId { get; init; }
}

// DTO для стран
public record CountryResponse
{
    public int Id { get; init; }
    public string Name { get; init; }
}
