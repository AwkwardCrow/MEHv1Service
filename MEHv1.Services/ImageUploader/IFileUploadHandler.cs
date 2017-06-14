using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEHv1.Services.ImageUploader
{
  public interface IFileUploadHandler
  {
    Task<FileUploadResult> HandleUpload(string fileName, Stream stream);
  }
}
