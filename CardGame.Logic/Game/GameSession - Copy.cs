using System.Collections.Generic;
using System.Linq;
using CardGame.Logic.Game.Requests;
using CardGame.Logic.Game.Responses;
using CardGame.Logic.Helpers;

namespace CardGame.Logic.Game
{
    public class GameSession
    {
        private readonly List<Player> _players = new List<Player>();

        private readonly List<Card> _cardsOnTable = new List<Card>();

        private bool _firstMovePlayed = false;

        // TODO: Do we need a player manager?
        private int _roundFirstPlayerIndex;

        private int _currentPlayerIndex;

        private int _roundLargestCardPlayerIndex;

        private Card _roundLargestCard;

        /// <summary>
        /// Initializes new instance of GameSession and then creates players with their hands.
        /// </summary>
        public GameSession(int numberOfHumans, int numberOfAgents)
        {
            int totalPlayers = numberOfHumans + numberOfAgents;
            List<List<Card>> hands = new CardFactory().GetHands(totalPlayers);

            for (int i = 0; i < totalPlayers; i++)
            {
                if (i < numberOfHumans)
                {
                    _players.Add(new Human(hands[i]));
                }
                else
                {
                    _players.Add(new Agent(hands[i + numberOfHumans]));
                }
            }

            _players.Shuffle();
        }

        private Player CurrentPlayer => _players[_currentPlayerIndex];

        public NextMoveResponse GetNextMove()
        {
            if (!_firstMovePlayed)
            {
                while (!CurrentPlayer.IsFirstPlayer())
                {
                    MovePlayerIndex();
                }

                _roundFirstPlayerIndex = _currentPlayerIndex;
            }

            var response = new NextMoveResponse
            {
                PlayerName = CurrentPlayer.Name
            };

            if (CurrentPlayer is Agent agent)
            {
                Card card = agent.ChooseCard(_cardsOnTable);
                response.BondiResponse = PlayCard(card);
                response.CardPlayed = card;
            }

            return response;
        }

        public HumanInputResponse MakeHumanMove(HumanInputRequest request)
        {
            var response = new HumanInputResponse();
            
            Card card = CurrentPlayer
                .GetCardIfAvailable(request.SymbolName, request.CharacterIdentifier);
            if (card == null)
            {
                response.IsCorrectMove = false;
                response.ErrorMessage = "Player does not have that card!";
                return response;
            }

            if (!ValidateMove(card))
            {
                response.IsCorrectMove = false;
                response.ErrorMessage = "Your move is incorrect!";
                return response;
            }

            response.BondiResponse = PlayCard(card);
            return response;
        }

        private PlayCardResponse PlayCard(Card card)
        {
            if (!_firstMovePlayed) _firstMovePlayed = true;
            PlayCardResponse response = new PlayCardResponse();

            CurrentPlayer.PlayCard(card);
            _cardsOnTable.Add(card);

            if (_cardsOnTable.Count == 1)
            {
                if (CurrentPlayer.HasWon()) response.NameOfPlayersWhoWon = new List<string> { CurrentPlayer.Name };
                MovePlayerIndex();
                return response;
            }

            if (card.Symbol == _cardsOnTable[0].Symbol)
            {
                // Played same symbol.
                if (_roundLargestCard == null 
                    || card.Character.Rank > _roundLargestCard.Character.Rank)
                {
                    // Played larger card.
                    _roundLargestCard = card;
                    _roundLargestCardPlayerIndex = _currentPlayerIndex;
                }

                if ((_currentPlayerIndex + 1) % _players.Count == _roundFirstPlayerIndex)
                {
                    // Is final player in round.
                    _currentPlayerIndex = _roundLargestCardPlayerIndex;
                    _roundFirstPlayerIndex = _roundLargestCardPlayerIndex;
                    _roundLargestCard = null;
                }
                else MovePlayerIndex();
            }
            else
            {
                // Played different symbol.
                response.BondiReceivedCards = _cardsOnTable.ToList();
                response.BondiRecipientName = _players[_roundLargestCardPlayerIndex].Name;

                _players[_roundLargestCardPlayerIndex].TakeCards(_cardsOnTable);
                _currentPlayerIndex = _roundLargestCardPlayerIndex;
                _roundFirstPlayerIndex = _roundLargestCardPlayerIndex;
                _roundLargestCard = null;
            }

            return response;
        }

        private bool ValidateMove(Card card)
        {
            if (!_firstMovePlayed)
            {
                return card.Character == "A";
            };

            if (_cardsOnTable.Count == 0) return true;

            if (_cardsOnTable[0].Symbol != card.Symbol)
            {
                return !CurrentPlayer.HasSymbol(card.Symbol);
            }

            return true;
        }

        private void MovePlayerIndex() =>
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
    }
}
