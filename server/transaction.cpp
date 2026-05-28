#include "board.h"
#include "player.h"
#include "property.h"
#include "card.h"
#include "exceptions.h"
#include "transaction.h"

int Transaction::gID = 0;

Transaction::Transaction(Board* board) : mBoard(board),
                                         mEnded(false)
{
  mID = NewID();
}

Transaction::Transaction(Board* board, int id) : mBoard(board),
                                                 mID(id),
                                                 mEnded(false)
{
}

Transaction::~Transaction()
{
}

int Transaction::NewID()
{
  return ++gID;
}

void Transaction::Dispatch(Message* message)
{
  Player* player = mBoard->GetPlayer(message->GetPlayer());
  if (!player || !InvolvesPlayer(player)) {
    mBoard->UnexpectedMessage(message);
    return;
  }    
  try {
    switch (message->GetType()) {
    case PIMP_TRANSACTION_SET_CASH: {
      PIMP_TRANSACTION_SET_CASH_CAST(message, details);
      SetCash(player, details->GetField2());
      break;
    }
    case PIMP_TRANSACTION_ADD_PROPERTY: {
      PIMP_TRANSACTION_ADD_PROPERTY_CAST(message, details);
      Property* property = mBoard->GetProperty(details->GetField2());
      if (property) {
        AddProperty(player, property);
      } else {
        mBoard->InvalidPayload(message);
      }
      break;
    }
    case PIMP_TRANSACTION_REMOVE_PROPERTY: {
      PIMP_TRANSACTION_REMOVE_PROPERTY_CAST(message, details);
      Property* property = mBoard->GetProperty(details->GetField2());
      if (property) {
        RemoveProperty(player, property);
      } else {
        mBoard->InvalidPayload(message);
      }
      break;
    }
    case PIMP_TRANSACTION_ADD_CARD: {
      PIMP_TRANSACTION_ADD_CARD_CAST(message, details);
      Card* card = mBoard->GetCard(details->GetField2());
      if (card) {
        AddCard(player, card);
      } else {
        mBoard->InvalidPayload(message);
      }
      break;
    }
    case PIMP_TRANSACTION_REMOVE_CARD: {
      PIMP_TRANSACTION_REMOVE_CARD_CAST(message, details);
      Card* card = mBoard->GetCard(details->GetField2());
      if (card) {
        RemoveCard(player, card);
      } else {
        mBoard->InvalidPayload(message);
      }
      break;
    }
    case PIMP_TRANSACTION_FINISH:
      Finish(player);
      break;
    case PIMP_TRANSACTION_REOPEN:
      Reopen(player);
      break;
    case PIMP_TRANSACTION_AGREE:
      Agree(player);
      break;
    case PIMP_BANKRUPT_TRANSACTION:
      Bankrupt(player);
      break;
    case PIMP_TRANSACTION_CANCEL:
      Cancel(player);
      break;
    default:
      mBoard->UnexpectedMessage(message);     
    }
  } catch (UnexpectedMessageError) {
    mBoard->UnexpectedMessage(message);
  }
}

void Transaction::RequestState(Player* player)
{
  if (InvolvesPlayer(player)) {
    RequestStateHead(player);
    RequestStateBody(player);
  }
}


BilateralTransaction::BilateralTransaction(Board* board, Player* player0, Player* player1) : Transaction(board)
{
  Reset(player0, player1);
}

BilateralTransaction::BilateralTransaction(Board* board, int id, Player* player0, Player* player1) : Transaction(board, id)
{
  Reset(player0, player1);
}

void BilateralTransaction::Reset(Player* player0, Player* player1)
{
  mPlayer[0] = player0;
  mPlayer[1] = player1;
  mCash[0] = 0;
  mCash[1] = 0;
  mStatus[0] = status_setup;
  mStatus[1] = status_setup;
  mEscrow[0] = 0;
  mEscrow[1] = 0;
}

bool BilateralTransaction::InvolvesPlayer(Player* player)
{
  return player == mPlayer[0] || player == mPlayer[1];
}

int BilateralTransaction::GetParty(Player* player)
{
  // assumes that InvolvesPlayer(player) == true
  return player == mPlayer[0] ? 0 : 1;
}

