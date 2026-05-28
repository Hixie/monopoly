class Player;

#ifndef INCLUDED_PLAYER_H
#define INCLUDED_PLAYER_H

#include <list>
#include "board.h"
#include "card.h"
#include "property.h"
#include "square.h"

class Player { // this class is also used for Observers, actually
 public:
  Player(Board* board, int id, int piece, char* name, Square* square);
  Player(Board* board, int id, int piece, char* name); // Observer
  ~Player();

  int GetID() { return mID; };
  void SetID(int id) { mID = id; };
  int GetPiece() { return mPiece; };
  void SetPiece(int piece) { mPiece = piece; };
  char* GetName() { return strdup(mName); };
  bool IsPlaying() { return mSquare != NULL; };

  void NextTurn(); // call this to reset per-turn data

  int GetJailTurns() { return mJailTurns; }
  void SetJailTurns(int jailTurns) { mJailTurns = jailTurns; }
  Square* GetSquare() { return mSquare; }
  void SetSquare(Square* square) { mSquare = square; }

  void Lapped(bool forwards) { mLaps += forwards ? 1 : -1; mHasPassedGo = true; };
  int GetLaps() { return mLaps; }
  int HasPassedGo() { return mHasPassedGo; }
  bool CanBuyProperties();

  // Add* and Remove* must only be called by Board::Transfer()
  void AddCash(int amount) { mCash += amount; }
  void RemoveCash(int amount) { mCash -= amount; }
  bool HasCash(int amount) { return mCash >= amount; }
  int GetCash() { return mCash; }
  void AddProperty(Property* property);
  void RemoveProperty(Property* property);
  bool HasProperty(Property* property);
  void AddCard(Card* card);
  void RemoveCard(Card* card);
  bool HasCard(Card* card);

  int GetNumHouses();
  int GetNumHotels();
  int GetNumOfGroup(int group);  

  int GetTaxableAssets();
  int GetSpendableAssets();

  bool VoteForKick(); // returns false if already voted this round

  bool AuctionStarted(Property* property) { mBidding = true; }
  bool GetBidding() { return mBidding; }
  // Bid() and NoBid() return true if the state changed
  bool Bid() { bool bidding = mBidding; mBidding = true; return !bidding; }
  bool NoBid() { bool bidding = mBidding; mBidding = false; return bidding; }

private:
  Board* mBoard;
  int mID;
  int mPiece;
  char* mName;

  int mJailTurns;
  Square* mSquare;

  bool mHasPassedGo;
  int mLaps;

  int mCash;
  std::list<Property*> mProperties;
  std::list<Card*> mCards;

  bool mVotedForKick;
  bool mBidding;
};

  // Is a list really the best way to store properties?
  // Why not a vector or a map?
  // - because we don't need random access to them, we need quick
  //   insertion and deletion - hixie

#endif
