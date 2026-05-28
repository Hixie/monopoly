class Transaction;

#ifndef INCLUDED_TRANSACTION_H
#define INCLUDED_TRANSACTION_H

#include "player.h"
#include "board.h"
#include "player.h"
#include "property.h"
#include "card.h"
#include "message.h"

#define PIMP_TRANSACTION_GENERIC_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)

// The instantiable classes available in this module are:
//   TradeTransaction -- two players trading willingly
//   RentTransaction -- two players forced to trade by a rent call
//   CardTransaction -- two players forced to trade by a card
//   BankTransaction -- a player forced to trade with the bank
//   CardBankTransaction -- a player forced to trade with the bank by a card
//   PostAuctionTransaction -- a player forced to pay cash for a property
//   JailTransaction -- a player forced to trade with the bank by a jail

enum TransactionStatuses {
  status_setup,
  status_finished,
  status_agreed
};

class Transaction {
 public:
  Transaction(Board* board);
  Transaction(Board* board, int id);
  virtual ~Transaction();
  int GetID() { return mID; }
  static int NewID();

  virtual bool InvolvesPlayer(Player* player) = 0;
  virtual void Dispatch(Message* message);

  // must not assume InvolvesPlayer(from or to) == true
  virtual void TransferNotification(Player* from, Player* to) = 0;
  virtual void TransferNotification(Player* from, Player* to, int amount) = 0;
  virtual void TransferNotification(Player* from, Player* to, Property* property) = 0;
  virtual void TransferNotification(Player* from, Player* to, Card* card) = 0;

  virtual void Begin() = 0;
  virtual void EnsureOpen(Player* player) = 0; // may assume InvolvesPlayer(player) == true
  virtual void Finalise() = 0;
  virtual void Unusual() = 0;
  virtual void End() { mEnded = true; };

  virtual Player* GetDebtorPlayer() = 0;
  virtual int GetLiabilityForPlayer(Player* player) = 0;
  virtual int GetNetSpendableAssetDeltaForPlayer(Player* player) = 0;
  virtual void AddToEscrow(Player* player, int amount) = 0;

  // may all assume InvolvesPlayer(player) == true
  virtual void SetCash(Player* player, int amount) = 0;
  virtual void AddProperty(Player* player, Property* property) = 0;
  virtual void RemoveProperty(Player* player, Property* property) = 0;
  virtual void AddCard(Player* player, Card* card) = 0;
  virtual void RemoveCard(Player* player, Card* card) = 0;
  virtual void Finish(Player* player) = 0;
  virtual void Reopen(Player* player) = 0;
  virtual void Agree(Player* player) = 0;
  virtual void Bankrupt(Player* player) = 0;
  virtual void Cancel(Player* player) = 0;

  // may not assume InvolvesPlayer(player) == true
  // in fact it should do nothing if it is false
  void RequestState(Player* player);
  // may all assume InvolvesPlayer(player) == true
  virtual void RequestStateHead(Player* player) = 0;
  virtual void RequestStateBody(Player* player) = 0;
    // PIMP_TRANSACTION_TRADE_REQUESTED
    // PIMP_TRANSACTION_RENT_REQUESTED
    // PIMP_TRANSACTION_CARD_REQUESTED
    // PIMP_TRANSACTION_BANK_REQUESTED
    // PIMP_TRANSACTION_JAIL_REQUESTED
    // PIMP_TRANSACTION_CASH_SET
    // PIMP_TRANSACTION_OTHER_CASH_SET
    // PIMP_TRANSACTION_PROPERTY_ADDED
    // PIMP_TRANSACTION_OTHER_PROPERTY_ADDED
    // PIMP_TRANSACTION_CARD_ADDED
    // PIMP_TRANSACTION_OTHER_CARD_ADDED
    // PIMP_TRANSACTION_FINISHED
    // PIMP_TRANSACTION_OTHER_FINISHED
    // PIMP_TRANSACTION_AGREED
    // PIMP_TRANSACTION_OTHER_AGREED

  int GetEnded() { return mEnded; }

 protected:
  int mID;
  static int gID;
  Board* mBoard;
  bool mEnded;
};

class BilateralTransaction : public Transaction {
 public:
  // the caller must ensure player0 != player1
  BilateralTransaction(Board* board, Player* player0, Player* player1);
  BilateralTransaction(Board* board, int id, Player* player0, Player* player1);
  void Reset(Player* player0, Player* player1);

  bool InvolvesPlayer(Player* player);

  void TransferNotification(Player* from, Player* to);
  void TransferNotification(Player* from, Player* to, int amount);
  void TransferNotification(Player* from, Player* to, Property* property);
  void TransferNotification(Player* from, Player* to, Card* card);

  void Begin();
  void EnsureOpen(Player* player);
  void Finalise();
  void Unusual();
  void End();

  Player* GetDebtorPlayer() { return mPlayer[1]; };
  int GetNetSpendableAssetDeltaForPlayer(Player* player);
  void AddToEscrow(Player* player, int amount);

