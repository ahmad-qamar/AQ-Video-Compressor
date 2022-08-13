using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace Video_Compressor.Platforms.Windows
{
    public class FFMpegVideoConverter : IFFMpegPlugin
    {
        record FFMpegVideo(string InputPath, string OutputFolder, bool AddSuffix);
        private bool isBusy = false;
        private int _processingCount = 0;
        private Queue<FFMpegVideo> _queue = new();
        public int ActiveThreads { get; set; }
        public Speed Speed { get; set; } = Speed.Faster;
        public bool NVENC { get; set; }

        public event IFFMpegPlugin.OnProgress Progress;
        public void Queue(string inputPath, string outputFolder, bool addSuffix)
        {
            Task.Run(processQueue);
            Thread.Sleep(100);

            async Task processQueue()
            {
                while (isBusy) await Task.Delay(1);
                isBusy = true;
                try
                {
                    var vid = new FFMpegVideo(inputPath, outputFolder, addSuffix);
                    var outputPath = vid.OutputFolder + $"/{(Path.GetFileNameWithoutExtension(vid.InputPath) + (vid.AddSuffix ? "-cmp" : "")) + ".mp4"}";

                    _processingCount++;

                    Action<double> progressHandler = new Action<double>(p =>
                    {
                        Progress?.Invoke((p / 100), _processingCount, outputPath);
                    });

                    var args = NVENC ? FFMpegArguments
                        .FromFileInput(vid.InputPath)
                        .OutputToFile(outputPath
                        , true, options => options
                            //.WithCustomArgument("-hwaccel cuda -hwaccel_output_format cuda")
                            .WithVideoCodec("hevc_nvenc")
                            .WithAudioCodec(AudioCodec.Aac)
                            .WithCustomArgument("-qmin 20 -qmax 52")
                            .WithFastStart())
                        .NotifyOnProgress(progressHandler, (await FFProbe.AnalyseAsync(vid.InputPath)).Duration) 
                        
                        : FFMpegArguments
                        .FromFileInput(vid.InputPath)
                        .OutputToFile(outputPath
                        , true, options => options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .WithSpeedPreset(Speed)
                            .WithConstantRateFactor(21)
                            .WithAudioCodec(AudioCodec.Aac)
                            .UsingThreads(ActiveThreads)
                            .WithFastStart())
                        .NotifyOnProgress(progressHandler, (await FFProbe.AnalyseAsync(vid.InputPath)).Duration);

                    await args.ProcessAsynchronously(true);
                }
                catch
                {
                    //Handle errors
                }

                isBusy = false;
            }
        }
    }
}