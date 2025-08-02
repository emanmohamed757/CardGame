using System.Collections.Generic;
using System.Linq;
using CardGame.Logic.Game.Requests;
using CardGame.Logic.Game.Responses;
using CardGame.Logic.Helpers;

namespace CardGame.Logic.Game
{
    public class GameSession
    {
        private bool _isFirstMovePlayed = false;

        private bool _roundFinished = false;

        // TODO: Maybe can combine this.
        private int _numberOfPlayersInRound;

        private Player _currentPlayer;

        private Player _firstPlayerInRound;

        private Player _largestCardPlayerInRound;

        private HashSet<Player> _potentialWinnersInRound = new HashSet<Player>();

        private BotMind _botMind = new BotMind();

        /// <summary>
        /// Initializes new instance of GameSession and then creates players with their hands.
        /// </summary>
        public GameSession(int numberOfHumans, int numberOfBots, IList<string> namesOfHumans, IList<int> playerIds = null)
        {
            int totalPlayers = numberOfHumans + numberOfBots;
            List<List<Card>> hands = new CardFactory().GetHands(totalPlayers);

            for (int i = 0; i < totalPlayers; i++)
            {
                if (i < numberOfHumans)
                {
                    Players.Add(new Human(hands[i], namesOfHumans[i], playerIds[i]));
                }
                else
                {
                    Players.Add(new Bot(hands[i], $"Bot {i - numberOfHumans + 1}", _botMind));
                }
            }

            _numberOfPlayersInRound = totalPlayers;
            Players.Shuffle();
            for (int i = 0; i < totalPlayers; i++)
            {
                Players[i].NextPlayer = Players[(i + 1) % totalPlayers];
                Players[i].PreviousPlayer = Players[i == 0 ? totalPlayers - 1 : i - 1];
            }

            _botMind.InitializeInternalView(Players[0]);
        }

        public bool HasGameEnded { get; set; }

        public List<Card> CardsOnTable { get; } = new List<Card>();

        public List<Player> Players { get; } = new List<Player>();

        public Card LargestCardInRound { get; set; }

        public List<string> GetPlayerNames() =>
           Players.Select(p => p.Name).ToList();

        public NextPlayerResponse GetNextPlayer()
        {
            if (!_isFirstMovePlayed)
            {
                _currentPlayer = Players.First(pl => pl.IsFirstPlayer());
                _firstPlayerInRound = _currentPlayer;
            }
            else
            {
                if (_roundFinished)
                {
                    _currentPlayer = _largestCardPlayerInRound;
                    _firstPlayerInRound = _currentPlayer;
                    _roundFinished = false;
                }
                else
                {
                    _currentPlayer = _currentPlayer.NextPlayer;
                }
            }

            return new NextPlayerResponse
            {
                PlayerId = _currentPlayer.Id,
                PlayerName = _currentPlayer.Name,
                IsHuman = _currentPlayer.IsHuman,
                Hand = _currentPlayer.GetHand(),
            };
        }

        public PlayCardResponse PlayCard()
        {
            Bot bot = _currentPlayer as Bot;
            Card card = bot.ChooseCard(LargestCardInRound, _firstPlayerInRound, _isFirstMovePlayed);
            return PlayCard(card);
        }

        public PlayCardResponse PlayCard(string symbolName, string characterIdentifier)
        {
            Card card = _currentPlayer.GetCardIfAvailable(symbolName, characterIdentifier);
            return PlayCard(card);
        }

        private PlayCardResponse PlayCard(Card card)
        {
            var response = new PlayCardResponse();

            if (card == null)
            {
                response.IsInvalidMove = true;
                response.ErrorMessage = "You do not have that card.";
                return response;
            }

            if (!ValidateMove(card))
            {
                response.IsInvalidMove = true;
                response.ErrorMessage = "You cannot play that card.";
                return response;
            }

            _currentPlayer.PlayCard(card, CardsOnTable);
            response.PlayedCard = card;
            if (!_isFirstMovePlayed) _isFirstMovePlayed = true;

            _botMind.ObservePlayCard(card, _currentPlayer.Name);

            if (_currentPlayer.HasEmptyHand())
            {
                // Potential winner.
                _potentialWinnersInRound.Add(_currentPlayer);
            }

            if (CardsOnTable.Count > 1 && card.Symbol != CardsOnTable[0].Symbol)
            {
                // Bondi.
                response.BondiCards = CardsOnTable.ToList();
                response.BondiRecipientName = _largestCardPlayerInRound.Name;
                _largestCardPlayerInRound.TakeCards(CardsOnTable);
                _potentialWinnersInRound.Remove(_largestCardPlayerInRound);

                _botMind.ObserveBondi(response.BondiCards, response.BondiRecipientName, _currentPlayer.Name);

                FinishRound(response);
                return response;
            }
            else if (LargestCardInRound == null 
                || card.Character.Rank < LargestCardInRound.Character.Rank)
            {
                // Largest card player.
                LargestCardInRound = card;
                _largestCardPlayerInRound = _currentPlayer;
            }

            if (_currentPlayer.NextPlayer == _firstPlayerInRound)
            {
                _botMind.ObserveRoundEnd(CardsOnTable.ToList());

                FinishRound(response);
            }

            return response;
        }

        private void FinishRound(PlayCardResponse response)
        {
            _roundFinished = true;
            CardsOnTable.Clear();
            LargestCardInRound = null;
            response.RoundEnded = true;

            foreach (Player winner in _potentialWinnersInRound)
            {
                winner.PreviousPlayer.NextPlayer = winner.NextPlayer;
                winner.NextPlayer.PreviousPlayer = winner.PreviousPlayer;
                _numberOfPlayersInRound--;
            }

            response.NameOfPlayersWhoWon = _potentialWinnersInRound
                .Select(player => player.Name)
                .ToList();

            if (_numberOfPlayersInRound < 2)
            {
                response.GameEnded = true;
                HasGameEnded = true;
                if (_potentialWinnersInRound.Count > 0 && _numberOfPlayersInRound == 1)
                {
                    response.NameOfPlayerWhoLost = _potentialWinnersInRound.First().NextPlayer.Name;
                }
            }

            _potentialWinnersInRound.Clear();
        }

        private bool ValidateMove(Card card)
        {
            if (!_isFirstMovePlayed)
            {
                return card.IsStartCard;
            };

            if (CardsOnTable.Count == 0) return true;

            if (CardsOnTable[0].Symbol != card.Symbol)
            {
                return !_currentPlayer.HasSymbol(CardsOnTable[0].Symbol);
            }

            return true;
        }

        // TODO: Card currently being played?
    }
}
