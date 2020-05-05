using ArtListVideoManager.Interfaces;
using ArtListVideoManager.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArtListVideoManager.Services
{
    public class FFMpegConverter : IVideoConverter
    {
        private readonly ILogger _logger;

        public FFMpegConverter(ILogger<FFMpegConverter> logger)
        {
            _logger = logger;
        }
        static Dictionary<VideoFormatEnum, string> FormatExtensionsDict = new Dictionary<VideoFormatEnum, string>
        {
            [VideoFormatEnum.Hls] = "m3u8",
            [VideoFormatEnum.H264] = "h264"
        };

        public async Task<string> ConvertMedia(string videoPath, string videoName, string outputDir, VideoFormatEnum format)
        {
            if (!FormatExtensionsDict.ContainsKey(format))
                throw new ArgumentException($"The format {format} is not supported");
            var ext = FormatExtensionsDict[format];
            var outputFileName = videoName + "." + ext;
            var outputVideo = Path.Combine(outputDir, outputFileName);
            var args = $"-i \"{videoPath}\" -f hls \"{outputVideo}\"";
            var output = await Task.Run(() => RunFFMPEG(args));
            return outputFileName;
        }

        public async Task<string> SaveVideoThumbnail(string videoPath, string outputPath, float frameTimeInSeconds)
        {
            var frameTimeSpan = TimeSpan.FromSeconds(frameTimeInSeconds);

            var args = $"-i \"{videoPath}\" -ss {frameTimeSpan.ToString(@"hh\:mm\:ss\.ff")} -vframes 1 {outputPath}";

            var output = await Task.Run(() => RunFFMPEG(args));
            return outputPath;
        }
        private string RunFFMPEG(string args)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe");
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null && !outputWaitHandle.SafeWaitHandle.IsClosed)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null && !errorWaitHandle.SafeWaitHandle.IsClosed)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                CloseProcess(process);
                var returnMessage = output.ToString();
                var errorMessage = error.ToString().Trim().Trim('\r', '\n');
                var processSucceded = string.IsNullOrEmpty(errorMessage); //&&
                if (!processSucceded)
                {
                    var msg = $"input:\"{path} {args}\"{Environment.NewLine}error:\"{errorMessage}\"{Environment.NewLine}output:" + returnMessage;
                    _logger.LogError(msg);
              //      throw new Exception(msg);
                }
                CloseProcess(process);
                return returnMessage;
            }
        }
        private static void CloseProcess(Process process)
        {
  //          process.StandardInput.Flush();
  //          process.StandardInput.Close();

            process.WaitForExit();
        }
    }
}