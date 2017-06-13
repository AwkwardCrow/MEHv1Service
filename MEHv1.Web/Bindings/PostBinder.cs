using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEHv1.Services.Models;
using Nancy;
using Nancy.ModelBinding;

namespace MEHv1.Web.Bindings
{
  public class PostBinder : IModelBinder
  {
    public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
    {
      var postRequest = (instance as Post) ?? new Post();

      var form = context.Request.Form;

      postRequest.Title = form["title"];
      postRequest.Content = form["content"];
      postRequest.Images = GetFiles(context);
      postRequest.Header = form["header"];
      postRequest.SubHeader = form["subheader"];
      postRequest.SlugDescription = form["slugdescription"];
      postRequest.SlugImage = GetFileByKey(context, "file");


      return postRequest;
    }

    private IList<string> GetTags(dynamic field)
    {
      try
      {
        var tags = (string)field;
        return tags.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
      }
      catch
      {
        return new List<string>();
      }
    }

    private HttpFile GetFileByKey(NancyContext context, string key)
    {
      IEnumerable<HttpFile> files = context.Request.Files;
      if (files != null)
      {
        return files.FirstOrDefault(x => x.Key == key);
      }
      return null;
    }

    private HttpFile[] GetFiles(NancyContext context)
    {
      IEnumerable<HttpFile> files = context.Request.Files;
      if (files != null)
      {
        return files.ToArray();
      }
      return null;
    }

    public bool CanBind(Type modelType)
    {
      return modelType == typeof(Post);
    }
  }
}
