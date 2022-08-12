namespace Video_Compressor;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        async Task<List<string>> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickMultipleAsync(options);

                return result.Select(r => r.FullPath).ToList();
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }
        PickAndShow(new PickOptions { FileTypes = FilePickerFileType.Videos, PickerTitle = "Select videos." });
    }

    private void StartBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void StopBtn_Clicked(object sender, EventArgs e)
    {

    }
}

