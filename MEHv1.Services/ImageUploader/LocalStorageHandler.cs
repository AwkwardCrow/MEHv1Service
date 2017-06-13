using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace MEHv1.Services.ImageUploader
{
  public class LocalStorageHandler : IFileUploadHandler
  {
    private readonly string fileUploadDir = "uploads";
    private readonly IRootPathProvider rootPathProvider;

    public LocalStorageHandler(IRootPathProvider rootPathProvider)
    {
      this.rootPathProvider = rootPathProvider;
    }

    public async Task<FileUploadResult> HandleUpload(string fileName, System.IO.Stream stream)
    {
      string uuid = GetFileName();
      string targetFile = GetTargetFile(uuid);

      using (FileStream destinationStream = File.Create(targetFile))
      {
        await stream.CopyToAsync(destinationStream);
      }

      return new FileUploadResult()
      {
        Name = fileName,
        Identifier = uuid
      };
    }

    private string GetTargetFile(string fileName)
    {
      return Path.Combine(GetUploadDirectory(), fileName);
    }

    private string GetFileName()
    {
      return Guid.NewGuid().ToString();
    }

    private string GetUploadDirectory()
    {
      var uploadDirectory = Path.Combine(rootPathProvider.GetRootPath(), fileUploadDir);

      if (!Directory.Exists(uploadDirectory))
      {
        Directory.CreateDirectory(uploadDirectory);
      }

      return uploadDirectory;
    }
  }
}
