
#include <iostream.h>
#include "board.h"
#include "card.h"
#include "player.h"
#include "property.h"
#include "transaction.h"
#include "claim.h"
#include "square.h"

Square::Square(Board* board, int id):mID(id),
                                     mBoard(board)
{
}

void Square::PassedBy(Player* player, bool forwards)
{
  mBoard->Broadcast(PIMP_PLAYER_PASSING_BY_SQUARE_FACTORY(player->GetID(), GetID()));
}

void Square::LandedOn(Player* player, bool forwards, int diceTotal)
{
  player->SetSquare(this);
  mBoard->Broadcast(PIMP_PLAYER_LANDING_ON_SQUARE_FACTORY(player->GetID(), GetID()));
}


GoSquare::GoSquare(Board* board, int id):Square(board, id)
{
}

void GoSquare::PassedBy(Player* player, bool forwards)
{
  Square::PassedBy(player, forwards);
  player->Lapped(forwards);
  if (forwards)
    mBoard->AddClaim(new GoClaim(mBoard, player, this, GO_AMOUNT));
}

void GoSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  player->Lapped(forwards);
  if (forwards) {
    if (mBoard->IsDoubleGoMoneyEnabled())
      mBoard->AddClaim(new GoClaim(mBoard, player, this, GO_AMOUNT*2));
    else
      mBoard->AddClaim(new GoClaim(mBoard, player, this, GO_AMOUNT));
  }
}


PropertySquare::PropertySquare(Board* board, int id, Property* property):Square(board, id),
                                                                         mProperty(property)
{
  mProperty->SetSquare(this);
}

int PropertySquare::GetPropertyType() { return mProperty->GetType(); }

void PropertySquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  Player* owner = mProperty->GetOwner();
  if (!owner && player->CanBuyProperties()) {
    // It's for sale and the player has passed go for the first time.
    mBoard->SellProperty(mProperty);
  } else if (owner && owner != player && !mProperty->IsMortgaged()) {
    // You owe money, pal!
    int rent = mProperty->GetRent(player, mBoard, diceTotal);
    if (rent)
      mBoard->AddClaim(new RentClaim(mBoard, player, owner, mProperty, rent));
  }
}


CardSquare::CardSquare(Board* board, int id, std::list<Card*>* cards):Square(board, id),
                                                                      mCards(cards),
                                                                      mLastCard(NULL)
{
}

void CardSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  mLastCard = mCards->front();
  mCards->pop_front();
  mBoard->Broadcast(PIMP_GOT_CARD_FACTORY(player->GetID(), mID, mLastCard->GetID()));
  mLastCard->Handle(player, this, diceTotal);
}

void CardSquare::Dispatch(Player* player, Message* message)
{
  if (!mLastCard)
    Square::Dispatch(player, message);
  mLastCard->Dispatch(player, this, message);
}

void CardSquare::RequestState(Player* player)
{
  if (!mLastCard)
    return;
  mBoard->Send(PIMP_GOT_CARD_FACTORY(mBoard->GetCurrentPlayer()->GetID(), mID, mLastCard->GetID()));
  mLastCard->RequestState(player);
}


IncomeTaxSquare::IncomeTaxSquare(Board* board, int id):Square(board, id)
{
}

void IncomeTaxSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  mBoard->Broadcast(PIMP_TAX_SELECT_OPTION_FACTORY(player->GetID(), mID));
  mBoard->Block(this);
}

void IncomeTaxSquare::Dispatch(Player* player, Message* message)
{
  switch (message->GetType()) {
  case PIMP_TAX_PAY_TEN_PERCENT:
    mBoard->Unblock();
    mBoard->ChargePlayer(player, this, (int) (player->GetTaxableAssets() * INCOME_TAX_FRACTION));
    break;
  case PIMP_TAX_PAY_FLAT_FEE:
    mBoard->Unblock();
    mBoard->ChargePlayer(player, this, INCOME_TAX);
    break;
  default:
    Square::Dispatch(player, message);
    break;
  }
}

void IncomeTaxSquare::RequestState(Player* player)
{
  mBoard->Send(PIMP_TAX_SELECT_OPTION_FACTORY(mBoard->GetCurrentPlayer()->GetID(), mID), player->GetID());
}


JustVisitingSquare::JustVisitingSquare(Board* board, int id, Pot* pot):Square(board, id),
                                                                       mPot(pot)
{
}

void JustVisitingSquare::PlayerSentToJail(Player* player)
{
  mBoard->Broadcast(PIMP_GOING_TO_JAIL_FACTORY(player->GetID(), GetID()));
  LandedOn(player, true, 0);
  player->SetJailTurns(1);
}

void JustVisitingSquare::PlayerWantsToPay(Player* player)
{
  Transaction* transaction = new JailTransaction(mBoard, mPot, player, this, BAIL, CARD_TYPE_GET_OUT_OF_JAIL);
  mBoard->AddTransaction(transaction, true);
}

void JustVisitingSquare::PlayerSetFree(Player* player)
{
  mBoard->Broadcast(PIMP_JAIL_FREE_FACTORY(player->GetID()));
  player->SetJailTurns(0);
}


FreeParkingSquare::FreeParkingSquare(Board* board, int id, Pot* pot):Square(board, id),
                                                                     mPot(pot)
{
}

void FreeParkingSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  if (mPot->GetPot()) {
    mBoard->Broadcast(PIMP_SQUARE_GIVES_CASH_FACTORY(player->GetID(), mID, mPot->GetPot()));
    mBoard->Transfer(BANK, player, mPot->GetPot());
    mPot->Reset();
  }
}


GoToJailSquare::GoToJailSquare(Board* board, int id):Square(board, id)
{
}

void GoToJailSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  mBoard->Broadcast(PIMP_SQUARE_MOVED_PLAYER_FACTORY(player->GetID(), mID, mBoard->GetJail()->GetID()));
  mBoard->GoToJail(player);
}


LuxuryTaxSquare::LuxuryTaxSquare(Board* board, int id):Square(board, id)
{
}

void LuxuryTaxSquare::LandedOn(Player* player, bool forwards, int diceTotal)
{
  Square::LandedOn(player, forwards, diceTotal);
  mBoard->ChargePlayer(player, this, LUXURY_TAX);
}
