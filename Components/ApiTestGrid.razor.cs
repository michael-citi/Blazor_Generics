using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ComponentTest.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ComponentTest.Components;

public partial class ApiTestGrid : ComponentBase
{
    private ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
    private Post EditPost { get; set; }
    private string? NewContent { get; set; }
    
    [Parameter] public string? ApiUrl { get; set; }
    
    [Inject] public HttpClient? HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
    }

    protected void StartedEditingItem(Post item)
    {
        EditPost = new Post
        {
            Id = item.Id,
            Content = item.Content,
            DateCreated = item.DateCreated,
            LastModified = item.LastModified
        };
    }

    protected void CanceledEditingItem(Post item)
    {
        EditPost = new Post();
    }

    protected async Task CommittedItemChanges(Post item)
    {
        EditPost = new Post
        {
            Id = item.Id,
            Content = item.Content
        };

        var jsonPost = new StringContent(
            JsonSerializer.Serialize(EditPost),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var httpResponseMessage = await HttpClient.PutAsync($"{ApiUrl}/{EditPost.Id}", jsonPost);
        Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
    }

    protected async Task OnClick_CreatePost(string? content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            var post = new Post { Content = content };

            var jsonPost = new StringContent(
                JsonSerializer.Serialize(post),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            using var httpResponseMessage = await HttpClient.PostAsync(ApiUrl, jsonPost);
            Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
            NewContent = string.Empty;
        }
    }

    protected async Task OnClick_DeletePost(Post item)
    {
        await HttpClient.DeleteAsync($"{ApiUrl}/{item.Id}");
        Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
    }
}