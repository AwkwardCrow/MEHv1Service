using System.Collections.Generic;
using MEHv1.Services;
using MEHv1.Services.ImageUploader;
using MEHv1.Services.Models;
using MEHv1.Services.Validation;
using Nancy;
using Nancy.ModelBinding;
using MEHv1.Services.Redis;
using Newtonsoft.Json;

namespace MEHv1.Web
{
  public class PostModule : NancyModule
  {
    private const string MessageKey = "message";
    private const string PostKey = "p";
    private readonly IFileUploadHandler fileUploadHandler;

    public PostModule(IFileUploadHandler fileUploadHandler): base("/post")
    {
      this.fileUploadHandler = fileUploadHandler;

      Get["/addPost"] = x =>
      {
				//we dont want this stuff, we just want the form
        var model = new PostModel
        {
          Post =  new Post()
        };

        //Session[MessageKey] = string.Empty;
        return View["index.html", model];
      };

			Get["/all"] = x =>
			{
				var sortedPosts = RedisConnector.Instance.GetAllPosts();
				return JsonConvert.SerializeObject(sortedPosts);
			};

			Get["/{value:string}"] = x =>
			{
				string id = x.value;
				var post = RedisConnector.Instance.GetPostById(id);
				if(post!=null)
				{
					var result = JsonConvert.SerializeObject(post);
				}
				return 400;
			};

			Post["/update"] = parameters =>
      {
        var post = this.Bind<Post>();
        var imageResults = new List<FileUploadResult>();
        var imageNames = new List<string>();
        foreach (var image in post.Images)
        {
          var uploadResult = fileUploadHandler.HandleUpload(image.Name, image.Value);
          imageResults.Add(uploadResult.Result);
          imageNames.Add(image.Name);
        }
        if (imageNames.Count > 0)
          post.ImageNames = imageNames.ToArray();

        if (post.SlugImage != null)
        {
          var stubImageResult = fileUploadHandler.HandleUpload(post.SlugImage.Name, post.SlugImage.Value).Result;
          post.SlugImageName = post.SlugImage.Name;
        }
          

				//validate the data
				//ValidatePost.Validate(post, imageResults, stubImageResult);

				// save information
				RedisConnector.Instance.Save(post);

        Session[MessageKey] = "Post Added";
        Session[PostKey] = post;
        return Response.AsRedirect("/post");
      };
    }
  }

  public class PostModel
  {
    public string Message { get; set; }
    public Post Post { get; set; }
    public bool HasMessage
    {
      get { return !string.IsNullOrWhiteSpace(Message); }
    }
  }
}
