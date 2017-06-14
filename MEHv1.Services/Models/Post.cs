using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace MEHv1.Services.Models
{
  [Serializable]
  public class Post
  {
		public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public HttpFile[] Images { get; set; }
    public string Header { get; set; }
    public string SubHeader { get; set; }
    public HttpFile SlugImage { get; set; }
    public string SlugDescription { get; set; }
		public double Datetime { get; set; }
  }
}
