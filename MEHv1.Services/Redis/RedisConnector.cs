using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using MEHv1.Services.Models;
using Newtonsoft.Json;
using System.Globalization;
using MEHv1.Services.Utils;

namespace MEHv1.Services.Redis
{
  public class RedisConnector
  {
		//singleton pattern this bitch
		private const string postListKey = "AllPosts";
    protected static IDatabase _redis;
    private string connString;
    private bool connected;

    //set up the redis connection in here
    private static RedisConnector instance;

    private RedisConnector()
    {
      connString = "localhost:6379, defaultDatabase=1";
      Init();
    }

    public static RedisConnector Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new RedisConnector();
        }
        return instance;
      }
    }

    private void Init()
    {
			var config = ConfigurationOptions.Parse(connString);
			_redis = ConnectionMultiplexer.Connect(config).GetDatabase();
    }

		public void Save(Post post)
		{
			post.Id = GetUniqueId();
			post.Datetime = DateTime.UtcNow.toUnixTimestamp();
			var hash = GenerateRedisHash(post);
			//check this to see what it looks like...

			//store all the posts as hashes with a PK guid and a hash of the post body
			//add to redis hash set
			_redis.HashSetAsync(post.Id, hash);

			var simplePost = post;
			simplePost.Content = null;
			simplePost.Images = null;
			//store a record of ALL posts for ever and ever in a sorted set, scored by their date added for easy retreival
			_redis.SortedSetAdd(postListKey, JsonConvert.SerializeObject(simplePost), post.Datetime);
			//persist permanently somewhere, not a concern for PoC
		}

		public Post GetPostById(string id)
		{
			Post result = null;
			var post = _redis.HashGetAll(id);
			if (post.Length > 1)
				result = MapFromHash(post);
			return result;
		}

		public Post[] GetAllPosts()
		{
			List<Post> result= new List<Post>();
			var posts = _redis.SortedSetRangeByRank(postListKey, 0, -1);
			if(posts != null && posts.Length >= 0)
			{
				foreach (var post in posts)
					result.Add(JsonConvert.DeserializeObject<Post>(post));
			}
			return result.ToArray<Post>();
		}


		private static string GetUniqueId()
		{
			return Guid.NewGuid().ToString();
		}

		private static Post MapFromHash(HashEntry[] hash)
		{
			var obj = (Post)Activator.CreateInstance(typeof(Post));
			var props = typeof(Post).GetProperties();
			for (int i = 0; i < props.Count(); i++)
				for (int j = 0; j < hash.Count(); j++)
					if (props[i].Name == hash[j].Name)
					{
						var val = hash[j].Value;
						var type = props[i].PropertyType;

						if (type.IsGenericType && type.GetGenericTypeDefinition() == (typeof(Nullable<>)))
						{
							if (string.IsNullOrEmpty(val))
								props[i].SetValue(obj, null);
							else
							{
								Type t = Nullable.GetUnderlyingType(type) ?? type;
								if (t == typeof(DateTime))
									props[i].SetValue(obj, DateTime.ParseExact(val.ToString(), "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), null);
								else
								{
									object safeValue = Convert.ChangeType(val, t);
									props[i].SetValue(obj, safeValue, null);
								}
							}
						}
						else if (type == typeof(bool))
							props[i].SetValue(obj, bool.Parse(val));
						else
							props[i].SetValue(obj, Convert.ChangeType(val, type));
						break;
					}
			return obj;
		}

		private static HashEntry[] GenerateRedisHash(Post obj)
		{
			var props = typeof(Post).GetProperties();
			var hash = new HashEntry[props.Length];
			for (int i = 0; i < props.Length; i++)
			{
				var val = props[i].GetValue(obj);
				if (val != null && (props[i].PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(props[i].PropertyType) == typeof(DateTime)))
					hash[i] = new HashEntry(props[i].Name, ((DateTime)val).ToString("o", CultureInfo.InvariantCulture));
				else
					hash[i] = new HashEntry(props[i].Name, val != null ? val.ToString() : "");
			}

			return hash;
		}

	}
}
