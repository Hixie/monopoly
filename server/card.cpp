
#include "player.h"
#include "card.h"

Card::Card(Board* board, int id, std::list<Card*>* pile) : mID(id),
                                                           mBoard(board),
                                                           mOwner(NULL),
                                                           mPile(pile)
{
  mPile->push_back(this);
}

Card::~Card()
{
  // assume that the piles have a lifetime no greater than the cards
}

void Card::Handle(Player* player, Square* square, int diceTotal)
{
  mPile->push_back(this);
}


GetOutOfJailCard::GetOutOfJailCard(Board* board, int id, std::list<Card*>* pile) : Card(board, id, pile)
{
}

void GetOutOfJailCard::Handle(Player* player, Square* square, int diceTotal)
{
  mBoard->Broadcast(PIMP_SQUARE_GIVES_CARD_FACTORY(player->GetID(), square->GetID(), mID));
  mBoard->Transfer(BANK, player, this);
}


CollectFromBankCard::CollectFromBankCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                                     mAmount(amount)
{
}

void CollectFromBankCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_SQUARE_GIVES_CASH_FACTORY(player->GetID(), square->GetID(), mAmount));
  mBoard->Transfer(BANK, player, mAmount);
}


PayToBankCard::PayToBankCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                         mAmount(amount)
{
}

void PayToBankCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->ChargePlayer(player, square, mAmount);
}


CollectFromEachPlayerCard::CollectFromEachPlayerCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                                                 mAmount(amount)
{
}

void CollectFromEachPlayerCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->ChargeAllPlayers(player, this, mAmount);
}


PayToEachPlayerCard::PayToEachPlayerCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                                             mAmount(amount)
{
}

void PayToEachPlayerCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->ChargeAllPlayers(player, this, -mAmount);
}


AdvanceToSquareCard::AdvanceToSquareCard(Board* board, int id, std::list<Card*>* pile, Square* square) : Card(board, id, pile),
                                                                                                         mSquare(square)
{
}

void AdvanceToSquareCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, mSquare->GetID()));
  mBoard->MovePlayer(player, mSquare, true, diceTotal);
}


GoBackToSquareCard::GoBackToSquareCard(Board* board, int id, std::list<Card*>* pile, Square* square) : Card(board, id, pile),
                                                                                                       mSquare(square)
{
}

void GoBackToSquareCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, mSquare->GetID()));
  mBoard->MovePlayer(player, mSquare, false, diceTotal);
}


AdvanceNSquaresCard::AdvanceNSquaresCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                                     mAmount(amount)
{
}

void AdvanceNSquaresCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, mBoard->GetSquare(player->GetSquare(), mAmount)->GetID()));
  mBoard->MovePlayer(player, mAmount, diceTotal);
}


GoBackNSquaresCard::GoBackNSquaresCard(Board* board, int id, std::list<Card*>* pile, int amount) : Card(board, id, pile),
                                                                                                   mAmount(amount)
{
}

void GoBackNSquaresCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, mBoard->GetSquare(player->GetSquare(), -mAmount)->GetID()));
  mBoard->MovePlayer(player, -mAmount, diceTotal);
}


AdvanceToNextCard::AdvanceToNextCard(Board* board, int id, std::list<Card*>* pile, int type) : Card(board, id, pile),
                                                                                               mType(type)
{
}

void AdvanceToNextCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  Square* target = mBoard->GetNextPropertySquareOfType(player->GetSquare(), mType);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, target->GetID()));
  mBoard->MovePlayer(player, target, true, 0);
}


GoBackToNextCard::GoBackToNextCard(Board* board, int id, std::list<Card*>* pile, int type) : Card(board, id, pile),
                                                                                             mType(type)
{
}

void GoBackToNextCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  Square* target = mBoard->GetLastPropertySquareOfType(player->GetSquare(), mType);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, target->GetID()));
  mBoard->MovePlayer(player, target, false, 0);
}


GoToJailCard::GoToJailCard(Board* board, int id, std::list<Card*>* pile) : Card(board, id, pile)
{
}

void GoToJailCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mBoard->Broadcast(PIMP_CARD_MOVED_PLAYER_FACTORY(player->GetID(), mID, mBoard->GetJail()->GetID()));
  mBoard->GoToJail(player);
}


PayOrTakeOtherPileCard::PayOrTakeOtherPileCard(Board* board, int id, std::list<Card*>* pile, int amount, std::list<Card*>* otherPile) : Card(board, id, pile),
                                                                                                                                        mOtherPile(otherPile),
                                                                                                                                        mAmount(amount)
{
}

void PayOrTakeOtherPileCard::Handle(Player* player, Square* square, int diceTotal)
{
  Card::Handle(player, square, diceTotal);
  mDiceTotal = diceTotal;
  mBoard->Broadcast(PIMP_CARD_COLLECT_OR_RETRY_FACTORY(player->GetID()));
  mBoard->Block(square);
}

void PayOrTakeOtherPileCard::Dispatch(Player* player, Square* square, Message* message)
{
  switch (message->GetType()) {
  case PIMP_CARD_SELECT_COLLECT:
    mBoard->Broadcast(PIMP_SQUARE_GIVES_CASH_FACTORY(player->GetID(), square->GetID(), mAmount));
    mBoard->Transfer(BANK, player, mAmount);
    mBoard->Unblock();
    break;
  case PIMP_CARD_SELECT_RETRY: {
    mBoard->Unblock();
    Card* card = mOtherPile->front();
    static_cast<CardSquare*>(square)->SetLastCard(card);
    mOtherPile->pop_front();
    mBoard->Broadcast(PIMP_GOT_CARD_FACTORY(player->GetID(), mID, card->GetID()));
    card->Handle(player, square, mDiceTotal);
    break;
  }
  default:
    Card::Dispatch(player, square, message);
    break;
  }
}

void PayOrTakeOtherPileCard::RequestState(Player* player)
{
  mBoard->Send(PIMP_CARD_COLLECT_OR_RETRY_FACTORY(mBoard->GetCurrentPlayer()->GetID()), player->GetID());
}



StreetRepairsCard::StreetRepairsCard(Board* board, int id, std::list<Card*>* pile, int houses, int hotels) : Card(board, id, pile),
                                                                                                             mHouses(houses),
                                                                                                             mHotels(hotels)
{
}

void StreetRepairsCard::Handle(Player* player, Square* square, int diceTotal)
{
  int price = player->GetNumHouses() * mHouses + player->GetNumHotels() * mHotels;
  mBoard->ChargePlayer(player, square, price);
}
