using Grpc.Core;
using Server;

namespace Server.Services
{
    public class AuctionService : Auction.AuctionBase
    {
        private readonly ILogger<AuctionService> _logger;
        private int _actualBet = 0;
        private bool _isSold = false;
        private Timer _timer;

        public AuctionService(ILogger<AuctionService> logger)
        {
            _logger = logger;
            _timer = new Timer(OnTimerEnd, null, 30000, 0);
        }

        public override Task<ServerReply> RaiseBet(BetRequest request, ServerCallContext context)
        {
            if (_isSold) return Task.FromResult(new ServerReply { Message = "Product is sold" });

            if (request.Bet > _actualBet)
            {
                _actualBet = request.Bet;
                _timer = new Timer(OnTimerEnd, null, 30000, 0);
                return Task.FromResult(new ServerReply { Message = "Bet is raised" });
            }
            else
            {
                return Task.FromResult(new ServerReply { Message = $"Actual bet = {_actualBet}" });
            }
        }

        private void OnTimerEnd(object obj)
        {
            _isSold = true;
        }
    }
}