void BilateralTransaction::End()
{
  Transaction::End();
  if (mEscrow[0])
    mBoard->Transfer(BANK, mPlayer[0], mEscrow[0], mID);
  if (mEscrow[1])
    mBoard->Transfer(BANK, mPlayer[1], mEscrow[1], mID);
}

int BilateralTransaction::GetNetSpendableAssetDeltaForPlayer(Player* player)
{
  int party = GetParty(player);
  int amount = 0;

  // cash
  amount -= mCash[party];
  amount += mCash[1-party];

  // property
  std::list<Property*>::iterator p;
  for (p = mProperties[party].begin(); p != mProperties[party].end(); ++p)
    amount -= (*p)->GetSpendableWorth();
  for (p = mProperties[1-party].begin(); p != mProperties[1-party].end(); ++p)
    amount += (*p)->GetSpendableWorth();

  return amount;
}

void BilateralTransaction::AddToEscrow(Player* player, int amount)
{
  mEscrow[GetParty(player)] += amount;
}

void BilateralTransaction::TransferNotification(Player* from, Player* to)
{
  if (!InvolvesPlayer(from) || mEnded)
    return;
  int party = GetParty(from);
  Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), party);
  if (to && to != mPlayer[1-party]) {
    mPlayer[party] = to;
    RequestState(to);
  } else {
    Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), 1-party);
    End();
  }
}

void BilateralTransaction::TransferNotification(Player* from, Player* to, int amount)
{
  if (!InvolvesPlayer(from) || mEnded)
    return;
  int party = GetParty(from);
  if (!mPlayer[party]->HasCash(mCash[party])) {
    EnsureOpen(mPlayer[party]);
    Send(PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_FACTORY(mID, mCash[party]), party);
    mCash[party] = mPlayer[party]->GetCash();
    Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash[party]), party);
    Send(PIMP_TRANSACTION_OTHER_CASH_SET_FACTORY(mID, mCash[party]), 1-party);
  }
}

void BilateralTransaction::TransferNotification(Player* from, Player* to, Property* property)
{
  if (!InvolvesPlayer(from) || mEnded)
    return;
  RemoveProperty(from, property);
}

void BilateralTransaction::TransferNotification(Player* from, Player* to, Card* card)
{
  if (!InvolvesPlayer(from) || mEnded)
    return;
  RemoveCard(from, card);
}

void BilateralTransaction::Begin()
{
  RequestStateHead(mPlayer[0]);
  RequestStateHead(mPlayer[1]);
}

void BilateralTransaction::EnsureOpen(Player* player)
{
  int party = GetParty(player);
  if (mStatus[party] > status_setup) {
    Send(PIMP_TRANSACTION_REOPENED_FACTORY(mID), party);
    Send(PIMP_TRANSACTION_OTHER_REOPENED_FACTORY(mID), 1-party);
    mStatus[party] = status_setup;
    if (mStatus[1-party] == status_agreed)
      mStatus[1-party] = status_finished;
  }
}

void BilateralTransaction::Finalise()
{
  // shake hands
  Send(PIMP_TRANSACTION_FINALISED_FACTORY(mID), 0);
  Send(PIMP_TRANSACTION_FINALISED_FACTORY(mID), 1);
  End();

  // transfer cash (bypassing escrow laws)
  mBoard->Transfer(mPlayer[0], mPlayer[1], mCash[0], overrideEscrow);
  mBoard->Transfer(mPlayer[1], mPlayer[0], mCash[1], overrideEscrow);

  // transfer property
  std::list<Property*>::iterator p;
  for (p = mProperties[0].begin(); p != mProperties[0].end(); ++p)
    mBoard->Transfer(mPlayer[0], mPlayer[1], (*p));
  for (p = mProperties[1].begin(); p != mProperties[1].end(); ++p)
    mBoard->Transfer(mPlayer[1], mPlayer[0], (*p));

  // transfer cards
  std::list<Card*>::iterator c;
  for (c = mCards[0].begin(); c != mCards[0].end(); ++c)
    mBoard->Transfer(mPlayer[0], mPlayer[1], (*c));
  for (c = mCards[1].begin(); c != mCards[1].end(); ++c)
    mBoard->Transfer(mPlayer[1], mPlayer[0], (*c));
}

