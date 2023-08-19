using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ComponentTest.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Telerik.Blazor.Components;

namespace ComponentTest.Components;

public partial class ApiTestGrid : ComponentBase
{
    private ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
    
    [Parameter] public string? ApiUrl { get; set; }
    
    [Inject] public HttpClient? HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
    }

    public void EditHandler(GridCommandEventArgs args)
    {
        Post item = (Post)args.Item;
        
    }

    public void CancelHandler(GridCommandEventArgs args)
    {
        Post item = (Post)args.Item;
    }

    public async Task UpdateHandler(GridCommandEventArgs args)
    {
        Post item = (Post)args.Item;

        var jsonPost = new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using (var httpResponseMessage = await HttpClient.PutAsync($"{ApiUrl}/{item.Id}", jsonPost))
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
        };
    }

    protected async Task CreateHandler(GridCommandEventArgs args)
    {
        Post item = (Post)args.Item;
        
        var jsonPost = new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using (var httpResponseMessage = await HttpClient.PostAsync(ApiUrl, jsonPost))
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
        }
    }

    protected async Task DeleteHandler(GridCommandEventArgs args)
    {
        Post item = (Post)args.Item;
        await HttpClient.DeleteAsync($"{ApiUrl}/{item.Id}");
        Posts = await HttpClient.GetFromJsonAsync<ObservableCollection<Post>>(ApiUrl);
    }
}