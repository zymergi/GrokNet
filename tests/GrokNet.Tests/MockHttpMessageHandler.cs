using System.Net;

namespace GrokNet.Tests;

public class MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        return Task.FromResult(new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
