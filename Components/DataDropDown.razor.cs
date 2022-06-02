using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace ComponentTest.Components
{
    /// <summary>
    /// This drop down list (select input) only accepts a key-value pair object due to the nature of the control.
    /// Set the Json properties in the custom object in order to make sure the relevant Json data will be assigned appropriately.
    /// </summary>
    /// <typeparam name="KeyValuePair"></typeparam>
    public partial class DataDropDown<KeyValuePair> : ComponentBase
    {
        private IList<KeyValuePair>? Items { get; set; }
        private Dictionary<Guid, KeyValuePair>? Dict { get; set; }

        [Parameter] public Func<KeyValuePair, object> Selector { get; set; }
        [Parameter] public EventCallback<KeyValuePair> ValueChanged { get; set; }
        [Parameter] public bool ShowDefaultOption { get; set; } = true;
        [Parameter] public string? ApiGet { get; set; }

        [Inject] public IHttpClientFactory ClientFactory { get; set; }

        protected override void OnInitialized()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ApiGet);

            var client = ClientFactory.CreateClient();

            var response = client.Send(request);

            if (response.IsSuccessStatusCode)
            {
                using var readStream = response.Content.ReadAsStream();
                Items = JsonSerializer.Deserialize<IList<KeyValuePair>>(readStream);
            }

            // Dictionary used for "onchange" event handler and dropdown data population.
            if (Items != null && Items.Count > 0)
            {
                Dict = new Dictionary<Guid, KeyValuePair>();
                Items.ToList().ForEach(x => Dict.Add(Guid.NewGuid(), x));
            }

            base.OnInitialized();
        }

        private async Task OnChangeHandler(ChangeEventArgs args)
        {
            if (Dict.TryGetValue(Guid.Parse(args.Value.ToString()), out var selectedItem))
            {
                await ValueChanged.InvokeAsync(selectedItem);
            }
        }
    }
}
