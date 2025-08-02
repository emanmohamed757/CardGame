using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame.Logic.Game
{
    internal class BotMind
    {
        Dictionary<string, BotsPlayerView> _playerViewByName = new Dictionary<string, BotsPlayerView>();

        public void InitializeInternalView(Player player)
        {
            Player currentPlayerInView = player;

            do
            {
                _playerViewByName[currentPlayerInView.Name] = new BotsPlayerView();
                currentPlayerInView = currentPlayerInView.NextPlayer;
            }
            while (currentPlayerInView != player);
        }

        // TODO: Can this be used for efficiency?
        //public List<Card> PassedCards { get; set; } = new List<Card>();

        public BotsPlayerView GetPlayerView(string playerName) => _playerViewByName[playerName];

        // Make sure cards not in hand of all players are populated during a bondi.

        public List<Card> CardsNotInPlayersHand(string playerName, List<Card> cardsInBotHand, Symbol? symbol = null)
        {
            IEnumerable<Card> cardsNotInHand = _playerViewByName[playerName].CardsNotInHand
                .Concat(cardsInBotHand);

            if (symbol != null)
            {
                cardsNotInHand = cardsNotInHand.Where(card => card.Symbol == symbol);
            }

            return cardsInBotHand.ToList();
        }

        public void ObserveBondi(List<Card> bondiCards, string bondiRecipientName, string bondiDelivererName)
        {
            foreach (string name in _playerViewByName.Keys)
            {
                if (name == bondiRecipientName)
                {
                    _playerViewByName[name].CardsInHand.AddRange(bondiCards);

                    foreach (Card card in bondiCards)
                    {
                        _playerViewByName[name].CardsNotInHand.Remove(card);
                        if (_playerViewByName[name].SymbolsNotInHand.Contains(card.Symbol))
                        {
                            _playerViewByName[name].SymbolsNotInHand.Remove(card.Symbol);
                        }
                    }
                }
                else
                {
                    if (name == bondiDelivererName)
                    {
                        if (!_playerViewByName[name].SymbolsNotInHand.Contains(bondiCards[0].Symbol))
                        {
                            _playerViewByName[name].SymbolsNotInHand.Add(bondiCards[0].Symbol);
                        }
                    }

                    foreach (Card card in bondiCards)
                    {
                        if (_playerViewByName[name].CardsNotInHand.Contains(card))
                        {
                            _playerViewByName[name].CardsNotInHand.Add(card);
                        }
                    }
                }
            }
        }

        internal void ObservePlayCard(Card card, string playerName)
        {
            foreach (string name in _playerViewByName.Keys)
            {
                if (name == playerName)
                {
                    _playerViewByName[name].CardsInHand.Remove(card);
                    _playerViewByName[name].CardsNotInHand.Add(card);
                }
                else
                {
                    _playerViewByName[name].CardsNotInHand.Add(card);
                }
            }
        }

        internal void ObserveRoundEnd(List<Card> cards)
        {
            
        }
    }
}
