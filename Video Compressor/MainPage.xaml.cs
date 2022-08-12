namespace Video_Compressor;

public partial class MainPage : ContentPage
{
    private readonly IFolderPicker _folderPicker;
    public MainPage(IFolderPicker folderPicker)
    {
        InitializeComponent();
        listView.BindingContext = this;

        _folderPicker = folderPicker;
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
    string folderLoc = "";
    private async void FolderBtn_Clicked(object sender, EventArgs e)
    {
        folderLoc = await _folderPicker.PickFolder();
    }

    private async void StopBtn_Clicked(object sender, EventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {
            //Something went wrong.
        }
    }
}

