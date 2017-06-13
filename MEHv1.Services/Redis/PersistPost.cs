using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEHv1.Services.Models;
using Newtonsoft.Json;

namespace MEHv1.Services.Redis
{
  public class PersistPost
  {
    public PersistPost()
    {
    }

    public void Save(Post post)
    {
      var json = JsonConvert.SerializeObject(post);
      //check this to see what it looks like...
      RedisConnector.Instance.GetConnection().GetDatabase() //blah blah blah
      //use redis connection to save the object to Redis in one or more places
    }

  }
}