void BilateralTransaction::Unusual()
{
  Send(PIMP_TRANSACTION_UNUSUAL_FACTORY(mID), 0);
  Send(PIMP_TRANSACTION_UNUSUAL_FACTORY(mID), 1);
}

void BilateralTransaction::SetCash(Player* player, int amount)
{
  EnsureOpen(player);
  int party = GetParty(player);
  if (player->HasCash(amount)) {
    mCash[party] = amount;
    Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash[party]), party);
    Send(PIMP_TRANSACTION_OTHER_CASH_SET_FACTORY(mID, mCash[party]), 1-party);
  } else {
    Send(PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_FACTORY(mID, amount), party);
    Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash[party]), party);
  }
}

void BilateralTransaction::AddProperty(Player* player, Property* property)
{
  int party = GetParty(player);
  if (!player->HasProperty(property)) {
    Send(PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED_FACTORY(mID, property->GetID()), party);
    return;
  }
  std::list<Property*>::iterator i;
  for (i = mProperties[party].begin(); i != mProperties[party].end(); ++i) {
    if ((*i) == property) {
      return;
    }
  }
  EnsureOpen(player);
  mProperties[party].push_back(property);
  Send(PIMP_TRANSACTION_PROPERTY_ADDED_FACTORY(mID, property->GetID()), party);
  Send(PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_FACTORY(mID, property->GetID()), 1-party);
}

void BilateralTransaction::RemoveProperty(Player* player, Property* property)
{
  int party = GetParty(player);
  std::list<Property*>::iterator i;
  for (i = mProperties[party].begin(); i != mProperties[party].end(); ++i) {
    if ((*i) == property) {
      EnsureOpen(player);
      mProperties[party].erase(i);
      Send(PIMP_TRANSACTION_PROPERTY_REMOVED_FACTORY(mID, property->GetID()), party);
      Send(PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED_FACTORY(mID, property->GetID()), 1-party);
      return;
    }
  }
}

void BilateralTransaction::AddCard(Player* player, Card* card)
{
  int party = GetParty(player);
  if (!player->HasCard(card)) {
    Send(PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED_FACTORY(mID, card->GetID()), party);
    return;
  }
  std::list<Card*>::iterator i;
  for (i = mCards[party].begin(); i != mCards[party].end(); ++i) {
    if ((*i) == card) {
      return;
    }
  }
  EnsureOpen(player);
  mCards[party].push_back(card);
  Send(PIMP_TRANSACTION_CARD_ADDED_FACTORY(mID, card->GetID()), party);
  Send(PIMP_TRANSACTION_OTHER_CARD_ADDED_FACTORY(mID, card->GetID()), 1-party);
}

void BilateralTransaction::RemoveCard(Player* player, Card* card)
{
  int party = GetParty(player);
  std::list<Card*>::iterator i;
  for (i = mCards[party].begin(); i != mCards[party].end(); ++i) {
    if ((*i) == card) {
      EnsureOpen(player);
      mCards[party].erase(i);
      Send(PIMP_TRANSACTION_CARD_REMOVED_FACTORY(mID, card->GetID()), party);
      Send(PIMP_TRANSACTION_OTHER_CARD_REMOVED_FACTORY(mID, card->GetID()), 1-party);
      return;
    }
  }
}

void BilateralTransaction::Finish(Player* player)
{
  int party = GetParty(player);
  if (mStatus[party] == status_setup) {
    mStatus[party] = status_finished;
    Send(PIMP_TRANSACTION_FINISHED_FACTORY(mID), party);
    Send(PIMP_TRANSACTION_OTHER_FINISHED_FACTORY(mID), 1-party);
  }
}

void BilateralTransaction::Reopen(Player* player)
{
  int party = GetParty(player);
  if (mStatus[party] == status_setup)
    throw UnexpectedMessageError();
  EnsureOpen(player);
}

