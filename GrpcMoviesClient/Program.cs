using System;
using Grpc.Core;
using GrpcMovies;
using Grpc.Net.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrpcMovies
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Channel channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);
            var client = new Greeter.GreeterClient(channel);

            Movies movies = await client.GetMovieByNameAsync(new NameMovie { Name = "Waterworld" });
            Console.WriteLine("Movies: " + movies.ToString());
            Console.WriteLine("Press any key to exit...");
        }
    }
}
