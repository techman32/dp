using NATS.Client;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RankCalculator
{
    class TextInfo
    {
        public TextInfo(string id, double data)
        {
            this.id = id;
            this.data = data;
        }
        public string id { get; set; }
        public double data { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationOptions redisConfiguration = ConfigurationOptions.Parse("localhost:6379");
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(redisConfiguration);
            IDatabase db = redisConnection.GetDatabase();
            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);

                string textKey = "TEXT-" + id;
                string text = db.StringGet(textKey);

                string rankKey = "RANK-" + id;

                double rank = CalculateRank(text);

                db.StringSet(rankKey, rank);

                TextInfo data = new TextInfo(id, rank);
                string jsonData = JsonSerializer.Serialize(data);

                byte[] jsonDataEncoded = Encoding.UTF8.GetBytes(jsonData);

                c.Publish("valuator.logs.events.rank", jsonDataEncoded);
            });

            s.Start();

            Console.WriteLine("Press Enter to exit(RankCalculator)");
            Console.ReadLine();
        }

        static double CalculateRank(string text)
        {
            int totalCharacters = text.Length;
            int nonAlphabeticCharacters = 0;

            foreach (char character in text)
            {
                if (!char.IsLetter(character))
                {
                    nonAlphabeticCharacters++;
                }
            }

            double contentRank = (double)nonAlphabeticCharacters / totalCharacters;

            return contentRank;
        }
    }
}