void BilateralTransaction::Agree(Player* player)
{
  int party = GetParty(player);
  if (mStatus[party] != status_finished || mStatus[1-party] < status_finished)
    throw UnexpectedMessageError();

  // check for other transactions
  Transaction* transaction = mBoard->GetTransactionBlockingAgreement(player, this);
  if (transaction) {
    Send(PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH_FACTORY(mID, transaction->GetID()),
         party);
    return;
  }

  // check for houses
  int houses = 0;
  std::list<Property*>::iterator p;
  for (p = mProperties[party].begin(); p != mProperties[party].end(); ++p) {
    if (!(*p)->IsTransferable()) {
      ++houses;
      Send(PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE_FACTORY(mID, (*p)->GetID()), party);
    }
  }
  if (houses)
    return;

  mStatus[party] = status_agreed;
  Send(PIMP_TRANSACTION_AGREED_FACTORY(mID), party);
  Send(PIMP_TRANSACTION_OTHER_AGREED_FACTORY(mID), 1-party);
  if (mStatus[1-party] == status_agreed)
    Finalise(); // both agreed
}

void BilateralTransaction::Bankrupt(Player* player)
{
  throw UnexpectedMessageError();
}

void BilateralTransaction::Cancel(Player* player)
{
  Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), 0);
  Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), 1);
  End();
}

void BilateralTransaction::RequestStateBody(Player* player)
{
  int party = GetParty(player);
  Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash[party]), party);
  Send(PIMP_TRANSACTION_OTHER_CASH_SET_FACTORY(mID, mCash[1-party]), party);

  std::list<Property*>::iterator p;
  for (p = mProperties[party].begin(); p != mProperties[party].end(); ++p)
    Send(PIMP_TRANSACTION_PROPERTY_ADDED_FACTORY(mID, (*p)->GetID()), party);
  for (p = mProperties[1-party].begin(); p != mProperties[1-party].end(); ++p)
    Send(PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_FACTORY(mID, (*p)->GetID()), party);

  std::list<Card*>::iterator c;
  for (c = mCards[party].begin(); c != mCards[party].end(); ++c)
    Send(PIMP_TRANSACTION_CARD_ADDED_FACTORY(mID, (*c)->GetID()), party);
  for (c = mCards[1-party].begin(); c != mCards[1-party].end(); ++c)
    Send(PIMP_TRANSACTION_OTHER_CARD_ADDED_FACTORY(mID, (*c)->GetID()), party);

  if (mStatus[party] >= status_finished)
    Send(PIMP_TRANSACTION_FINISHED_FACTORY(mID), party);
  if (mStatus[1-party] >= status_finished)
    Send(PIMP_TRANSACTION_OTHER_FINISHED_FACTORY(mID), party);

  if (mStatus[party] >= status_agreed)
    Send(PIMP_TRANSACTION_AGREED_FACTORY(mID), party);
  if (mStatus[1-party] >= status_agreed)
    Send(PIMP_TRANSACTION_OTHER_AGREED_FACTORY(mID), party);

}

void BilateralTransaction::Send(Message* message, int party)
{
  mBoard->Send(message, mPlayer[party]->GetID());
}



TradeTransaction::TradeTransaction(Board* board, Player* player0, Player* player1) : BilateralTransaction(board, player0, player1)
{
}

int TradeTransaction::GetLiabilityForPlayer(Player* player) {
  return 0;
}

void TradeTransaction::RequestStateHead(Player* player)
{
  int party = GetParty(player);
  Send(PIMP_TRANSACTION_TRADE_REQUESTED_FACTORY(mID, mPlayer[1-party]->GetID()), party);
}


ForcedTransaction::ForcedTransaction(Board* board, Player* creditor, Player* debtor, int amount) : BilateralTransaction(board, creditor, debtor),
                                                                                                   mAmount(amount)
{
}

ForcedTransaction::ForcedTransaction(Board* board, int id, Player* creditor, Player* debtor, int amount) : BilateralTransaction(board, id, creditor, debtor),
                                                                                                   mAmount(amount)
{
}

void ForcedTransaction::Begin()
{
  BilateralTransaction::Begin();
  Finish(mPlayer[0]);
  SetCash(mPlayer[1], mAmount);
  if (mCash[1] == mAmount) {
    Finish(mPlayer[1]);
    Agree(mPlayer[0]);
  }
}

