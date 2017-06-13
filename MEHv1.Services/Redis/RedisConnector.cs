using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MEHv1.Services.Redis
{
  public class RedisConnector
  {
    //singleton pattern this bitch
    protected IConnectionMultiplexer _redis;
    private string connString;
    private bool connected;
    //set up the redis connection in here
    private static RedisConnector instance;

    private RedisConnector()
    {
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
      _redis = ConnectionMultiplexer.Connect(connString);
      if (_redis.IsConnected)
        connected = true;
      else
      {
        throw new Exception("redis not connected");
      }
    }

    public IConnectionMultiplexer GetConnection()
    {
      while (!connected)
        Init();
      return _redis;
    }
  }
}
