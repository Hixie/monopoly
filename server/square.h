class Square;
class JustVisitingSquare;

#ifndef INCLUDED_SQUARE_H
#define INCLUDED_SQUARE_H

#include "board.h"
#include "card.h"
#include "player.h"
#include "property.h"
#include "message.h"
#include "exceptions.h"

#define SQ_TYPE_GO              0
#define SQ_TYPE_PROPERTY        1
#define SQ_TYPE_FREE_PARKING    2
#define SQ_TYPE_CHANCE          3
#define SQ_TYPE_COMMUNITY_CHEST 4
#define SQ_TYPE_GO_TO_JAIL      5
#define SQ_TYPE_JUST_VISITING   6
#define SQ_TYPE_INCOME_TAX      7
#define SQ_TYPE_LUXURY_TAX      8

// SQ_TYPE_CHANCE and SQ_TYPE_COMMUNITY_CHEST aren't used by the main
// board code, they are both equivalent and fall under the following
// type. The two types above are used only for generating the board.
#define SQ_TYPE_CARD            3

class Square {
 public:
  Square(Board* board, int id);
  int GetID() { return mID; }
  virtual void PassedBy(Player* player, bool forwards);
  virtual void LandedOn(Player* player, bool forwards, int diceTotal);
    // returns status_idle if the board shouldn't be blocked
  virtual int GetType() = 0;
  virtual void Dispatch(Player* player, Message* message) { throw UnexpectedMessageError(); }
    // only called if player is the CurrentPlayer
    // returns status_idle if the board should be unblocked
  virtual void RequestState(Player* player) {}
 protected:
  Board* mBoard;
  int mID;
};

class GoSquare : public Square {
 public:
  GoSquare(Board* board, int id);
  void PassedBy(Player* player, bool forwards);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_GO; }
};

class PropertySquare : public Square {
 public:
  PropertySquare(Board* board, int id, Property* property);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_PROPERTY; }
  int GetPropertyType();
 private:
  Property* mProperty;
}
;
class CardSquare : public Square {
 public:
  CardSquare(Board* board, int id, std::list<Card*>* cards);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_CARD; }
  void Dispatch(Player* player, Message* message);
  void RequestState(Player* player);
  void SetLastCard(Card* card) { mLastCard = card; }
 private:
  std::list<Card*>* mCards;
  Card* mLastCard;
};

class IncomeTaxSquare : public Square {
 public:
  IncomeTaxSquare(Board* board, int id);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_INCOME_TAX; }
  void Dispatch(Player* player, Message* message);
  void RequestState(Player* player);
};

class JustVisitingSquare : public Square {
 public:
  JustVisitingSquare(Board* board, int id, Pot* pot);
  int GetType() { return SQ_TYPE_JUST_VISITING; }

  void PlayerSentToJail(Player* player);
  void PlayerWantsToPay(Player* player);
  void PlayerSetFree(Player* player);

 private:
  Pot* mPot;
};

class FreeParkingSquare : public Square {
 public:
  FreeParkingSquare(Board* board, int id, Pot* pot);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_FREE_PARKING; }
 private:
  Pot* mPot;
};

class GoToJailSquare : public Square {
 public:
  GoToJailSquare(Board* board, int id);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_GO_TO_JAIL; }
};

class LuxuryTaxSquare : public Square {
 public:
  LuxuryTaxSquare(Board* board, int id);
  void LandedOn(Player* player, bool forwards, int diceTotal);
  int GetType() { return SQ_TYPE_LUXURY_TAX; }
};

#endif
