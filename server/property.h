class Property;

#ifndef INCLUDED_PROPERTY_H
#define INCLUDED_PROPERTY_H

#include "board.h"
#include "square.h"
#include "player.h"

#define PROP_TYPE_COLOUR_GROUP 0
#define PROP_TYPE_RAILROAD 1
#define PROP_TYPE_UTILITY 2

#define MORTGAGE_MARKUP 0.1

class Property {
 public:
  Property(int id, int type, int price, int house, int mortgage, int* rent, int groupID);

  int GetType() { return mType; }
  int GetID() { return mID; }
  int GetPrice() { return mPrice; };
  int GetRent(Player* player, Board* board, int diceTotal);
  int GetGroupID() { return mGroupID; }

  Square* GetSquare() { return mSquare; }
  void SetSquare(Square* square) { mSquare = square; }

  // adding negative numbers is perfectly valid (means removal/sale)
  // each added hotel implies four houses are removed
  bool VerifyAdditionAllowed(int& houses, int& hotels); // returns whether or not the change is possible, and the number of houses that would be left (e.g. "add 3 houses and 1 hotel" returns "0 houses and 1 hotel")
  int GetHouseCost(int& houses, int& hotels); // returns the cost, and the theoretical number of pieces needed (e.g. "add 3 houses and 1 hotel" returns "3 houses and 1 hotel, $x")
  void AddHouses(int& houses, int& hotels); // returns the actual number of pieces added (e.g. "add 3 houses and 1 hotel" returns "-1 houses and 1 hotel")
  int GetNumHouses() { return mNumHouses; }
  int GetNumHotels() { return mNumHotels; }

  void Mortgage(Board* board);
  void Unmortgage(Board* board);
  int MortgageValue() { return mMortgage; }
  int UnmortgageCost() { return (int)(mMortgage * (1 + MORTGAGE_MARKUP)); }
  int MortgageTransferFee() { return (int)(mMortgage * MORTGAGE_MARKUP); }
  bool IsMortgaged() { return mMortgaged; }
  bool IsTransferable() { return mNumHouses + mNumHotels == 0; }

  Player* GetOwner() { return mOwner; }
  void SetOwner(Player* owner) { mOwner = owner; }

  int GetTaxableWorth();
  int GetSpendableWorth();

 private:
  int mType;
  int mID;
  int mGroupID; // group ID 0 is for properties that are not in any group (there aren't any in standard monopoly)
  Square* mSquare;
  // Prices
  int mPrice;
  int mHouse;
  int mMortgage;
  int mRent[6];
  // State
  bool mMortgaged;
  int mNumHouses;
  int mNumHotels;
  Player* mOwner;
};

#endif