int ForcedTransaction::GetLiabilityForPlayer(Player* player) {
  int party = GetParty(player);
  if (party == 1)
    return mAmount;
  else
    return 0;
}

void ForcedTransaction::SetCash(Player* player, int amount)
{
  BilateralTransaction::SetCash(player, amount);
  if (mCash[0] != 0 || mCash[1] != mAmount)
    Unusual();
}

void ForcedTransaction::AddProperty(Player* player, Property* property)
{
  BilateralTransaction::AddProperty(player, property);
  Unusual();
}

void ForcedTransaction::AddCard(Player* player, Card* card)
{
  BilateralTransaction::AddCard(player, card);
  Unusual();
}

void ForcedTransaction::RequestStateBody(Player* player)
{
  BilateralTransaction::RequestStateBody(player);
  if (mCash[0] != 0 ||
      mCash[1] != mAmount ||
      mProperties[0].begin() != mProperties[0].end() ||
      mProperties[1].begin() != mProperties[1].end() ||
      mCards[0].begin() != mCards[0].end() ||
      mCards[1].begin() != mCards[1].end() ||
      mStatus[0] != status_agreed ||
      mStatus[1] != status_finished)
    Send(PIMP_TRANSACTION_UNUSUAL_FACTORY(mID), GetParty(player));
}

void ForcedTransaction::Bankrupt(Player* player)
{
  int party = GetParty(player);
  if (party == 0) {
    BilateralTransaction::Bankrupt(player);
    return;
  }
  if (player->GetSpendableAssets() < mAmount) {
    // check there are no outstanding transactions with lower IDs
    Transaction* transaction = mBoard->GetLowestTransactionInvolvingPlayer(player);
    if (transaction == this) {
      Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), 0);
      Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID), 1);
      End();
      mBoard->PlayerClaimingBankruptcy(mPlayer[1], mPlayer[0], mAmount);
    } else {
      Send(PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_FACTORY(mID, transaction->GetID()), 1);
    }
  } else {
    Send(PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_FACTORY(mID, 0), 1);
  }
}

void ForcedTransaction::Cancel(Player* player)
{
  int party = GetParty(player);
  if (party == 1) {
    Send(PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED_FACTORY(mID), GetParty(player));
    return;
  }
  BilateralTransaction::Cancel(player);
}


RentTransaction::RentTransaction(Board* board, int id, Player* creditor, Player* debtor, Property* property, int amount) : ForcedTransaction(board, id, creditor, debtor, amount),
                                                                                                                            mProperty(property)
{
}

void RentTransaction::RequestStateHead(Player* player)
{
  int party = GetParty(player);
  Send(PIMP_TRANSACTION_RENT_REQUESTED_FACTORY(mID, mPlayer[1-party]->GetID(),
                                               mProperty->GetID(), mPlayer[0]->GetID(),
                                               mAmount), party);
}


CardTransaction::CardTransaction(Board* board, Player* creditor, Player* debtor, Card* card, int amount) : ForcedTransaction(board, creditor, debtor, amount),
                                                                                                           mCard(card)
{
}

void CardTransaction::RequestStateHead(Player* player)
{
  int party = GetParty(player);
  Send(PIMP_TRANSACTION_CARD_REQUESTED_FACTORY(mID, mPlayer[1-party]->GetID(),
                                               mCard->GetID(), mPlayer[0]->GetID(),
                                               mAmount), party);
}


BankTransaction::BankTransaction(Board* board, Pot* pot, Player* player, int amount) : Transaction(board),
                                                                                       mPot(pot),
                                                                                       mPlayer(player),
                                                                                       mAmount(amount),
                                                                                       mCash(0),
                                                                                       mStatus(status_setup),
                                                                                       mEscrow(0)
{ }

BankTransaction::BankTransaction(Board* board, Player* player, int amount) : Transaction(board),
                                                                             mPot(NULL),
                                                                             mPlayer(player),
                                                                             mAmount(amount),
                                                                             mCash(0),
                                                                             mStatus(status_setup),
                                                                             mEscrow(0)
{ }

bool BankTransaction::InvolvesPlayer(Player* player)
{
  return player == mPlayer || player == BANK;
}

