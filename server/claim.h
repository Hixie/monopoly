class Claim;

#ifndef INCLUDED_CLAIM_H
#define INCLUDED_CLAIM_H

#include "card.h"
#include "player.h"
#include "board.h"

class Claim {
 public:
  Claim(Board* board);
  virtual ~Claim();
  virtual bool Dispatch(Message* message) = 0;
  virtual bool IsAgainst(Player* player) = 0;
  virtual bool TransferNotification(Player* from, Player* to) = 0;
    // returns whether still relevant
 protected:
  Board* mBoard;
  int mID;
};

class RentClaim : public Claim {
 public:
  RentClaim(Board* board, Player* debtor, Player* creditor, Property* property, int rent);
  bool Dispatch(Message* message);
  bool IsAgainst(Player* player) { return mDebtor == player; }
  bool TransferNotification(Player* from, Player* to);
 protected:
  Player* mDebtor;
  Player* mCreditor;
  Property* mProperty;
  int mRent;
};

class GoClaim : public Claim {
 public:
  GoClaim(Board* board, Player* claimant, Square* square, int salary);
  bool Dispatch(Message* message);
  bool IsAgainst(Player* player) { return false; }
  bool TransferNotification(Player* from, Player* to);
 protected:
  Player* mClaimant;
  Square* mSquare;
  int mSalary;
};

#endif
