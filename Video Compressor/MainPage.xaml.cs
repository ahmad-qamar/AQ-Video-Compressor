namespace Video_Compressor;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        listView.BindingContext = this;
    }
    private async void OnCounterClicked(object sender, EventArgs e)
    {
        async Task<IEnumerable<string>> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickMultipleAsync(options);

                return result.Select(r => r.FullPath).AsEnumerable();
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                return null;
            }

            return null;
        }
        VideoLocations.AddRange(await PickAndShow(new PickOptions { FileTypes = FilePickerFileType.Videos, PickerTitle = "Select videos." }));
        listView.ItemsSource = null;
        listView.ItemsSource = VideoLocations;
    }
    public List<string> VideoLocations { get; set; } = new();
    public record VideoLocation(string Path);

    private void StartBtn_Clicked(object sender, EventArgs e)
    {
        
    }

    private void StopBtn_Clicked(object sender, EventArgs e)
    {

    }
}