void BankTransaction::End()
{
  Transaction::End();
  if (mEscrow)
    mBoard->Transfer(BANK, mPlayer, mEscrow, mID);
}

int BankTransaction::GetNetSpendableAssetDeltaForPlayer(Player* player)
{
  // cash
  int amount = -mCash;

  // property
  std::list<Property*>::iterator p;
  for (p = mProperties.begin(); p != mProperties.end(); ++p)
    amount -= (*p)->GetSpendableWorth();

  return amount;
}

int BankTransaction::GetLiabilityForPlayer(Player* player) {
  return mAmount;
}

void BankTransaction::AddToEscrow(Player* player, int amount)
{
  mEscrow += amount;
}

void BankTransaction::TransferNotification(Player* from, Player* to)
{
  if (from != mPlayer || mEnded)
    return;
  Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID));
  if (to) {
    mPlayer = to;
    RequestState(to);
  } else {
    End();
  }
}

void BankTransaction::TransferNotification(Player* from, Player* to, int amount)
{
  if (from != mPlayer || mEnded)
    return;
  if (!mPlayer->HasCash(mCash)) {
    EnsureOpen(mPlayer);
    Send(PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_FACTORY(mID, mCash));
    mCash = mPlayer->GetCash();
    Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash));
  }
}

void BankTransaction::TransferNotification(Player* from, Player* to, Property* property)
{
  if (from != mPlayer || mEnded)
    return;
  RemoveProperty(from, property);
}

void BankTransaction::TransferNotification(Player* from, Player* to, Card* card)
{
  if (from != mPlayer || mEnded)
    return;
  RemoveCard(from, card);
}

void BankTransaction::Begin()
{
  RequestStateHead(mPlayer);
  Setup();
  Send(PIMP_TRANSACTION_OTHER_FINISHED_FACTORY(mID));
  if (Satisfied())
    Finish(mPlayer);
}

void BankTransaction::Setup()
{
  SetCash(mPlayer, mAmount);
}

void BankTransaction::EnsureOpen(Player* player)
{
  if (mStatus > status_setup) {
    Send(PIMP_TRANSACTION_REOPENED_FACTORY(mID));
    mStatus = status_setup;
    Unusual();
  }
}

void BankTransaction::Finalise()
{
  Send(PIMP_TRANSACTION_FINALISED_FACTORY(mID));
  End();
  mBoard->Transfer(mPlayer, BANK, mCash);
  if (mPot)
    mPot += mCash;
  std::list<Property*>::iterator p;
  for (p = mProperties.begin(); p != mProperties.end(); ++p) {
    mBoard->Transfer(mPlayer, BANK, (*p));
  }
  std::list<Card*>::iterator c;
  for (c = mCards.begin(); c != mCards.end(); ++c) {
    mBoard->Transfer(mPlayer, BANK, (*c));
  }
}

void BankTransaction::Unusual()
{
  Send(PIMP_TRANSACTION_UNUSUAL_FACTORY(mID));
}

void BankTransaction::SetCash(Player* player, int amount)
{
  EnsureOpen(mPlayer);
  if (player->HasCash(amount)) {
    mCash = amount;
  } else {
    Send(PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_FACTORY(mID, amount));
  }
  Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash));
}

void BankTransaction::AddProperty(Player* player, Property* property)
{
  if (!player->HasProperty(property)) {
    Send(PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED_FACTORY(mID, property->GetID()));
    return;
  }
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    if ((*i) == property) {
      return;
    }
  }
  EnsureOpen(mPlayer);
  mProperties.push_back(property);
  Send(PIMP_TRANSACTION_PROPERTY_ADDED_FACTORY(mID, property->GetID()));
}

void BankTransaction::RemoveProperty(Player* player, Property* property)
{
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    if ((*i) == property) {
      EnsureOpen(mPlayer);
      mProperties.erase(i);
      Send(PIMP_TRANSACTION_PROPERTY_REMOVED_FACTORY(mID, property->GetID()));
      return;
    }
  }
}

