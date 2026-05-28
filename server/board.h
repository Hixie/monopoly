class Board;
class Pot;

#ifndef INCLUDED_BOARD_H
#define INCLUDED_BOARD_H

#include <vector>
#include <stdexcept>
#include <climits>
#include "player.h"
#include "property.h"
#include "card.h"
#include "square.h"
#include "transaction.h"
#include "claim.h"
#include "message.h"

#define NUM_PROPERTIES 28
#define NUM_CARDS 32
#define NUM_SQUARES 40
#define MAX_PLAYERS 255

#define BANK NULL

#define overrideEscrow 0

// XXX these should all be in the board file really:
#define STARTING_CASH 1500
#define POT_DEFAULT_AMOUNT 100
#define GO_AMOUNT 200
#define BAIL 50
#define LUXURY_TAX 75
#define INCOME_TAX 200
#define INCOME_TAX_FRACTION 0.10
#define TOTAL_HOUSES_AVAILABLE 32
#define TOTAL_HOTELS_AVAILABLE 12
#define MAX_JAIL_TURNS 3

class Pot {
 public:
  Pot(Board* board) : mBoard(board),
                      mPot(POT_DEFAULT_AMOUNT),
                      Enabled(true) {}
  ~Pot() {}
  void Reset() { mPot = POT_DEFAULT_AMOUNT; Report(); }
  void operator+=(int delta) { mPot += delta; Report(); }
  void Report();
  int GetPot() { return Enabled ? mPot : 0; }
  bool Enabled;
 private:
  Board* mBoard;
  int mPot;
};

class Board {
public:
  Board();
  ~Board();

  void OverrideSettings(bool potEnabled,
                        bool needToPassGoToBuy,
                        bool doubleGoMoneyEnabled,
                        bool bankruptcyTransfersToPlayer);

  Player* GetPlayer(int id);
  Player* GetPlayer(Player* player, int delta);
  int GetPlayingPlayersCount();
  Player* GetCurrentPlayer() { return mCurrentPlayer; }

  Property* GetProperty(int id);
  void PurchaseHouses(Player* buyer, int count, Property** properties, int* amountHouses, int* amountHotels);
  int GetNumOfGroup(int group);  
  void SellProperty(Property* property);
  void AuctionProperty(Property* property);
  void CheckAuction();

  Card* GetCard(int id);

  Square* GetSquare(int id);
  Square* GetSquare(Square* start, int delta);
  Square* GetNextPropertySquareOfType(Square* start, int type);
  Square* GetLastPropertySquareOfType(Square* start, int type);
  JustVisitingSquare* GetJail() { return mJail; };

  Transaction* GetTransaction(int id);
  Transaction* GetLowestTransactionInvolvingPlayer(Player* player);
  Transaction* GetUncancellableTransactionInvolvingPlayer(Player* player, int id);
  Transaction* GetTransactionBlockingAgreement(Player* player, Transaction* transaction);
  Transaction* GetTransactionBlockingAgreement(Player* player, int deltaWorth);
  void AddTransaction(Transaction* transaction, bool blockPlay);
  void RemoveTransaction(Transaction* transaction);

  void AddClaim(Claim* claim);
  void RemoveClaim(Claim* claim);

  void ChargePlayer(Player* player, Square* square, int amount);
  void ChargeAllPlayers(Player* player, Card* card, int amount);

  void Block(Square* square) { mStatus = status_squareBlocking; }
  void Unblock() { mStatus = status_idle; }
 
  void NextTurn();
  void NextTurn(Player* player);

  void Transfer(Player* from, Player* to);
  void Transfer(Player* from, Player* to, int amount, int transactionID = INT_MAX);
  void Transfer(Player* from, Player* to, Property* property, bool immuneToFees = false);
  void Transfer(Player* from, Player* to, Card* card);
  // the MovePlayer methods return true if they have requested
  // further input from the user:
  void MovePlayer(Player* player, int diceTotal) { return MovePlayer(player, diceTotal, diceTotal); }
  void MovePlayer(Player* player, int delta, int diceTotal);
  void MovePlayer(Player* player, Square* target, bool forwards, int diceTotal);
  void MovePlayerDirectly(Player* player, Square* target, bool forwards, int diceTotal);
  void GoToJail(Player* player);

  void ClearPendingClaims();
  bool AreOutstandingClaimsAgainst(Player* player);

  void PlayerClaimingBankruptcy(Player* player, Player* creditor, int amount);

  void Dispatch(Message* message);
  void DispatchWelcomeUser(Message* message);
  void DispatchObserverBecamePlayer(Message* message);
  void DispatchRequestState(Message* message);
  void DispatchKick(Message* message);
  void DispatchTransfer(Message* message);
  void DispatchThrowDice(Message* message);
  void DispatchJailChoice(Message* message);
  void DispatchTransaction(Message* message);
  void DispatchTradeRequest(Message* message);
  void DispatchClaims(Message* message);
  void DispatchPropertySale(Message* message);
  void DispatchPropertyAuction(Message* message);
  void DispatchPurchaseHouses(Message* message);
  void DispatchMortgages(Message* message);
  void DispatchSquare(Message* message);

  void UnexpectedMessage(Message* message); // creates and sends a response
  void InvalidPayload(Message* message); // creates and sends a response
  void Send(Message* message); // takes ownership of the message
  void Send(Message* message, int userID); // sets the message player id first
  void Broadcast(Message* message);
  Message* PopQueue();

  void RollDice(Player* player, int& die1, int& die2);

  void TellPlayerState(Player* player);

  bool NeedToPassGoToBuy() { return mNeedToPassGoToBuy; }
  bool IsDoubleGoMoneyEnabled() { return mDoubleGoMoneyEnabled; }
  bool IsGameOver() { return mGameOver; }

 protected:
  // Add* and Remove* must only be called by Transfer(), with the
  // exception of AddPlayer(), which may only be called by
  // DispatchWelcomeUser() in response to certain messages.
  void AddPlayer(Player* player);
  void AddCash(int amount) { mCash += amount; }
  void RemoveCash(int amount) { mCash -= amount; }
  int GetCash() { return mCash; }
  bool HasProperty(Property* property);
  void AddCard(Card* card);
  void RemoveCard(Card* card);
  bool HasCard(Card* card);

 private:
  Property* mProperties[NUM_PROPERTIES];
  Card* mCards[NUM_CARDS];
  Square* mSquares[NUM_SQUARES];
  JustVisitingSquare* mJail;
  std::list<Card*> mChanceCards;
  std::list<Card*> mCommunityChestCards;
  std::vector<Player*> mPlayers;
  std::list<Message*> mMessageQueue; // outbound messages
  int mAvailableHouses, mAvailableHotels, mCash;
  Pot mPot;
  int mKickVotes;
  enum {
    status_idle,
    status_startOfTurn,
    status_playAgain,
    status_transactionBlocking,
    status_propertySale,
    status_propertyAuction,
    status_squareBlocking
  } mStatus;
  Player* mCurrentPlayer;
  int mDoubles;
  bool mCanBuyHouses;
  bool mGameOver;

  // Transactions
  std::list<Transaction*> mTransactions;
  std::list<Claim*> mClaims;
  Transaction* mBlockingTransaction;

  // Sales and Auctions
  Property* mPropertyOnSale;
  int mBid;
  Player* mLeadingBidder;
  int mNoBids;

  // Options
  bool mNeedToPassGoToBuy;
  bool mDoubleGoMoneyEnabled;
  bool mBankruptcyTransfersToPlayer;
};

#endif
