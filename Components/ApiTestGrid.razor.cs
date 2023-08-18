using System.Net.Mime;
using System.Text;
using System.Text.Json;
using BlazorBootstrap;
using ComponentTest.Models;
using Microsoft.AspNetCore.Components;

namespace ComponentTest.Components;

public partial class ApiTestGrid : ComponentBase
{
    private ICollection<Post>? Posts { get; set; }
    private Post? SelectedPost { get; set; }
    private string? NewPostContent { get; set; }
    
    private Grid<Post> ApiGrid { get; set; }
    
    [Parameter] public string? ApiGet { get; set; }
    
    [Inject] public IHttpClientFactory ClientFactory { get; set; }
    
    private async Task<GridDataProviderResult<Post>> PostGridDataProvider(GridDataProviderRequest<Post> request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, ApiGet);

        var client = ClientFactory.CreateClient();

        var response = client.Send(httpRequest);

        if (response.IsSuccessStatusCode)
            await GetPosts(response);

        return new GridDataProviderResult<Post> { Data = Posts ?? new List<Post>() };
    }

    private Task OnSelectedItemsChanged(HashSet<Post> posts)
    {
        
        return Task.CompletedTask;
    }

    protected async Task OnClick_AddPost()
    {
        if (!string.IsNullOrEmpty(NewPostContent))
        {
            var post = new Post { Content = NewPostContent };
            
            var content = new StringContent(
                JsonSerializer.Serialize(post),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            var client = ClientFactory.CreateClient();

            var response = await client.PostAsync(ApiGet, content);

            if (response.IsSuccessStatusCode)
                await GetPosts(response);
        }
    }

    protected async Task OnClick_DeletePosts()
    {
        if (SelectedPost != null)
        {
            var client = ClientFactory.CreateClient();
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{ApiGet}/{SelectedPost.Id}");

            var response = client.Send(httpRequest);

            await GetPosts(response);
        }
    }

    private async Task GetPosts(HttpResponseMessage response)
    {
        using var readStream = response.Content.ReadAsStreamAsync();
        Posts = await JsonSerializer.DeserializeAsync<ICollection<Post>>(readStream.Result);
        await ApiGrid.RefreshDataAsync();
    }
}