class Card;

#ifndef INCLUDED_CARD_H
#define INCLUDED_CARD_H

#include "square.h"
#include "player.h"
#include "exceptions.h"
#include "message.h"

#define CARD_TYPE_NOTHING                   0
#define CARD_TYPE_GET_OUT_OF_JAIL           1
#define CARD_TYPE_COLLECT_FROM_BANK         2
#define CARD_TYPE_PAY_TO_BANK               3
#define CARD_TYPE_COLLECT_FROM_EACH_PLAYER  4
#define CARD_TYPE_PAY_TO_EACH_PLAYER        5
#define CARD_TYPE_ADVANCE_TO_SQUARE         6
#define CARD_TYPE_GO_BACK_TO_SQUARE         7
#define CARD_TYPE_ADVANCE_N_SQUARES         8
#define CARD_TYPE_GO_BACK_N_SQUARES         9
#define CARD_TYPE_ADVANCE_TO_NEXT           10
#define CARD_TYPE_GO_BACK_TO_NEXT           11
#define CARD_TYPE_GO_TO_JAIL                12
#define CARD_TYPE_PAY_OR_TAKE_OTHER_PILE    13
#define CARD_TYPE_STREET_REPAIRS            14

class Card {
 public:
  Card(Board* board, int id, std::list<Card*>* pile);
  ~Card();
  int GetID() { return mID; }

  Player* GetOwner() { return mOwner; }
  void SetOwner(Player* owner) { mOwner = owner; }

  std::list<Card*>* GetPile() { return mPile; };
  virtual int GetType() { return CARD_TYPE_NOTHING; }
  virtual void Handle(Player* player, Square* square, int diceTotal);
  virtual void Dispatch(Player* player, Square* square, Message* message) { throw UnexpectedMessageError(); }
  virtual void RequestState(Player* player) { }

 protected:
  int mID;
  Board* mBoard;
  Player* mOwner;
  std::list<Card*>* mPile;
};

class GetOutOfJailCard : public Card {
 public:
  GetOutOfJailCard(Board* board, int id, std::list<Card*>* pile);
  int GetType() { return CARD_TYPE_GET_OUT_OF_JAIL; }
  void Handle(Player* player, Square* square, int diceTotal);
};

class CollectFromBankCard : public Card {
 public:
  CollectFromBankCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_COLLECT_FROM_BANK; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class PayToBankCard : public Card {
 public:
  PayToBankCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_PAY_TO_BANK; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class CollectFromEachPlayerCard : public Card {
 public:
  CollectFromEachPlayerCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_COLLECT_FROM_EACH_PLAYER; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class PayToEachPlayerCard : public Card {
 public:
  PayToEachPlayerCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_PAY_TO_EACH_PLAYER; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class AdvanceToSquareCard : public Card {
 public:
  AdvanceToSquareCard(Board* board, int id, std::list<Card*>* pile, Square* square);
  int GetType() { return CARD_TYPE_ADVANCE_TO_SQUARE; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  Square* mSquare;
};

class GoBackToSquareCard : public Card {
 public:
  GoBackToSquareCard(Board* board, int id, std::list<Card*>* pile, Square* square);
  int GetType() { return CARD_TYPE_GO_BACK_TO_SQUARE; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  Square* mSquare;
};

class AdvanceNSquaresCard : public Card {
 public:
  AdvanceNSquaresCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_ADVANCE_N_SQUARES; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class GoBackNSquaresCard : public Card {
 public:
  GoBackNSquaresCard(Board* board, int id, std::list<Card*>* pile, int amount);
  int GetType() { return CARD_TYPE_GO_BACK_N_SQUARES; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mAmount;
};

class AdvanceToNextCard : public Card {
 public:
  AdvanceToNextCard(Board* board, int id, std::list<Card*>* pile, int type);
  int GetType() { return CARD_TYPE_ADVANCE_TO_NEXT; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mType;
};

class GoBackToNextCard : public Card {
 public:
  GoBackToNextCard(Board* board, int id, std::list<Card*>* pile, int type);
  int GetType() { return CARD_TYPE_GO_BACK_TO_NEXT; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mType;
};

class GoToJailCard : public Card {
 public:
  GoToJailCard(Board* board, int id, std::list<Card*>* pile);
  int GetType() { return CARD_TYPE_GO_TO_JAIL; }
  void Handle(Player* player, Square* square, int diceTotal);
};

class PayOrTakeOtherPileCard : public Card {
 public:
  PayOrTakeOtherPileCard(Board* board, int id, std::list<Card*>* pile, int amount, std::list<Card*>* otherPile);
  int GetType() { return CARD_TYPE_PAY_OR_TAKE_OTHER_PILE; }
  void Handle(Player* player, Square* square, int diceTotal);
  void Dispatch(Player* player, Square* square, Message* message);
  void RequestState(Player* player);
 protected:
  int mAmount;
  std::list<Card*>* mOtherPile;
  Square* mSquare;
  int mDiceTotal;
};

class StreetRepairsCard : public Card {
 public:
  StreetRepairsCard(Board* board, int id, std::list<Card*>* pile, int houses, int hotels);
  int GetType() { return CARD_TYPE_STREET_REPAIRS; }
  void Handle(Player* player, Square* square, int diceTotal);
 protected:
  int mHouses;
  int mHotels;
};

#endif
