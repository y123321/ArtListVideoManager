using ArtListVideoManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArtListVideoManager.Helpers
{
    public static class MultipartRequestHelper
    {
        public static async Task<AddVideoModel> HandleAddVideoRequest(HttpRequest request, string outputDir, ModelStateDictionary modelState, string[] _allowedExtensions)
        {

            var boundary = request.GetMultipartBoundary();
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();
            var model = new AddVideoModel();

            while (section != null)
            {

                var fileSection = section.AsFileSection();
                if (fileSection == null)
                {
                    await MapToRequestModelProperty(section, model);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(fileSection.FileName))
                    {
                        modelState.AddModelError("File", "No file name was provided");
                        return null;
                    }
                    var ext = Path.GetExtension(fileSection.FileName);

                    if (ext == null || !_allowedExtensions.Contains(ext.Trim('.'), StringComparer.InvariantCultureIgnoreCase))
                    {
                        modelState.AddModelError("File", $"File type of {ext} is not alowed");
                        return null;
                    }
                    model.FilePath = await SaveFile(fileSection, outputDir);
                }
                section = reader.ReadNextSectionAsync().Result;
            }
            return model;
        }

        /// <summary>
        /// extract a file from the form section and saves it
        /// </summary>
        /// <param name="fileSection"></param>
        /// <returns>saved file path</returns>
        private static async Task<string> SaveFile(FileMultipartSection fileSection, string outputDir)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var fileName = string.Join("_", fileSection.FileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            var path = Path.Combine(outputDir, fileName);
            using (var stream = new FileStream(path, FileMode.Append))
                await fileSection.FileStream.CopyToAsync(stream);
            return path;
        }

        private static async Task MapToRequestModelProperty(MultipartSection section, AddVideoModel requestModel)
        {
            var data = section.AsFormDataSection();
            var csName = char.ToUpper(data.Name[0]) + data.Name.Substring(1);
            switch (csName)
            {
                case nameof(requestModel.Name):
                    var name = await section.ReadAsStringAsync();
                    requestModel.Name = Uri.EscapeUriString(name);
                    break;
                case nameof(requestModel.VideoFormat):
                    var str = await section.ReadAsStringAsync();
                    requestModel.VideoFormat = Enum.Parse<VideoFormatEnum>(str);
                    break;
            }
        }
    }
}
