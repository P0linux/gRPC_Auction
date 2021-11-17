using System;
using System.Net.Http;
using System.Threading.Tasks;
using Server;
using Grpc.Net.Client;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5179", new GrpcChannelOptions()
            {
                HttpHandler = GetHttpClientHandler()
            });

            Console.WriteLine("Start the auction!");
            var client = new Auction.AuctionClient(channel);

            var firstBet = await client.RaiseBetAsync(new BetRequest { Bet = 0 });
            Console.WriteLine(firstBet.Message);

            while (true)
            {
                Console.Write("Enter bet: ");
                var bet = Console.ReadLine();

                var request = new BetRequest { Bet = Convert.ToInt32(bet) };
                var reply = await client.RaiseBetAsync(request);
                Console.WriteLine(reply.Message);
            }
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            return httpHandler;
        }
    }
}
