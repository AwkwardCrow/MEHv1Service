using Nancy;

namespace MEHv1.Web
{
  public class MainModule : NancyModule
  {
    public MainModule()
    {
      Get["/"] = x => View["index.html"];
    }
  }
}