  void SetCash(Player* player, int amount);
  void AddProperty(Player* player, Property* property);
  void RemoveProperty(Player* player, Property* property);
  void AddCard(Player* player, Card* card);
  void RemoveCard(Player* player, Card* card);
  void Finish(Player* player);
  void Reopen(Player* player);
  void Agree(Player* player);
  void Bankrupt(Player* player);
  void Cancel(Player* player);

  void RequestStateBody(Player* player);

  void Send(Message* message, int party);

 protected:
  int GetParty(Player* player); // assumes InvolvesPlayer(player) == true

  Player* mPlayer[2];
  TransactionStatuses mStatus[2];

  int mCash[2];
  std::list<Property*> mProperties[2];
  std::list<Card*> mCards[2];

  int mEscrow[2];
};

class TradeTransaction : public BilateralTransaction {
 public:
  TradeTransaction(Board* board, Player* player0, Player* player1);
  void RequestStateHead(Player* player);
  int GetLiabilityForPlayer(Player* player);
};

class ForcedTransaction : public BilateralTransaction {
 public:
  ForcedTransaction(Board* board, Player* creditor, Player* debtor, int amount);
  ForcedTransaction(Board* board, int id, Player* creditor, Player* debtor, int amount);
  void Begin();
  void SetCash(Player* player, int amount);
  void AddProperty(Player* player, Property* property);
  void AddCard(Player* player, Card* card);
  void RequestStateBody(Player* player);
  void Bankrupt(Player* player);
  void Cancel(Player* player);
  int GetLiabilityForPlayer(Player* player);
 protected:
  int mAmount;
};

class RentTransaction : public ForcedTransaction {
 public:
  RentTransaction(Board* board, Player* creditor, Player* debtor, Property* property, int amount);
  RentTransaction(Board* board, int id, Player* creditor, Player* debtor, Property* property, int amount);
  void RequestStateHead(Player* player);
 protected:
  Property* mProperty;
};

class CardTransaction : public ForcedTransaction {
 public:
  CardTransaction(Board* board, Player* creditor, Player* debtor, Card* card, int amount);
  void RequestStateHead(Player* player);
 protected:
  Card* mCard;
};

class BankTransaction : public Transaction {
 public:
  BankTransaction(Board* board, Pot* pot, Player* player, int amount);
  BankTransaction(Board* board, Player* player, int amount);

  void End();

  bool InvolvesPlayer(Player* player);
  Player* GetDebtorPlayer() { return mPlayer; };
  int GetLiabilityForPlayer(Player* player);
  int GetNetSpendableAssetDeltaForPlayer(Player* player);
  void AddToEscrow(Player* player, int amount);

  void TransferNotification(Player* from, Player* to);
  void TransferNotification(Player* from, Player* to, int amount);
  void TransferNotification(Player* from, Player* to, Property* property);
  void TransferNotification(Player* from, Player* to, Card* card);

  void Begin();
  virtual void Setup();
  void EnsureOpen(Player* player);
  void Finalise();
  void Unusual();

  void SetCash(Player* player, int amount);
  void AddProperty(Player* player, Property* property);
  void RemoveProperty(Player* player, Property* property);
  void AddCard(Player* player, Card* card);
  void RemoveCard(Player* player, Card* card);
  void Finish(Player* player);
  void Reopen(Player* player);
  void Agree(Player* player);
  void Bankrupt(Player* player);
  void Cancel(Player* player);

  void RequestStateHead(Player* player);
  void RequestStateBody(Player* player);

  void Send(Message* message);

  virtual bool Satisfied();

 protected:
  Player* mPlayer;
  Pot* mPot;
  TransactionStatuses mStatus;
  int mAmount;

  int mCash;
  std::list<Property*> mProperties;
  std::list<Card*> mCards;

  int mEscrow;
};

class CardBankTransaction : public BankTransaction {
 public:
  CardBankTransaction(Board* board, Pot* pot, Player* player, Card* card, int amount);
  void RequestStateHead(Player* player);
 protected:
  Card* mCard;
};

class SquareTransaction : public BankTransaction {
 public:
  SquareTransaction(Board* board, Pot* pot, Player* player, Square* square, int amount);
  void Finalise();
  void RequestStateHead(Player* player);
 protected:
  Square* mSquare;
};

class JailTransaction : public SquareTransaction {
 public:
  JailTransaction(Board* board, Pot* pot, Player* player, JustVisitingSquare* square, int amount, int cardType);
  int GetLiabilityForPlayer(Player* player);
  void Finalise();
  void RequestStateHead(Player* player);
  bool Satisfied();
 protected:
  int mCardType;
};

class PostAuctionTransaction : public BankTransaction {
 public:
  PostAuctionTransaction(Board* board, Player* player, Property* property, int amount);
  int GetNetSpendableAssetDeltaForPlayer(Player* player);
  void Setup();
  void RequestStateBody(Player* player);
  void Finalise();
 protected:
  Property* mProperty;
};

#endif

