using FFMpegCore;
using FFMpegCore.Enums;
using System.Diagnostics;

namespace FFMpegPlugin
{
    internal record FFMpegVideo(string InputPath, string OutputFolder, bool AddSuffix);
    public class FFMpegVideoConverter
    {
        private bool isBusy = false;
        private int _processingCount = 0;
        private Queue<FFMpegVideo> _queue = new();
        public delegate Task OnProgress(double percentage, int processingCount);
        public event OnProgress? Progress;

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
                    _processingCount++;

                    Action<double> progressHandler = new Action<double>(p =>
                    {
                        Progress?.Invoke((p / 100), _processingCount);
                    });

                    var args = FFMpegArguments
                        .FromFileInput(vid.InputPath)
                        .OutputToFile(vid.OutputFolder + $"/{(Path.GetFileNameWithoutExtension(vid.InputPath) + (vid.AddSuffix ? "-cmp" : "")) + ".mp4"}"
                        , false, options => options
                            .WithVideoCodec(VideoCodec.LibX265)
                            .WithConstantRateFactor(28)
                            .WithAudioCodec(AudioCodec.Aac)
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