void BankTransaction::AddCard(Player* player, Card* card)
{
  if (!player->HasCard(card)) {
    Send(PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED_FACTORY(mID, card->GetID()));
    return;
  }
  std::list<Card*>::iterator i;
  for (i = mCards.begin(); i != mCards.end(); ++i) {
    if ((*i) == card) {
      return;
    }
  }
  EnsureOpen(mPlayer);
  mCards.push_back(card);
  Send(PIMP_TRANSACTION_CARD_ADDED_FACTORY(mID, card->GetID()));
}

void BankTransaction::RemoveCard(Player* player, Card* card)
{
  std::list<Card*>::iterator i;
  for (i = mCards.begin(); i != mCards.end(); ++i) {
    if ((*i) == card) {
      EnsureOpen(mPlayer);
      mCards.erase(i);
      Send(PIMP_TRANSACTION_CARD_REMOVED_FACTORY(mID, card->GetID()));
      return;
    }
  }
}

void BankTransaction::Finish(Player* player)
{
  if (mStatus == status_setup) {
    mStatus = status_finished;
    Send(PIMP_TRANSACTION_FINISHED_FACTORY(mID));
    if (Satisfied()) {
      Send(PIMP_TRANSACTION_OTHER_AGREED_FACTORY(mID));
    } else {
      Send(PIMP_ERROR_TRANSACTION_NOT_SUITABLE_FACTORY(mID, mAmount));
    }
  }
}

void BankTransaction::Reopen(Player* player)
{
  if (mStatus == status_setup)
    throw UnexpectedMessageError();
  Send(PIMP_TRANSACTION_REOPENED_FACTORY(mID));
  mStatus = status_setup;
}

void BankTransaction::Agree(Player* player)
{
  if (mStatus != status_finished)
    throw UnexpectedMessageError();

  // check for other transactions
  Transaction* transaction = mBoard->GetTransactionBlockingAgreement(player, this);
  if (transaction) {
    Send(PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH_FACTORY(mID, transaction->GetID()));
    return;
  }

  int houses = 0;
  std::list<Property*>::iterator p;
  for (p = mProperties.begin(); p != mProperties.end(); ++p) {
    if (!(*p)->IsTransferable()) {
      ++houses;
      Send(PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE_FACTORY(mID, (*p)->GetID()));
    }
  }
  if (houses)
    return;

  mStatus = status_agreed;
  Send(PIMP_TRANSACTION_AGREED_FACTORY(mID));
  if (Satisfied()) {
    Finalise();
  }
}

void BankTransaction::Bankrupt(Player* player)
{
  if (player->GetSpendableAssets() < mAmount) {
    // check there are no outstanding transactions with lower IDs
    Transaction* transaction = mBoard->GetLowestTransactionInvolvingPlayer(player);
    if (transaction == this) {
      Send(PIMP_TRANSACTION_CANCELLED_FACTORY(mID));
      End();
      mBoard->PlayerClaimingBankruptcy(player, BANK, mAmount);
    } else {
      Send(PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_FACTORY(mID, transaction->GetID()));
    }
  } else {
    Send(PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_FACTORY(mID, 0));
  }
}

void BankTransaction::Cancel(Player* player)
{
  Send(PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED_FACTORY(mID));
}

void BankTransaction::RequestStateHead(Player* player)
{
  Send(PIMP_TRANSACTION_BANK_REQUESTED_FACTORY(mID, mAmount));
}

void BankTransaction::RequestStateBody(Player* player)
{
  Send(PIMP_TRANSACTION_OTHER_FINISHED_FACTORY(mID));

  Send(PIMP_TRANSACTION_CASH_SET_FACTORY(mID, mCash));

  std::list<Property*>::iterator p;
  for (p = mProperties.begin(); p != mProperties.end(); ++p) {
    Send(PIMP_TRANSACTION_PROPERTY_ADDED_FACTORY(mID, (*p)->GetID()));
  }

  std::list<Card*>::iterator c;
  for (c = mCards.begin(); c != mCards.end(); ++c) {
    Send(PIMP_TRANSACTION_CARD_ADDED_FACTORY(mID, (*c)->GetID()));
  }

  if (mStatus >= status_finished) {
    Send(PIMP_TRANSACTION_FINISHED_FACTORY(mID));
    if (mStatus == status_agreed)
      Send(PIMP_TRANSACTION_AGREED_FACTORY(mID));
    if (Satisfied())
      Send(PIMP_TRANSACTION_OTHER_AGREED_FACTORY(mID));
    else
      Unusual();
  } else {
    Unusual();
  }

}


