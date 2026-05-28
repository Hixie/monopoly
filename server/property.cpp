
#include "board.h"
#include "player.h"
#include "property.h"

Property::Property(int id, int type, int price, int house, int mortgage, int* rent, int groupID) :
  mType(type),
  mID(id),
  mGroupID(groupID),
  mPrice(price),
  mHouse(house),
  mMortgage(mortgage),
  mMortgaged(false),
  mNumHouses(0),
  mNumHotels(0),
  mOwner(NULL),
  mSquare(NULL)
{
  int i;
  for (i = 0; i < 6; ++i)
    mRent[i] = rent[i];
}

int Property::GetRent(Player* player, Board* board, int diceTotal)
{
  switch (mType) {
  case PROP_TYPE_RAILROAD:
    if (diceTotal != 0) {
      return mRent[mOwner->GetNumOfGroup(mGroupID)-1];
    } else {
      // dice can only equal zero if we got here by certain cards playing games with us
      // (namely the "Pay owner twice..." card)
      return mRent[mOwner->GetNumOfGroup(mGroupID)-1] * 2;
    }
  case PROP_TYPE_UTILITY:
    if (diceTotal == 0) {
      // roll dice again, and pay 10 times amount shown
      // this is again assuming that we got here via one of those damned cards
      // XXX technically we should get the player to send PIMP_THROW_DICE, but sod that
      int die1, die2;
      board->RollDice(player, die1, die2);
      diceTotal = (die1 + die2);
      return 10 * diceTotal;
    } else {
      return mRent[mOwner->GetNumOfGroup(mGroupID) - 1] * diceTotal;
    }
  default:
    int rent;
    int houses = mNumHouses;
    int hotels = mNumHotels;
    if (hotels || houses) {
      rent = hotels*mRent[5];
      if (houses)
        rent += mRent[houses];
    } else {
      rent = mRent[0];
      if (mOwner->GetNumOfGroup(mGroupID) == board->GetNumOfGroup(mGroupID)) {
        rent *= 2;
      }
    }
    return rent;
  }
}

bool Property::VerifyAdditionAllowed(int& houses, // in: number of houses to buy, out: number of houses that would be left
                                     int& hotels  // in: number of hotels to buy, out: number of hotels that would be left
                                     ) // returns whether or not the change is possible
{
  houses = mNumHouses + houses - 4 * hotels;
  hotels = mNumHotels + hotels;
  
  // the test
  return (houses >= 0) && (houses <= 4) && (hotels >= 0) && (hotels <= 1);
}

int Property::GetHouseCost(int& houses, // in: number of houses to buy; out: number of houses required
                           int& hotels  // in: number of hotels to buy; out: number of hotels required
                           ) // returns total cost
{
  int total = 0;
  if (houses > 0)
    total += mHouse * houses;
  else
    total -= mHouse / 2 * -houses;
  if (hotels > 0)
    total += mHouse * hotels;
  else
    total -= mHouse / 2 * -hotels;

  // for each hotel sold, we need 4 houses
  if (hotels < 0)
    houses += 4 * -hotels;

  return total;
}

void Property::AddHouses(int& houses, // in: number of houses to buy; out: actual number of houses used
                         int& hotels  // in: number of houses to buy; out: actual number of hotels used
                         ) // returns nothing
{
  houses -= 4*hotels;
  mNumHouses += houses;
  mNumHotels += hotels;
}

void Property::Mortgage(Board* board)
{
  if (mMortgaged)
    return;
  board->Broadcast(PIMP_DELTA_PROPERTY_MORTGAGED_FACTORY(mID));
  mMortgaged = true;
}

void Property::Unmortgage(Board* board)
{
  if (!mMortgaged)
    return;
  board->Broadcast(PIMP_DELTA_PROPERTY_UNMORTGAGED_FACTORY(mID));
  mMortgaged = false;
}

int Property::GetTaxableWorth()
{
  return mPrice + mNumHouses * mHouse + mNumHotels * mHouse;
}

int Property::GetSpendableWorth()
{
  int value = 0;
  if (!mMortgaged)
    value += mMortgage;
  value += mNumHouses * mHouse / 2 + mNumHotels * mHouse / 2;
  return value;
}
