using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Chat.ApiTests;

/// <summary>Level 3 (функциональные): контракт HTTP-эндпоинтов канала.</summary>
public sealed class MessagesApiTests : IClassFixture<ChatApiFactory>
{
    private readonly ChatApiFactory _factory;

    public MessagesApiTests(ChatApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Health_endpoint_is_alive()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/hc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Send_returns_message_id()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/messages/send", new
        {
            product = "Chat",
            role = "@BE",
            to = "@DO",
            text = "привет из API-теста"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.GetProperty("success").GetBoolean());
        Assert.NotEqual(Guid.Empty, payload.RootElement.GetProperty("messageId").GetGuid());
    }

    [Fact]
    public async Task Send_rejects_empty_text_with_400_and_reason()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/messages/send", new
        {
            product = "Chat",
            role = "@BE",
            to = "@DO",
            text = "   "
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(payload.RootElement.GetProperty("success").GetBoolean());
        Assert.NotEmpty(payload.RootElement.GetProperty("reasons").EnumerateArray());
    }

    [Fact]
    public async Task Send_rejects_invalid_addressee_with_400()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/messages/send", new
        {
            product = "Chat",
            role = "@BE",
            to = "DO",
            text = "текст"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Read_of_unknown_message_returns_400()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/messages/{Guid.NewGuid()}/read", new
        {
            addressee = "@DO"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Inbox_endpoint_returns_view_for_role()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/messages/inbox?role=%40DO");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("@DO", payload.RootElement.GetProperty("role").GetString());
    }

    [Fact]
    public async Task Last_timestamp_endpoint_returns_number()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/messages/last-timestamp");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(long.TryParse(await response.Content.ReadAsStringAsync(), out _));
    }
}
