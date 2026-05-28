
#include <stdlib.h>
#include <string.h>
#include <list>
#include <iostream>
#include "player.h"
#include "board.h"
#include "math.h"

Player::Player(Board* board,int id, int piece, char* name, Square* square) :
  mBoard(board),
  mID(id),
  mPiece(piece),
  mName(strdup(name)),
  mJailTurns(0),
  mLaps(0),
  mHasPassedGo(false),
  mSquare(square),
  mCash(0), // board transfers funds to us
  mVotedForKick(false)
{ }

Player::Player(Board* board,int id, int piece, char* name) :
  mBoard(board),
  mID(id),
  mPiece(piece),
  mName(strdup(name)),
  mSquare(NULL),
  mJailTurns(0),
  mCash(0),
  mVotedForKick(false)
{ }

Player::~Player()
{
  free(mName);
 };

void Player::NextTurn() {
  mVotedForKick = false;
}

bool Player::CanBuyProperties() { 
  return (!mBoard->NeedToPassGoToBuy()) || (mHasPassedGo);
}

void Player::AddProperty(Property* property)
{
  mProperties.push_back(property);
}

void Player::RemoveProperty(Property* property)
{
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    if ((*i) == property) {
      mProperties.erase(i);
      return;
    }
  }
}

bool Player::HasProperty(Property* property)
{
  return property->GetOwner() == this;
}

void Player::AddCard(Card* card)
{
  mCards.push_back(card);
}

void Player::RemoveCard(Card* card)
{
  std::list<Card*>::iterator i;
  for (i = mCards.begin(); i != mCards.end(); ++i) {
    if ((*i) == card) {
      mCards.erase(i);
      return;
    }
  }
}

bool Player::HasCard(Card* card)
{
  return card->GetOwner() == this;
}

int Player::GetNumHouses()
{
  int num = 0;
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    num += (*i)->GetNumHouses();
  }
  return num;
}

int Player::GetNumHotels()
{
  int num = 0;
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    num += (*i)->GetNumHotels();
  }
  return num;
}

// Returns the number of properties the player has
// that are in the group of the property passed in.
// e.g. if |property| is a railroad, this returns how many railroads
//      the player has.  If it's St. James Place, this returns
//      the number of orange properties the player has.
int Player::GetNumOfGroup(int group)
{
  int num = 0;
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i) {
    if ((*i)->GetGroupID() == group)
      ++num;
  }
  return num;
}

int Player::GetTaxableAssets() {
  int netWorth = mCash;
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i)
    netWorth += (*i)->GetTaxableWorth();
  return netWorth;
}

int Player::GetSpendableAssets() {
  int netWorth = mCash;
  std::list<Property*>::iterator i;
  for (i = mProperties.begin(); i != mProperties.end(); ++i)
    netWorth += (*i)->GetSpendableWorth();
  return netWorth;
}

bool Player::VoteForKick() {
  bool state = mVotedForKick;
  mVotedForKick = true;
  return !state;
}
