﻿namespace Video_Compressor;

public partial class MainPage : ContentPage
{
    private readonly IFolderPicker _folderPicker;
    private readonly IFFMpegPlugin _ffmpegPlugin;
    public MainPage(IFolderPicker folderPicker, IFFMpegPlugin ffmpegPlugin)
    {
        InitializeComponent();
        listView.BindingContext = this;
        presetPicker.BindingContext = this;

        _folderPicker = folderPicker;
        _ffmpegPlugin = ffmpegPlugin;

        _ffmpegPlugin.Progress += ffmpegPlugin_Progress;

        cpuThreads.Maximum = Environment.ProcessorCount;
        cpuThreads.ValueChanged += CpuThreads_ValueChanged;

        presetPicker.ItemsSource = Enum.GetNames(typeof(FFMpegCore.Enums.Speed));
        presetPicker.SelectedIndexChanged += PresetPicker_SelectedIndexChanged;
    }

    private void PresetPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        _ffmpegPlugin.Speed = Enum.Parse<FFMpegCore.Enums.Speed>((string)presetPicker.SelectedItem);
    }

    private void CpuThreads_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        threadLabel.Text = $"CPU Threads: {(int)e.NewValue}";
        _ffmpegPlugin.ActiveThreads = (int)e.NewValue;
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

    private void nvencCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if(e.Value)
        {
            cpuThreads.IsEnabled = false;
            presetPicker.IsEnabled = false;

            _ffmpegPlugin.NVENC = true;
        }
        else
        {
            cpuThreads.IsEnabled = true;
            presetPicker.IsEnabled = true;

            _ffmpegPlugin.NVENC = false;
        }
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

