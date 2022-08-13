namespace Video_Compressor;

public partial class MainPage : ContentPage
{
    private readonly IFolderPicker _folderPicker;
    private readonly IFFMpegPlugin _ffmpegPlugin;
    public MainPage(IFolderPicker folderPicker, IFFMpegPlugin ffmpegPlugin)
    {
        InitializeComponent();
        listView.BindingContext = this;

        _folderPicker = folderPicker;
        _ffmpegPlugin = ffmpegPlugin;

        _ffmpegPlugin.Progress += ffmpegPlugin_Progress;
    }

    private Task ffmpegPlugin_Progress(double percentage, int processingCount, string fileName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            progressBar.Progress = percentage;
            progressLabel.Text = $"{processingCount}/{VideoLocations.Count} - {fileName}";
        });
        return Task.CompletedTask;
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
        foreach (var vid in VideoLocations)
        {
            _ffmpegPlugin.Queue(vid, folderLoc, checkbox_Suffix.IsChecked);
        }
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