void BankTransaction::Send(Message* message)
{
  mBoard->Send(message, mPlayer->GetID());
}

bool BankTransaction::Satisfied()
{
  return (mCash == mAmount)
    && (mProperties.begin() == mProperties.end())
    && (mCards.begin() == mCards.end());
}


CardBankTransaction::CardBankTransaction(Board* board, Pot* pot, Player* player, Card* card, int amount) : BankTransaction(board, pot, player, amount),
                                                                                                           mCard(card)
{ }

void CardBankTransaction::RequestStateHead(Player* player)
{
  Send(PIMP_TRANSACTION_CARD_REQUESTED_FACTORY(mID, 0 /*BANK->GetID()*/,
                                               mCard->GetID(), 0 /*BANK->GetID()*/,
                                               mAmount));
}


SquareTransaction::SquareTransaction(Board* board, Pot* pot, Player* player, Square* square, int amount) : BankTransaction(board, pot, player, amount),
                                                                                                           mSquare(square)
{ }

void SquareTransaction::Finalise()
{
  BankTransaction::Finalise();
  if (mCash)
    mBoard->Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(mPlayer->GetID(), mSquare->GetID(), mCash));
  std::list<Card*>::iterator c;
  for (c = mCards.begin(); c != mCards.end(); ++c) {
    mBoard->Broadcast(PIMP_SQUARE_TAKES_CARD_FACTORY(mPlayer->GetID(), mSquare->GetID(), (*c)->GetID()));
  }
}

void SquareTransaction::RequestStateHead(Player* player)
{
  Send(PIMP_TRANSACTION_SQUARE_REQUESTED_FACTORY(mID, mSquare->GetID(), mAmount));
}


JailTransaction::JailTransaction(Board* board, Pot* pot, Player* player, JustVisitingSquare* square, int amount, int cardType) : SquareTransaction(board, pot, player, square, amount),
                                                                                                                                 mCardType(cardType)
{ }

int JailTransaction::GetLiabilityForPlayer(Player* player) {
  return 0; // because you can pay for this using a card, which is worth $0
}

void JailTransaction::Finalise()
{
  SquareTransaction::Finalise();
  static_cast<JustVisitingSquare*>(mSquare)->PlayerSetFree(mPlayer);
}

void JailTransaction::RequestStateHead(Player* player)
{
  Send(PIMP_TRANSACTION_JAIL_REQUESTED_FACTORY(mID, mAmount));
}

bool JailTransaction::Satisfied()
{
  if ((mCash == 0) && // if: $0
      (mProperties.begin() == mProperties.end())) { // no properties
    std::list<Card*>::iterator c = mCards.begin();
    if (c != mCards.end()) { // there is at least one card
      if ((*c)->GetType() == mCardType) { // it's of the right type
        ++c;
        if (c == mCards.end()) { // _and_ it's the last one
          return true; // then we're all set.
        }
      }
    }
  }
  // else:
  return SquareTransaction::Satisfied();
}


PostAuctionTransaction::PostAuctionTransaction(Board* board, Player* player, Property* property, int amount) : BankTransaction(board, player, amount),
                                                                                                               mProperty(property)
{
}

int PostAuctionTransaction::GetNetSpendableAssetDeltaForPlayer(Player* player)
{
  int amount = BankTransaction::GetNetSpendableAssetDeltaForPlayer(player);
  amount += mProperty->GetSpendableWorth();
  return amount;
}

void PostAuctionTransaction::Setup()
{
  Send(PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_FACTORY(mID, mProperty->GetID()));
  BankTransaction::Setup();
}

void PostAuctionTransaction::RequestStateBody(Player* player)
{
  Send(PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_FACTORY(mID, mProperty->GetID()));
  BankTransaction::RequestStateBody(player);
}

void PostAuctionTransaction::Finalise()
{
  mBoard->Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(mPlayer->GetID(),
                                                   mProperty->GetSquare()->GetID(),
                                                   mCash));
  BankTransaction::Finalise();
  mBoard->Transfer(BANK, mPlayer, mProperty);
}
