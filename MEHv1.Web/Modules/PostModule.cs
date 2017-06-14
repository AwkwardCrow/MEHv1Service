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
        //var message = Session[MessageKey] != null ? Session[MessageKey].ToString() : string.Empty;
        //var model = new PostModel
        //{
        //  Message = message,
        //  Post = Session[PostKey] == null ? new Post() : (Post)Session[PostKey],
        //};
        //Session[MessageKey] = string.Empty;
        return View["index.html"];
      };

			Get["/all"] = x =>
			{
				var sortedPosts = RedisConnector.GetAllPosts();
				return JsonConvert.SerializeObject(sortedPosts);
			};

			Get["/{value:int}"] = x =>
			{
				string id = x.value.ToString();
				var post = RedisConnector.GetPostById(id);
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
        foreach (var image in post.Images)
        {
          var uploadResult = fileUploadHandler.HandleUpload(image.Name, image.Value);
          imageResults.Add(uploadResult.Result);
        }
        
        var stubImageResult = fileUploadHandler.HandleUpload(post.SlugImage.Name, post.SlugImage.Value).Result;

				//validate the data
				//ValidatePost.Validate(post, imageResults, stubImageResult);

				// save information
				RedisConnector.Save(post);

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
