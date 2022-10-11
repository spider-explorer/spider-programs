using System;
using System.Threading.Tasks;
using JavaCommons;
using SQLite;
using Tweetinvi;
using Tweetinvi.Core.DTO;
using Tweetinvi.Core.Models;
using Tweetinvi.Parameters;
Util.Print("(1)");
var userClient = new TwitterClient(
    "HqtS8BBpeJDIpznejiHJ545ao",
    "F4AvMp4kErifh92ODzeNoOjARvA2WvZtnw7qJm6yUOG6PBM7iU",
    "775364940106391552-VKv39qDbOv8vuuXKOGJACtBWJWLgRLq",
    "iunc7WZ4fxWPSG14FUbDUbI1h7VWmvrZe2thJEdwA5I9D");
Util.Print("(2)");
var userId = await userClient.Users.GetAuthenticatedUserAsync();
Util.Print("(3)");
Console.WriteLine(userId);
Util.Print("(4)");
// データベースへ接続
using (var connection = new SQLiteConnection(@"tl.net.db3"))
{
    // テーブルの作成(あればスキップ)
    connection.CreateTable<TwitterUtil.TweetStatus>();
    // データ数が0件の場合
    if (connection.Table<TwitterUtil.TweetStatus>().Count() > 0)
    {
        var maxId = connection.ExecuteScalar<long>("select max(Id) from TweetStatus");
        Util.Print(maxId, "maxId");
        var tweetList = connection.Table<TwitterUtil.TweetStatus>();
        foreach (var tweet in tweetList)
        {
            //Console.WriteLine("Id:{0}, Name:{1}, Age:{2}", user.Id, user.Name, user.Age);
            //Util.Print(tweet);
            //Util.Print(tweet.Id);
            var tweetDTO = Util.FromJson(tweet.Json);
            Util.Print(tweetDTO.id, "tweetDTO.id");
            Util.Print(tweetDTO.user.screen_name, "tweetDTO.user.screen_name");
            Util.Print(tweetDTO.full_text, "tweetDTO.full_text");
            if (tweetDTO.retweeted_status != null)
            {
                throw new Exception("?");
                /*
                   Util.Print(tweetDTO.retweeted_status.id, "tweetDTO.retweeted_status.id");
                   var retweet = await userClient.Tweets.GetTweetAsync(new GetTweetParameters((long)tweetDTO.retweeted_status.id) {
                        TweetMode = TweetMode.Extended
                   });
                   var retweetDTO = retweet.TweetDTO;
                   Util.Print(retweetDTO.Id);
                   Util.Print(retweetDTO);
                 */
            }
        }
        Environment.Exit(0);
    }
    var tweets = await userClient.Timelines.GetHomeTimelineAsync(
        new GetHomeTimelineParameters()
    {
        TweetMode = TweetMode.None,
        //MaxId = 1577896857149509632 - 1,
        PageSize = 200
    });
    //Util.Print(tweets);
    //Util.Print(tweets.Length);
    for (int i = 0; i < tweets.Length; i++)
    {
        Util.Print(tweets[i].TweetDTO.Id);
        /*
           var tweet = await userClient.Tweets.GetTweetAsync(new GetTweetParameters(tweets[i].TweetDTO.Id) {
                TweetMode = TweetMode.Extended
           });
         */
        var tweet = tweets[i];
        var tweetDTO = Util.ToDynamic(tweet.TweetDTO);
        string user = tweetDTO.user.screen_name;
        //Util.Print(tweetDTO);
        Util.Print(tweetDTO.id);
        if (tweetDTO.retweeted_status != null)
        {
            Util.Print(tweetDTO.retweeted_status.id, "tweetDTO.retweeted_status.id");
            tweet = await userClient.Tweets.GetTweetAsync(new GetTweetParameters((long)tweetDTO.retweeted_status.id)
            {
                TweetMode = TweetMode.Extended
            });
            tweetDTO = Util.ToDynamic(tweet.TweetDTO);
        }
        string json = Util.ToJson(tweetDTO, true);
        //Tweetinvi.Core.DTO.TweetDTO restored = Util.FromJson<Tweetinvi.Core.DTO.TweetDTO>(json);
        //Util.Print(restored.Id, "restored.Id");
        connection.Insert(new TwitterUtil.TweetStatus
        {
            Id = tweetDTO.id,
            IdStr = tweetDTO.id_str,
            Json = json,
            User = user,
            Flag = "New"
        });
    }
    // 全データSELECT
    /*
       // データSELECT（Id指定）
       Console.WriteLine("// データSELECT（Id指定）");
       userList = from s in connection.Table<User>()
                       where s.Id == 3
                       select s;
       foreach (User u in userList)
       {
            Console.WriteLine("Id=3 Id:{0}, Name:{1}, Age:{2}", u.Id, u.Name, u.Age);
       }
       // データSELECT（Nameにaを含む）
       Console.WriteLine("// データSELECT（Nameにaを含む）");
       userList = from s in connection.Table<User>()
                       where s.Name.Contains("a")
                       select s;
       foreach (User u in userList)
       {
            Console.WriteLine("Contains(a) Id:{0}, Name:{1}, Age:{2}", u.Id, u.Name, u.Age);
       }
       // データSELECT（Ageが20以上かつ40以下）
       Console.WriteLine("// データSELECT（Ageが20以上かつ40以下）");
       userList = from s in connection.Table<User>()
                       where s.Age >= 20 && s.Age <= 40
                       select s;
       foreach (User u in userList)
       {
            Console.WriteLine("20 <= age <= 40 Id:{0}, Name:{1}, Age:{2}", u.Id, u.Name, u.Age);
       }
     */
}
