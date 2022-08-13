using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Video_Compressor
{
    public interface IFFMpegPlugin
    {
        public delegate Task OnProgress(double percentage, int processingCount, string fileName);
        public event OnProgress Progress;
        public int ActiveThreads { get; set; }
        public void Queue(string inputPath, string outputFolder, bool addSuffix);
    }
}
