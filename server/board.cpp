
#include <fstream>
#include <vector>
#include <algorithm>
#include <functional>
#include "stdlib.h" // for abs()
#include "limits.h" // for UINT_MAX 
#include "card.h"
#include "property.h"
#include "square.h"
#include "transaction.h"
#include "claim.h"
#include "exceptions.h"
#include "message.h"
#include "board.h"

#define CARD_JAIL1 0

// XXX There are almost certainly some possible exploits involving
// sending packets as if a game was in progress but when
// mCurrentPlayer is null.

void skipComments(std::ifstream& stream) {
  while (1) {
    int c = stream.peek();
    if (c == '\n' || c == ' ' || c == '\t') {
      // skip blank lines and leading and trailing spaces
      stream.ignore(1);
    } else if (c == '#') {
      // skip comments
      // well, 1024 seems long enough, let's hope we never find anyone silly enough to have a 1025 character comment, eh
      stream.ignore(1024, '\n');
    } else {
      break;
    }
  }
}

// return an integral random number in the range 0 - (n - 1)
int Rand(int n) { return rand() % n; }

void shuffle(std::list<Card*>* list) {
  std::vector<Card*> table;
  std::list<Card*>::iterator l;
  for (l = list->begin(); l != list->end(); ++l) {
    table.push_back(*l);
  }
  list->clear();
  std::pointer_to_unary_function<int, int> shuffler
    = std::pointer_to_unary_function<int, int>(Rand);
  std::random_shuffle(table.begin(), table.end(), shuffler);
  std::vector<Card*>::iterator t;
  for (t = table.begin(); t != table.end(); ++t) {
    list->push_back(*t);
  }
}

void Pot::Report()
{
  if (Enabled)
    mBoard->Broadcast(PIMP_DELTA_POT_FACTORY(mPot));
}

Board::Board():mAvailableHouses(TOTAL_HOUSES_AVAILABLE),
               mAvailableHotels(TOTAL_HOTELS_AVAILABLE),
               mKickVotes(0),
               mStatus(status_idle),
               mCurrentPlayer(NULL),
               mDoubles(0),
               mCash(0),
               mJail(NULL),
               mCanBuyHouses(false),
               mBlockingTransaction(NULL),
               mPot(this),
               mGameOver(false)
{
  int id;
  int success = true;

  // In case this fails, we have to make sure we close the file
  std::ifstream dataFile("board0.db");

  int type, land, house, mortgage, rent[6], groupid, property, pile, data1, data2;

  // The properties
  id = 0;
  while (id < NUM_PROPERTIES) {
    skipComments(dataFile);
    dataFile >> type >> land >> house >> mortgage >> rent[0] >> rent[1] >> rent[2] >> rent[3] >> rent[4] >> rent[5] >> groupid;
    mProperties[id++] = new Property(id-1, type, land, house, mortgage, rent, groupid);
  }

  // The board
  id = 0;
  while (id < NUM_SQUARES) {
    skipComments(dataFile);
    dataFile >> type >> property;
    switch(type) {
      case SQ_TYPE_GO:
        mSquares[id] = new GoSquare(this, id);
        break;
      case SQ_TYPE_PROPERTY:
        mSquares[id] = new PropertySquare(this, id, mProperties[property]);
        break;
      case SQ_TYPE_FREE_PARKING:
        mSquares[id] = new FreeParkingSquare(this, id, &mPot);
        break;
      case SQ_TYPE_CHANCE:
        mSquares[id] = new CardSquare(this, id, &mChanceCards);
        break;
      case SQ_TYPE_COMMUNITY_CHEST:
        mSquares[id] = new CardSquare(this, id, &mCommunityChestCards);
        break;
      case SQ_TYPE_GO_TO_JAIL:
        mSquares[id] = new GoToJailSquare(this, id);
        break;
      case SQ_TYPE_JUST_VISITING:
        mJail = new JustVisitingSquare(this, id, &mPot);
        mSquares[id] = mJail;
        break;
      case SQ_TYPE_LUXURY_TAX:
        mSquares[id] = new LuxuryTaxSquare(this, id);
        break;
      case SQ_TYPE_INCOME_TAX:
        mSquares[id] = new IncomeTaxSquare(this, id);
        break;
      default:
        success = false;
    }
    ++id;
  }

  // The chance and community chest cards
  id = 0;
  while (id < NUM_CARDS) {
    skipComments(dataFile);
    dataFile >> type >> pile >> data1 >> data2;
    std::list<Card*>* pileRef = pile ? &mCommunityChestCards : &mChanceCards;
    switch(type) {
      case CARD_TYPE_NOTHING:
        mCards[id] = new Card(this, id, pileRef);
        break;
      case CARD_TYPE_GET_OUT_OF_JAIL:
        mCards[id] = new GetOutOfJailCard(this, id, pileRef);
        break;
      case CARD_TYPE_COLLECT_FROM_BANK:
        mCards[id] = new CollectFromBankCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_PAY_TO_BANK:
        mCards[id] = new PayToBankCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_COLLECT_FROM_EACH_PLAYER:
        mCards[id] = new CollectFromEachPlayerCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_PAY_TO_EACH_PLAYER:
        mCards[id] = new PayToEachPlayerCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_ADVANCE_TO_SQUARE:
        mCards[id] = new AdvanceToSquareCard(this, id, pileRef, GetSquare(data1));
        break;
      case CARD_TYPE_GO_BACK_TO_SQUARE:
        mCards[id] = new GoBackToSquareCard(this, id, pileRef, GetSquare(data1));
        break;
      case CARD_TYPE_ADVANCE_N_SQUARES:
        mCards[id] = new AdvanceNSquaresCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_GO_BACK_N_SQUARES:
        mCards[id] = new GoBackNSquaresCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_ADVANCE_TO_NEXT:
        mCards[id] = new AdvanceToNextCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_GO_BACK_TO_NEXT:
        mCards[id] = new GoBackToNextCard(this, id, pileRef, data1);
        break;
      case CARD_TYPE_GO_TO_JAIL:
        mCards[id] = new GoToJailCard(this, id, pileRef);
        break;
      case CARD_TYPE_PAY_OR_TAKE_OTHER_PILE:
        mCards[id] = new PayOrTakeOtherPileCard(this, id, pileRef, data1, pile ? &mChanceCards : &mCommunityChestCards);
        break;
      case CARD_TYPE_STREET_REPAIRS:
        mCards[id] = new StreetRepairsCard(this, id, pileRef, data1, data2);
        break;
      default:
        success = false;
    }
    ++id;
  }

  // Options
  int flag;

  // enable the pot?
  skipComments(dataFile);
  dataFile >> flag;
  mPot.Enabled = flag == 1;

  // need to go round the board?
  skipComments(dataFile);
  dataFile >> flag;
  mNeedToPassGoToBuy = flag == 1;

  // double the go money when you land on go?
  skipComments(dataFile);
  dataFile >> flag;
  mDoubleGoMoneyEnabled = flag == 1;

  // who gets the money when someone goes bankrupt?
  skipComments(dataFile);
  dataFile >> flag;
  mBankruptcyTransfersToPlayer = flag == 0;

  success |= !!dataFile; // convert to boolean

  // regardless of any errors, make sure to close the file
  dataFile.close();

  if (!success || !mJail) {
    // it failed. Throw an exception.
    throw DataFileParseError();
  }

  // shuffle cards
  shuffle(&mChanceCards);
  shuffle(&mCommunityChestCards);

}

void Board::OverrideSettings(bool potEnabled,
                             bool needToPassGoToBuy,
                             bool doubleGoMoneyEnabled,
                             bool bankruptcyTransfersToPlayer)
{
  mPot.Enabled = potEnabled;
  mNeedToPassGoToBuy = needToPassGoToBuy;
  mDoubleGoMoneyEnabled = doubleGoMoneyEnabled;
  mBankruptcyTransfersToPlayer = bankruptcyTransfersToPlayer;
}

Board::~Board()
{
  int i;
  while (mClaims.begin() != mClaims.end()) {
    Claim* claim = mClaims.front();
    delete claim;
    mClaims.pop_front();
  }
  while (mTransactions.begin() != mTransactions.end()) {
    Transaction* transaction = mTransactions.front();
    delete transaction;
    mTransactions.pop_front();
  }
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i])
      delete mPlayers[i];
  }
  for (i = 0; i < NUM_PROPERTIES; ++i) {
    delete mProperties[i];
  }
  for (i = 0; i < NUM_CARDS; ++i) {
    delete mCards[i];
  }
  for (i = 0; i < NUM_SQUARES; ++i) {
    delete mSquares[i];
  }
}

// Player IDs are 1..MAX_PLAYERS, but are stored in a vector at positions 0..MAX_PLAYERS-1.

Player* Board::GetPlayer(int id)
{
  if (id > 0 && id <= mPlayers.size()) {
    return mPlayers[id-1];
  } else {
    return NULL;
  }
}

Player* Board::GetPlayer(Player* player, int delta) {
  int index = player->GetID();
  while (delta || !player->IsPlaying()) {
    do {
      if (delta >= 0) {
        if (++index > mPlayers.size())
          index = 1;
      } else {
        if (--index < 1)
          index = mPlayers.size();
      }
      player = GetPlayer(index);
    } while (!player || !player->IsPlaying());
    if (delta > 0) {
      --delta;
    } else if (delta < 0) {
      ++delta;
    }
  }
  return player;
}

int Board::GetPlayingPlayersCount() {
  int count = 0;
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i]) {
      if (mPlayers[i]->IsPlaying())
        ++count;
    }
  }
  return count;
}

Property* Board::GetProperty(int id)
{
  if (id < 0 || id >= NUM_PROPERTIES)
    return NULL;
  return mProperties[id];
}

void Board::PurchaseHouses(Player* buyer, int count, Property** properties, int* amountHouses, int* amountHotels)
{
  int cost = 0, i;
  int totalHouses[NUM_PROPERTIES]; // the board, as it will look like after this trade
  int totalHouseDelta = 0;
  int totalHotelDelta = 0;

  for (i = 0; i < NUM_PROPERTIES; ++i) {
    // prefill the array so that any properties not being modified are set up too
    totalHouses[i] = mProperties[i]->GetNumHouses() + mProperties[i]->GetNumHotels()*5;
  }

  for (i = 0; i < count; ++i) {
    int groupID = properties[i]->GetGroupID();

    // Can't buy houses if you don't own property
    if (properties[i]->GetOwner() != buyer)
      return Send(PIMP_ERROR_HOUSES_NOT_OWNED_FACTORY(properties[i]->GetID()),
                  buyer->GetID());

    // Can't buy houses on railroads or utilities.
    if (properties[i]->GetType() != PROP_TYPE_COLOUR_GROUP)
      return Send(PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP_FACTORY(properties[i]->GetID()),
                  buyer->GetID());

    // Can't buy houses if the property is mortgaged.  
    if (properties[i]->IsMortgaged())
      return Send(PIMP_ERROR_HOUSES_MORTGAGED_FACTORY(properties[i]->GetID()),
                  buyer->GetID());

    // Can't buy houses if you don't own the group.
    if (buyer->GetNumOfGroup(groupID) != GetNumOfGroup(groupID))
      return Send(PIMP_ERROR_HOUSES_NEED_MONOPOLY_FACTORY(properties[i]->GetID()),
                  buyer->GetID());

    // Can't buy houses if it isn't your turn
    if (((mCurrentPlayer != buyer) || (!mCanBuyHouses)) &&
        ((amountHotels[i] > 0) || (amountHouses[i] > 0)))
      return Send(PIMP_ERROR_HOUSES_NOT_YOUR_TURN_FACTORY(), buyer->GetID());

    // Can't buy if the amount wanted is impossible.
    // This also sets up the "target" values.
    int targetHouses = amountHouses[i];
    int targetHotels = amountHotels[i];
    if (!properties[i]->VerifyAdditionAllowed(targetHouses, targetHotels)) // modifies arguments in place
      return Send(PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER_FACTORY(properties[i]->GetID()),
                  buyer->GetID());
    totalHouses[properties[i]->GetID()] = targetHouses + targetHotels*5;

    // Take note of number of pieces _theoretically_ needed to do this.
    // Probably more than are actually physically needed, but the
    // rules say that it must be possible to get to the total amount.
    // Alse take note of cash cost.
    int neededHouses = amountHouses[i];
    int neededHotels = amountHotels[i];
    cost += properties[i]->GetHouseCost(neededHouses, neededHotels); // modifies arguments in place
    totalHouseDelta += neededHouses;
    totalHotelDelta += neededHotels;
  }

  // Can't buy houses if there are none to buy.
  if (mAvailableHouses - totalHouseDelta < 0 ||
      mAvailableHotels - totalHotelDelta < 0)
    return Send(PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT_FACTORY(mAvailableHouses,
                                                               mAvailableHotels),
                buyer->GetID());

  // Can't buy houses if can't afford it.
  if (!buyer->HasCash(cost))
    return Send(PIMP_ERROR_HOUSES_TOO_EXPENSIVE_FACTORY(cost), buyer->GetID());

  // if cost > 0, don't allow this with pending transactions if it
  // would decrease net worth.
  Transaction* t = GetTransactionBlockingAgreement(buyer, -cost);
  if (t)
    return Send(PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH_FACTORY(t->GetID()),
                buyer->GetID());

  // Can't build unevenly.
  // (this has to be able to cope for groups that are non-adjacent
  // in case we ever allow building on railroads or new boards)
  for (i = 0; i < count; ++i) {
    int id = properties[i]->GetID();
    if (totalHouses[id] == -1)
      continue; // this means we've already examined this property
    // else, we haven't done it yet, so check this property's group
    // first, initialise the values from this property
    int groupID = properties[i]->GetGroupID();
    int low = totalHouses[id];
    int high = totalHouses[id];
    // next, look at every property on the board, and for those in the same group, update values
    for (int j = 0; j < NUM_PROPERTIES; ++j) {
      if (mProperties[j]->GetGroupID() == groupID) {
        if (totalHouses[j] < low)
          low = totalHouses[j];
        else if (totalHouses[j] > high)
          high = totalHouses[j];
        // as an optimisation, we set the values to -1 
        // that way we know we've done the property already
        totalHouses[j] = -1;
      }
    }
    // finally, check values are acceptable
    if (high-low > 1) // difference between houses can be at most 1
      return Send(PIMP_ERROR_HOUSES_UNBALANCED_FACTORY(id), buyer->GetID());
  }

  // Okay, you've passed the tests. Buy the houses.

  // Change the houses
  for (i = 0; i < count; ++i) {
    properties[i]->AddHouses(amountHouses[i], amountHotels[i]); // this changes those numbers in place!
    // don't use totalHouseDelta and totalHotelDelta because that's
    // the number theoretically needed, not the number physically
    // needed. e.g. going from 3 houses to 1 hotel requires 1 houses
    // and 1 hotel to be available, but only uses 1 hotel and actually
    // _returns_ three houses.
    mAvailableHouses -= amountHouses[i];
    mAvailableHotels -= amountHotels[i];
    Broadcast(PIMP_DELTA_HOUSES_PURCHASED_FACTORY(buyer->GetID(), properties[i]->GetID(), properties[i]->GetNumHouses(), properties[i]->GetNumHotels()));
  }

  Broadcast(PIMP_DELTA_BANK_FACTORY(mAvailableHouses, mAvailableHotels));

  // Spend the money
  if (cost > 0)
    Broadcast(PIMP_CONSTRUCTION_TAKES_CASH_FACTORY(buyer->GetID(), cost));
  else if (cost < 0)
    Broadcast(PIMP_DESTRUCTION_GIVES_CASH_FACTORY(buyer->GetID(), -cost));
  Transfer(buyer, BANK, cost, overrideEscrow);
}

int Board::GetNumOfGroup(int group) {
  int count = 0;
  for (int i = 0; i < NUM_PROPERTIES; ++i) {
    if (mProperties[i]->GetGroupID() == group)
      ++count;
  }
  return count;
}

void Board::SellProperty(Property* property)
{
  mStatus = status_propertySale;
  mPropertyOnSale = property;
  Broadcast(PIMP_PROPERTY_SALE_FACTORY(mCurrentPlayer->GetID(), property->GetID(), property->GetPrice()));
}

void Board::AuctionProperty(Property* property)
{
  mStatus = status_propertyAuction;
  mPropertyOnSale = property;
  mBid = 0;
  mLeadingBidder = NULL;
  mNoBids = 0;
  // call all players and tell them AuctionStarted()
  Broadcast(PIMP_PROPERTY_AUCTION_FACTORY(property->GetID()));
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i] && mPlayers[i]->IsPlaying()) {
      mPlayers[i]->AuctionStarted(property);
    }
  }
}

void Board::CheckAuction()
{
  int remainingBidders = GetPlayingPlayersCount() - mNoBids;
  if (remainingBidders == 0) {
    // No winners
    Broadcast(PIMP_PROPERTY_AUCTION_VOID_FACTORY());
    NextTurn();
  } else if (remainingBidders == 1 && mLeadingBidder) {
    // We have a winner!
    Broadcast(PIMP_PROPERTY_AUCTION_WON_FACTORY(mLeadingBidder->GetID()));
    if (mLeadingBidder->HasCash(mBid) &&
        !GetTransactionBlockingAgreement(mLeadingBidder, -mBid)) {
      Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(mLeadingBidder->GetID(),
                                               mPropertyOnSale->GetSquare()->GetID(),
                                               mBid));
      Transfer(mLeadingBidder, BANK, mBid);
      Transfer(BANK, mLeadingBidder, mPropertyOnSale);
      NextTurn();
    } else {
      AddTransaction(new PostAuctionTransaction(this, mLeadingBidder, mPropertyOnSale, mBid), true);
    }
  }
}

Card* Board::GetCard(int id)
{
  if (id < 0 || id >= NUM_CARDS)
    return NULL;
  return mCards[id];
}

Square* Board::GetSquare(int id)
{
  if (id < 0 || id >= NUM_SQUARES)
    return NULL;
  return mSquares[id];
}

Square* Board::GetSquare(Square* start, int delta)
{
  int targetID = start->GetID() + delta;
  while (targetID >= NUM_SQUARES)
    targetID -= NUM_SQUARES;
  while (targetID < 0)
    targetID += NUM_SQUARES;
  return mSquares[targetID];
}

Square* Board::GetNextPropertySquareOfType(Square* start, int type)
{
  Square* square = start;
  do {
    square = GetSquare(square, 1);
  } while (square != start &&
           !(square->GetType() == SQ_TYPE_PROPERTY &&
             static_cast<PropertySquare*> (square)->GetPropertyType() == type));
  return square;
}

Square* Board::GetLastPropertySquareOfType(Square* start, int type)
{
  Square* square = start;
  do {
    square = GetSquare(square, -1);
  } while (square != start &&
           !(square->GetType() == SQ_TYPE_PROPERTY &&
             static_cast<PropertySquare*> (square)->GetPropertyType() == type));
  return square;
}

Transaction* Board::GetTransaction(int id)
{
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    if ((*i)->GetID() == id) {
      return (*i);
    }
  }
  return NULL;
}

Transaction* Board::GetLowestTransactionInvolvingPlayer(Player* player)
{
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    if ((*i)->InvolvesPlayer(player)) {
      return (*i);
    }
  }
  return NULL;
}

Transaction* Board::GetUncancellableTransactionInvolvingPlayer(Player* player, int id)
{
  Transaction* t = NULL;
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    if ((*i)->GetID() >= id)
      break;
    if ((*i)->InvolvesPlayer(player) &&
        (*i)->GetLiabilityForPlayer(player) > 0)
      t = (*i);
  }
  return t;
}

Transaction* Board::GetTransactionBlockingAgreement(Player* player,
                                                    Transaction* transaction)
{
  int currentWorth = player->GetSpendableAssets();
  int deltaWorth = transaction->GetNetSpendableAssetDeltaForPlayer(player);
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    int liability = (*i)->GetLiabilityForPlayer(player);
    if ((*i) == transaction) {
      if (liability > 0) {
        // this is the transaction the player wants to agree
        // it is not cancellable
        // it therefore overrides any later transactions
        return NULL;
      } // else it is cancellable, later uncancellable ones override
    } else if ((*i)->InvolvesPlayer(player)) {
      if (liability <= currentWorth &&
          liability > currentWorth + deltaWorth) {
        // this one is not a cancellable transaction, player could
        // afford it without doing the deal, but couldn't after doing
        // the deal. Therefore, this transaction blocks the deal.
        return (*i);
      } else if (liability > currentWorth &&
                 deltaWorth < 0) {
        // this one is not a cancellable transaction, and while player
        // could not afford it without doing the deal, this deal would
        // actually make the situation worse. Therefore, this
        // transaction blocks the deal.
        return (*i);
      }
    }
  }
  // no transactions have any reason to stop this
  return NULL;
}

Transaction* Board::GetTransactionBlockingAgreement(Player* player, int deltaWorth)
{
  int currentWorth = player->GetSpendableAssets();
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    int liability = (*i)->GetLiabilityForPlayer(player);
    if ((*i)->InvolvesPlayer(player)) {
      if (liability <= currentWorth &&
          liability > currentWorth + deltaWorth) {
        // this one is not a cancellable transaction, player could
        // afford it without spending money, but couldn't after
        // spending money. Therefore, this transaction blocks the
        // spending.
        return (*i);
      } else if (liability > currentWorth &&
                 deltaWorth < 0) {
        // this one is not a cancellable transaction, and while player
        // could not afford it without spending this money, spending
        // it would actually make the situation worse. Therefore, this
        // transaction blocks the spending.
        return (*i);
      }
    }
  }
  // no transactions have any reason to stop this
  return NULL;
}

void Board::AddTransaction(Transaction* transaction, bool blocking)
{
  mTransactions.push_back(transaction);
  transaction->Begin();
  if (blocking) {
    mStatus = status_transactionBlocking;
    mBlockingTransaction = transaction;
    Broadcast(PIMP_WAITING_FOR_TRANSACTION_FACTORY(transaction->GetDebtorPlayer()->GetID(),
                                                   transaction->GetID()));
  }
}

void Board::RemoveTransaction(Transaction* transaction)
{
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i) {
    if ((*i) == transaction) {
      mTransactions.erase(i);
      return;
    }
  }
}

void Board::AddClaim(Claim* claim)
{
  mClaims.push_back(claim);
}

void Board::RemoveClaim(Claim* claim)
{
  std::list<Claim*>::iterator i;
  for (i = mClaims.begin(); i != mClaims.end(); ++i) {
    if ((*i) == claim) {
      mClaims.erase(i);
      return;
    }
  }
}

void Board::ChargePlayer(Player* player, Square* square, int amount)
{
  if (player->HasCash(amount) && !GetTransactionBlockingAgreement(player, -amount)) {
    Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(player->GetID(), square->GetID(), amount));
    Transfer(player, BANK, amount);
    mPot += amount;
  } else {
    AddTransaction(new SquareTransaction(this, &mPot, player, square, amount), true);
  }
}

void Board::ChargeAllPlayers(Player* player, Card* card, int amount)
{
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i] && mPlayers[i]->IsPlaying() && mPlayers[i] != player) {
      if (amount > 0) {
        AddTransaction(new CardTransaction(this, player, mPlayers[i], card, amount), false);
      } else {
        AddTransaction(new CardTransaction(this, mPlayers[i], player, card, -amount), false);
      }
    }
  }
}

void Board::NextTurn()
{
  if (GetPlayingPlayersCount() == 0) {
    // shouldn't be able to get here...
    cerr << "Tried to go to the next player but there aren't any." << endl;
    return;
  }
  if (mDoubles && (mCurrentPlayer->GetJailTurns() == 0)) {
    // did current player get doubles?
    // if so, replay him
    Broadcast(PIMP_ROLL_AGAIN_FACTORY(mCurrentPlayer->GetID()));
    mStatus = status_playAgain;
  } else {
    // else, find who next player is, and NextTurn(him)
    NextTurn(GetPlayer(mCurrentPlayer, 1));
  }
}

void Board::NextTurn(Player* player)
{
  mStatus = status_startOfTurn;
  mCurrentPlayer = player;
  mDoubles = 0;
  mKickVotes = 0;
  // call all players and tell them NextTurn()
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i]) {
      mPlayers[i]->NextTurn();
    }
  }
  Broadcast(PIMP_START_OF_TURN_FACTORY(mCurrentPlayer->GetID(), mCurrentPlayer->GetJailTurns() == 0));
  mCanBuyHouses = true;
  if (mCurrentPlayer->GetJailTurns() > 0) {
    Broadcast(PIMP_JAIL_PAY_OR_ROLL_FACTORY(mCurrentPlayer->GetID(),
                                            MAX_JAIL_TURNS - mCurrentPlayer->GetJailTurns() + 1,
                                            mCanBuyHouses));
  }
}

void Board::Transfer(Player* from, Player* to)
{
  if ((from == BANK) || (from == to))
    return;

  bool takeover = false;
  if (to != BANK && !to->IsPlaying()) {
    // make new player into player
    to->SetSquare(from->GetSquare());
    to->SetJailTurns(from->GetJailTurns());
    Broadcast(PIMP_PLAYER_TAKEOVER_FACTORY(to->GetID(), from->GetID()));
    if (mStatus == status_propertyAuction)
      to->AuctionStarted(mPropertyOnSale);
    takeover = true;
  }

  // tell transactions
  std::list<Transaction*>::iterator t;
  for (t = mTransactions.begin(); t != mTransactions.end(); ++t) {
    if (!(*t)->GetEnded()) {
      (*t)->TransferNotification(from, to);
      if ((*t)->GetEnded()) {
        std::list<Transaction*>::iterator endedTransaction = t++;
        Transaction* transaction = (*endedTransaction);
        mTransactions.erase(endedTransaction);
        delete transaction;
      }
    }
  }

  // transfer estate
  Transfer(from, to, from->GetCash(), overrideEscrow); // XXX should pass the id of the transaction that went bankrupt all the way to here
  int i;
  for (i = 0; i < NUM_PROPERTIES; ++i) {
    Property* property = GetProperty(i);
    if (from->HasProperty(property)) {
      Transfer(from, to, property, takeover);
    }
  }
  for (i = 0; i < NUM_CARDS; ++i) {
    Card* card = GetCard(i);
    if (from->HasCard(card)) {
      Transfer(from, to, card);
    }
  }

  // remove from board
  from->SetSquare(NULL);
  from->SetJailTurns(0);

  // tell claims
  std::list<Claim*>::iterator c;
  for (c = mClaims.begin(); c != mClaims.end(); ++c) {
    if (!(*c)->TransferNotification(from, to)) {
      std::list<Claim*>::iterator obsoleteClaim = c++;
      Claim* claim = (*obsoleteClaim);
      mClaims.erase(obsoleteClaim);
      delete claim;
    }
  }

  if (mCurrentPlayer == from) {
    if (takeover) { // to != NULL
      mCurrentPlayer = to;
      // tell everyone about this
      int i;
      for (i = 0; i < mPlayers.size(); ++i) {
        if (mPlayers[i])
          TellPlayerState(mPlayers[i]);
      }
    } else {
      if (GetPlayingPlayersCount() > 0)
        NextTurn(GetPlayer(mCurrentPlayer, 1));
      else {
        mCurrentPlayer = NULL;
        mStatus = status_idle;
        for (i = 0; i < mPlayers.size(); ++i) {
          if (mPlayers[i])
            TellPlayerState(mPlayers[i]);
        }
      }
    }
  } else {
    // handle auction problems
    if (mStatus == status_propertyAuction) {
      if (from->GetBidding() == false) {
        --mNoBids;
        CheckAuction();
      } else if (from == mLeadingBidder) {
        // restarting the auction is the simplest thing to do, I guess
        AuctionProperty(mPropertyOnSale);
      }
    }
  }
}

void Board::Transfer(Player* from, Player* to, int amount, int transactionID = INT_MAX)
{
  if ((amount == 0) || (from == to))
    return;

  if (amount < 0) {
    Player* temp = from;
    from = to;
    to = temp;
    amount = -amount;
  }

  if (transactionID > 0 && from == BANK && to != BANK) {
    Transaction* t = GetUncancellableTransactionInvolvingPlayer(to, transactionID);
    if (t) {
      t->AddToEscrow(to, amount);
      Broadcast(PIMP_MONEY_BEING_HELD_IN_ESCROW_FACTORY(to->GetID(), t->GetID(), amount));
      return;
    }
  }

  int fromID, toID;
  if (from == BANK) {
    fromID = 0;
    RemoveCash(amount);
  } else {
    fromID = from->GetID();
    from->RemoveCash(amount);
  }
  if (to == BANK) {
    toID = 0;
    AddCash(amount);
  } else {
    toID = to->GetID();
    to->AddCash(amount);
  }
  Broadcast(PIMP_DELTA_CASH_FACTORY(fromID, toID, amount));
  // tell transactions
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i)
    (*i)->TransferNotification(from, to, amount);
}

void Board::Transfer(Player* from, Player* to, Property* property, bool immuneToFees = false)
{
  if ((!property) || (from == to))
    return;

  int fromID, toID;
  if (from == BANK) {
    fromID = 0;
  } else {
    fromID = from->GetID();
    from->RemoveProperty(property);
  }
  if (to == BANK) {
    toID = 0;
  } else {
    toID = to->GetID();
    to->AddProperty(property);
  }
  property->SetOwner(to);
  // sell houses if any
  int houses = -property->GetNumHouses();
  int hotels = -property->GetNumHotels();
  if (!immuneToFees && (houses != 0 || hotels != 0)) {
    houses = houses + 4 * hotels;
    if (to != BANK) {
      int cash = -property->GetHouseCost(houses, hotels);
      Broadcast(PIMP_DESTRUCTION_GIVES_CASH_FACTORY(to->GetID(), cash));
      Transfer(BANK, to, cash); // XXX should propagate transaction id from bankruptcy to here
    }
    property->AddHouses(houses, hotels);
    mAvailableHouses -= houses;
    mAvailableHotels -= hotels;
    Broadcast(PIMP_DELTA_BANK_FACTORY(mAvailableHouses, mAvailableHotels));
    Broadcast(PIMP_DELTA_HOUSES_PURCHASED_FACTORY(0, property->GetID(),
                                                  property->GetNumHouses(),
                                                  property->GetNumHotels()));
  }
  Broadcast(PIMP_DELTA_PROPERTY_FACTORY(fromID, toID, property->GetID()));
  // tell transactions
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i)
    (*i)->TransferNotification(from, to, property);
  // charge transfer fee if mortgaged (and not bank)
  if (property->IsMortgaged()) {
    if (to == BANK) {
      property->Unmortgage(this);
    } else if (!immuneToFees) {
      int cost = property->MortgageTransferFee();
      if (to->HasCash(cost)) {
        // XXX should propagate transaction id from bankruptcy to here
        Transfer(to, BANK, cost);
      } else {
        AddTransaction(new BankTransaction(this, to, cost), false);
      }
    }
  }
}

void Board::Transfer(Player* from, Player* to, Card* card)
{
  if ((!card) || (from == to))
    return;

  int fromID, toID;
  if (from == BANK) {
    fromID = 0;
    RemoveCard(card);
  } else {
    fromID = from->GetID();
    from->RemoveCard(card);
  }
  if (to == BANK) {
    toID = 0;
    AddCard(card);
  } else {
    toID = to->GetID();
    to->AddCard(card);
  }
  card->SetOwner(to);
  Broadcast(PIMP_DELTA_CARD_FACTORY(fromID, toID, card->GetID()));
  // tell transactions
  std::list<Transaction*>::iterator i;
  for (i = mTransactions.begin(); i != mTransactions.end(); ++i)
    (*i)->TransferNotification(from, to, card);
}

void Board::MovePlayer(Player* player, int delta, int diceTotal)
{
  if (delta == 0)
    return MovePlayerDirectly(player, player->GetSquare(), true, diceTotal);
  bool forwards = delta > 0;
  int remainingSquaresToPass = abs(delta);
  Square* square = GetSquare(player->GetSquare(), forwards ? 1 : -1);
  while (--remainingSquaresToPass) {
    square->PassedBy(player, forwards);
    square = GetSquare(square, forwards ? 1 : -1);
  }
  return MovePlayerDirectly(player, square, forwards, diceTotal);
}

void Board::MovePlayer(Player* player, Square* target, bool forwards, int diceTotal)
{
  Square* square = GetSquare(player->GetSquare(), forwards ? 1 : -1);
  while (square != target) {
    square->PassedBy(player, forwards);
    square = GetSquare(square, forwards ? 1 : -1);
  }
  return MovePlayerDirectly(player, target, forwards, diceTotal);
}

void Board::MovePlayerDirectly(Player* player, Square* target, bool forwards, int diceTotal)
{
  return target->LandedOn(player, forwards, diceTotal);
}

void Board::GoToJail(Player* player)
{
  mDoubles = 0;
  mJail->PlayerSentToJail(player);
}

void Board::ClearPendingClaims()
{
  if (mClaims.begin() != mClaims.end()) {
    Broadcast(PIMP_RENT_COLLECTION_MORATORIUM_FACTORY());
    while (mClaims.begin() != mClaims.end()) {
      Claim* claim = mClaims.front();
      delete claim;
      mClaims.pop_front();
    }
  }
}

bool Board::AreOutstandingClaimsAgainst(Player* player) {
  std::list<Claim*>::iterator i;
  for (i = mClaims.begin(); i != mClaims.end(); ++i) {
    if ((*i)->IsAgainst(player))
      return true;
  }
  return false;
}

void Board::PlayerClaimingBankruptcy(Player* player, Player* creditor, int amount)
{
  Broadcast(PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT_FACTORY(player->GetID()));
  // XXX should propagate transaction id down to those transfers:
  if (mBankruptcyTransfersToPlayer) {
    Transfer(player, creditor);
  } else {
    Transfer(player, BANK);
    Transfer(BANK, creditor, amount, overrideEscrow);
  }
  if (GetPlayingPlayersCount() == 1) {
    Broadcast(PIMP_PLAYER_WON_FACTORY(GetPlayer(mCurrentPlayer, 0)->GetID()));
    mGameOver = true;
  }
}

void Board::Dispatch(Message* message)
{
  switch (message->GetType()) {
  // The metagame
  case PIMP_WELCOME_PLAYER:
  case PIMP_WELCOME_OBSERVER:
    DispatchWelcomeUser(message);
    break;
  case PIMP_OBSERVER_BECAME_PLAYER:
    DispatchObserverBecamePlayer(message);
    break;
  case PIMP_REQUEST_STATE:
    DispatchRequestState(message);
    break;
  case PIMP_KICK:
    DispatchKick(message);
    break;
  case PIMP_TRANSFER:
    DispatchTransfer(message);
    break;
  // The game
  case PIMP_THROW_DICE:
    DispatchThrowDice(message);
    break;
  case PIMP_JAIL_PAY_BAIL:
  case PIMP_JAIL_ROLL_DICE:
    DispatchJailChoice(message);
    break;
  case PIMP_TRANSACTION_SET_CASH:
  case PIMP_TRANSACTION_ADD_PROPERTY:
  case PIMP_TRANSACTION_REMOVE_PROPERTY:
  case PIMP_TRANSACTION_ADD_CARD:
  case PIMP_TRANSACTION_REMOVE_CARD:
  case PIMP_TRANSACTION_FINISH:
  case PIMP_TRANSACTION_REOPEN:
  case PIMP_TRANSACTION_AGREE:
  case PIMP_BANKRUPT_TRANSACTION:
  case PIMP_TRANSACTION_CANCEL:
    DispatchTransaction(message);
    break;
  case PIMP_TRANSACTION_REQUEST_TRADE:
    DispatchTradeRequest(message);
    break;
  case PIMP_CLAIM_RENT:
  case PIMP_CLAIM_GO:
    DispatchClaims(message);
    break;
  case PIMP_BUY_PROPERTY:
  case PIMP_AUCTION_PROPERTY:
    DispatchPropertySale(message);
    break;
  case PIMP_BID:
  case PIMP_NO_BID:
    DispatchPropertyAuction(message);
    break;
  case PIMP_PURCHASE_HOUSES:
    DispatchPurchaseHouses(message);
    break;
  case PIMP_MORTGAGE_PROPERTY:
  case PIMP_UNMORTGAGE_PROPERTY:
    DispatchMortgages(message);
    break;
  case PIMP_TAX_PAY_TEN_PERCENT:
  case PIMP_TAX_PAY_FLAT_FEE:
  case PIMP_CARD_SELECT_COLLECT:
  case PIMP_CARD_SELECT_RETRY:
    DispatchSquare(message);
    break;
  default:
    UnexpectedMessage(message);
  }
}

void Board::DispatchWelcomeUser(Message* message)
{
  PIMP_WELCOME_PLAYER_CAST(message, details);
  bool startGame = false;
  Player* player;
  char* name = details->GetField3();
  if (message->GetType() == PIMP_WELCOME_PLAYER) {
    player = new Player(this, details->GetField1(), details->GetField2(), name, GetSquare(0));
    Transfer(BANK, player, STARTING_CASH);
    mGameOver = false;
    // if this is the second player, start the game
    if ((GetPlayingPlayersCount() > 0) && (mStatus == status_idle))
      startGame = true;
  } else {
    // Observer
    player = new Player(this, details->GetField1(), details->GetField2(), name);
  }
  free(name);
  AddPlayer(player);
  DispatchRequestState(message);
  if (startGame)
    NextTurn(player);
}

void Board::DispatchObserverBecamePlayer(Message* message)
{
  PIMP_OBSERVER_BECAME_PLAYER_CAST(message, details);
  Player* player = GetPlayer(details->GetPlayer());
  Transfer(BANK, player, STARTING_CASH);
  player->SetSquare(GetSquare(0));
  player->SetJailTurns(0);
  mGameOver = false;
  // if this is the first player, start the game
  if ((GetPlayingPlayersCount()) > 1 && (mStatus == status_idle))
    NextTurn(player);
}

void Board::DispatchRequestState(Message* message)
{
  Send(PIMP_STATE_BOARD_FACTORY(0), message->GetPlayer());
  Player* player = GetPlayer(message->GetPlayer());
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i]) {
      char* name = mPlayers[i]->GetName();
      if (mPlayers[i]->IsPlaying()) {
        Send(PIMP_STATE_PLAYER_FACTORY(mPlayers[i]->GetID(), mPlayers[i]->GetPiece(), name,
                                       mPlayers[i]->GetSquare()->GetID(),
                                       mPlayers[i]->GetCash(), mPlayers[i]->GetJailTurns()),
             message->GetPlayer());
      } else {
        Send(PIMP_STATE_OBSERVER_FACTORY(mPlayers[i]->GetID(), mPlayers[i]->GetPiece(), name),
             message->GetPlayer());
      }
      free(name);
    }
  }
  for (i = 0; i < NUM_PROPERTIES; ++i) {
    if (mProperties[i]->GetOwner()) {
        Send(PIMP_STATE_PROPERTY_FACTORY(mProperties[i]->GetID(),
                                         mProperties[i]->GetOwner()->GetID(),
                                         mProperties[i]->IsMortgaged(),
                                         mProperties[i]->GetNumHouses(),
                                         mProperties[i]->GetNumHotels()),
             message->GetPlayer());
    }
  }
  for (i = 0; i < NUM_CARDS; ++i) {
    if (mCards[i]->GetOwner()) {
        Send(PIMP_STATE_CARD_FACTORY(mCards[i]->GetID(),
                                     mCards[i]->GetOwner()->GetID()),
             message->GetPlayer());
    }
  }
  Send(PIMP_DELTA_BANK_FACTORY(mAvailableHouses, mAvailableHotels), message->GetPlayer());
  Send(PIMP_STATE_POT_FACTORY(mPot.GetPot()), message->GetPlayer());

  // XXX: PIMP_LINK_DEAD

  // tell transactions
  std::list<Transaction*>::iterator t;
  for (t = mTransactions.begin(); t != mTransactions.end(); ++t)
    (*t)->RequestState(player);

  TellPlayerState(player);
}

void Board::DispatchKick(Message* message)
{
  Player* player = GetPlayer(message->GetPlayer());
  // XXX should only allow players to vote
  // XXX shouldn't allow kick while player owes money
  if (mCurrentPlayer && player->VoteForKick()) {
    ++mKickVotes;
    if (mKickVotes >= (mPlayers.size() + 1) / 2) {
      int id = mCurrentPlayer->GetID();
      Transfer(mCurrentPlayer, BANK);
      Broadcast(PIMP_PLAYER_BECAME_OBSERVER_KICKED_FACTORY(id));
    }
  } else {
    // already voted or no current player
    UnexpectedMessage(message);
  }
}

void Board::DispatchTransfer(Message* message)
{
  PIMP_TRANSFER_CAST(message, details);
  Player* player = GetPlayer(details->GetPlayer());
  if (!player || !player->IsPlaying()) {
    UnexpectedMessage(message);
    return;
  }
  Player* target;
  int targetID = details->GetField1();
  if (targetID) {
    target = GetPlayer(targetID);
    if (!target) {
      InvalidPayload(message);
      return;
    }
  } else {
    target = BANK;
  }
  if (player == target) {
    InvalidPayload(message);
    return;
  }
  if (target == BANK || target->IsPlaying()) {
    Transaction* transaction = GetLowestTransactionInvolvingPlayer(player);
    if (transaction) {
      Send(PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION_FACTORY(transaction->GetID()),
           message->GetPlayer());
      return;
    }
    if (AreOutstandingClaimsAgainst(player)) {
      Send(PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION_FACTORY(0), message->GetPlayer());
      return;
    }
  }
  int beforeCount = GetPlayingPlayersCount();
  Transfer(player, target);
  int afterCount = GetPlayingPlayersCount();
  Broadcast(PIMP_PLAYER_BECAME_OBSERVER_TRANSFER_FACTORY(player->GetID()));
  if (beforeCount > afterCount && afterCount == 1 && mCurrentPlayer) {
    Broadcast(PIMP_PLAYER_WON_FACTORY(GetPlayer(mCurrentPlayer, 0)->GetID()));
    mGameOver = true;
  }
}

void Board::DispatchThrowDice(Message* message)
{
  Player* player = GetPlayer(message->GetPlayer());
  if ((player == mCurrentPlayer) && (player->GetJailTurns() == 0)) {
    switch (mStatus) {
    case status_startOfTurn:
      ClearPendingClaims();
      mCanBuyHouses = false;
      // fallthrough:
    case status_playAgain: {
      // roll dice
      int die1, die2;
      RollDice(player, die1, die2);
      if (die1 == die2) {
        // doubles, take that into account
        ++mDoubles;
        if (mDoubles >= 3) {
          // triple doubles -> jail!
          Broadcast(PIMP_THREE_DOUBLES_FACTORY(player->GetID()));
          Broadcast(PIMP_DICE_MOVED_PLAYER_FACTORY(player->GetID(), mJail->GetID(), 0));
          GoToJail(player);
          NextTurn();
          break;
        }
      } else {
        // not doubles, clear the doubles streak counter
        mDoubles = 0;
      }
      // move player
      Square* target = GetSquare(player->GetSquare(), die1 + die2);
      Broadcast(PIMP_DICE_MOVED_PLAYER_FACTORY(player->GetID(), target->GetID(), die1 + die2));
      Unblock();
      MovePlayer(player, die1 + die2);
      if (mStatus == status_idle)
        NextTurn();
      break;
    }
    default:
      // how inappropriate
      UnexpectedMessage(message);
    }
  } else {
    // not your turn
    UnexpectedMessage(message);
  }
}

void Board::DispatchJailChoice(Message* message)
{
  Player* player = GetPlayer(message->GetPlayer());
  if (player == mCurrentPlayer && player->GetJailTurns() != 0) {
    ClearPendingClaims();
    mCanBuyHouses = false;
    if (player->GetSquare()->GetType() != SQ_TYPE_JUST_VISITING) {
      cerr << "Player is in jail but it isn't a jail!" << endl;
      UnexpectedMessage(message);
      return;
    }
    JustVisitingSquare* jail = static_cast<JustVisitingSquare*> (player->GetSquare());
    switch (message->GetType()) {
    case PIMP_JAIL_PAY_BAIL: {
      jail->PlayerWantsToPay(player);
      break;
    }
    case PIMP_JAIL_ROLL_DICE: {
      if (player->GetJailTurns() <= MAX_JAIL_TURNS) {
        int die1, die2;
        RollDice(player, die1, die2);
        if (die1 == die2) {
          // doubles, you are free
          jail->PlayerSetFree(player);
          Broadcast(PIMP_ROLL_AGAIN_FACTORY(player->GetID()));
          mStatus = status_playAgain;
        } else {
          // not doubles; increase turn count
          player->SetJailTurns(player->GetJailTurns()+1);
          if (player->GetJailTurns() > MAX_JAIL_TURNS) {
            // that was the last turn
            jail->PlayerWantsToPay(player);
          } else {
            // better luck again next time
            NextTurn();
          }
        }
      } else {
        // got to pay after three turns.
        UnexpectedMessage(message);
      }
      break;
    }
    }
  } else {
    // not your turn
    UnexpectedMessage(message);
  }
}

void Board::DispatchTransaction(Message* message)
{
  // all those messages start with an unsigned int, so:
  PIMP_TRANSACTION_GENERIC_CAST(message, details);
  Transaction* transaction = GetTransaction(details->GetField1());
  if (transaction) {
    // transaction checks player is involved
    transaction->Dispatch(message);
    if (transaction->GetEnded()) {
      if (transaction == mBlockingTransaction) {
        // we've cleared the blockage
        if (GetPlayingPlayersCount() > 1)
          NextTurn();
        mBlockingTransaction = NULL;
      }
      RemoveTransaction(transaction);
      delete transaction;
    }
  } else {
    InvalidPayload(message);
  }
}

void Board::DispatchTradeRequest(Message* message)
{
  PIMP_TRANSACTION_REQUEST_TRADE_CAST(message, details);
  Player* player0 = GetPlayer(details->GetPlayer());
  Player* player1 = GetPlayer(details->GetField1());
  if (!player0 || !player1 || player0 == player1 || !player0->IsPlaying() || !player1->IsPlaying()) {
    InvalidPayload(message);
    return;
  }
  Transaction* transaction = new TradeTransaction(this, player0, player1);
  AddTransaction(transaction, false);
}

void Board::DispatchClaims(Message* message)
{
  // XXX can this ever find a claim with a non-playing player?
  std::list<Claim*>::iterator i;
  for (i = mClaims.begin(); i != mClaims.end(); ++i) {
    if ((*i)->Dispatch(message)) {
      Claim* claim = (*i);
      mClaims.erase(i);
      delete claim;
      return;
    }
  }
  if (message->GetType() == PIMP_CLAIM_RENT) {
    Send(PIMP_ERROR_INVALID_RENT_CLAIM_FACTORY(), message->GetPlayer());
  } else {
    Send(PIMP_ERROR_INVALID_GO_CLAIM_FACTORY(), message->GetPlayer());
  }
}

void Board::DispatchPropertySale(Message* message)
{
  if (mStatus != status_propertySale ||
      message->GetPlayer() != mCurrentPlayer->GetID()) {
    UnexpectedMessage(message);
    return;
  }
  switch (message->GetType()) {
  case PIMP_BUY_PROPERTY: {
    int price = mPropertyOnSale->GetPrice();
    Transaction* t = GetTransactionBlockingAgreement(mCurrentPlayer, -price);
    if (t) {
      Send(PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH_FACTORY(mPropertyOnSale->GetID(),
                                                              t->GetID()),
           message->GetPlayer());
    } else if (!mCurrentPlayer->HasCash(price)) {
      Send(PIMP_ERROR_PROPERTY_TOO_EXPENSIVE_FACTORY(mPropertyOnSale->GetID(), price),
           message->GetPlayer());
    } else {
      Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(mCurrentPlayer->GetID(),
                                               mPropertyOnSale->GetSquare()->GetID(),
                                               price));
      Transfer(mCurrentPlayer, BANK, price);
      Transfer(BANK, mCurrentPlayer, mPropertyOnSale);
      NextTurn();
    }
    break;
  }
  case PIMP_AUCTION_PROPERTY: {
    AuctionProperty(mPropertyOnSale);
    break;
  }
  }
}

void Board::DispatchPropertyAuction(Message* message)
{
  if ((mStatus != status_propertyAuction) ||
      (mLeadingBidder &&
       (message->GetPlayer() == mLeadingBidder->GetID()))) {
    UnexpectedMessage(message);
    return;
  }
  Player* player = GetPlayer(message->GetPlayer());
  if (!player->IsPlaying()) {
    UnexpectedMessage(message);
    return;
  }
  switch (message->GetType()) {
  case PIMP_BID: {
    PIMP_BID_CAST(message, details);
    int bid = details->GetField1();
    if (bid > mBid) {
      mLeadingBidder = player;
      mBid = bid;
      if (mLeadingBidder->Bid())
        --mNoBids;
      Broadcast(PIMP_PROPERTY_AUCTION_BID_FACTORY(mLeadingBidder->GetID(), mBid));
      CheckAuction();
    } else if (bid > 0) {
      Send(PIMP_PROPERTY_AUCTION_BID_FACTORY(mLeadingBidder->GetID(), mBid),
           message->GetPlayer());
    } else {
      InvalidPayload(message);
    }
    break;
  }
  case PIMP_NO_BID: {
    // can't get here if player is leading bidder
    if (player->NoBid()) {
      ++mNoBids;
      Broadcast(PIMP_PROPERTY_AUCTION_NO_BID_FACTORY(player->GetID()));
      CheckAuction();
    }
    break;
  }
  }
}

void Board::DispatchPurchaseHouses(Message* message)
{
  PIMP_PURCHASE_HOUSES_CAST(message, details);
  int count = details->GetField1();
  Property** properties = new Property*[count];
  int* amountHouses = new int[count];
  int* amountHotels = new int[count];
  int i;
  for (i = 0; i < count; ++i) {
    properties[i] = GetProperty(details->GetField2()[i]);
    amountHouses[i] = details->GetField3()[i];
    amountHotels[i] = details->GetField4()[i];
    if (!properties[i]) {
      InvalidPayload(message);
      break;
    }
  }
  if (i == count)
    PurchaseHouses(GetPlayer(message->GetPlayer()), count, properties, amountHouses, amountHotels);
  delete[] properties;
  delete[] amountHouses;
  delete[] amountHotels;
}

void Board::DispatchMortgages(Message* message)
{
  PIMP_MORTGAGE_PROPERTY_CAST(message, details);
  Property* property = GetProperty(details->GetField1());
  Player* player = GetPlayer(message->GetPlayer());
  if (!property) {
    InvalidPayload(message);
    return;
  }
  if (property->GetOwner() != player) {
    UnexpectedMessage(message);
    return;
  }
  switch (message->GetType()) {
  case PIMP_MORTGAGE_PROPERTY:
    if (property->IsMortgaged() ||
        property->GetNumHouses() || property->GetNumHotels()) {
      UnexpectedMessage(message);
      return;
    }
    Broadcast(PIMP_SQUARE_GIVES_CASH_FACTORY(player->GetID(),
                                             property->GetSquare()->GetID(),
                                             property->MortgageValue()));
    property->Mortgage(this);
    Transfer(BANK, player, property->MortgageValue(), overrideEscrow);
    break;
  case PIMP_UNMORTGAGE_PROPERTY:
    if (!property->IsMortgaged()) {
      UnexpectedMessage(message);
      return;
    }
    int cost = property->UnmortgageCost();
    if (!player->HasCash(cost)) {
      Send(PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE_FACTORY(property->GetID(), cost),
           message->GetPlayer());
      return;
    }
    Broadcast(PIMP_SQUARE_TAKES_CASH_FACTORY(player->GetID(),
                                             property->GetSquare()->GetID(),
                                             cost));
    property->Unmortgage(this);
    Transfer(player, BANK, cost);
    break;
  }
}

void Board::DispatchSquare(Message* message)
{
  Player* player = GetPlayer(message->GetPlayer());
  if (mStatus != status_squareBlocking || player != mCurrentPlayer)
    UnexpectedMessage(message);
  try {
    mCurrentPlayer->GetSquare()->Dispatch(player, message);
    if (mStatus == status_idle)
      NextTurn();
  } catch (UnexpectedMessageError) {
    UnexpectedMessage(message);
  }
}

void Board::UnexpectedMessage(Message* message)
{
  Send(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()), message->GetPlayer());
}

void Board::InvalidPayload(Message* message)
{
  Send(PIMP_ERROR_INVALID_PAYLOAD_FACTORY(message->GetType()), message->GetPlayer());
}

void Board::Send(Message* message)
{
  mMessageQueue.push_back(message);
}

void Board::Send(Message* message, int userID)
{
  message->SetPlayer(userID);
  Send(message);
}

void Board::Broadcast(Message* message)
{
  int i;
  for (i = 0; i < mPlayers.size(); ++i) {
    if (mPlayers[i]) {
      Message* playerMessage = message->Copy();
      playerMessage->SetPlayer(mPlayers[i]->GetID());
      Send(playerMessage);
    }
  }
  delete message;
}

Message* Board::PopQueue()
{
  if (mMessageQueue.size()) {
    Message* message = mMessageQueue.front();
    mMessageQueue.pop_front();
    return message;
  } else {
    return NULL;
  }
}

void Board::TellPlayerState(Player* player)
{
  // Send S=>C messages
  switch (mStatus) {
  case status_idle:
    // Nothing to report
    // XXX
    break;
  case status_startOfTurn:
    Send(PIMP_START_OF_TURN_FACTORY(mCurrentPlayer->GetID(), mCurrentPlayer->GetJailTurns() == 0),
         player->GetID());
    if (mCurrentPlayer->GetJailTurns() > 0) {
      Send(PIMP_JAIL_PAY_OR_ROLL_FACTORY(mCurrentPlayer->GetID(),
                                         MAX_JAIL_TURNS - mCurrentPlayer->GetJailTurns() + 1,
                                         mCanBuyHouses),
           player->GetID());
    }
    break;
  case status_playAgain:
    Send(PIMP_ROLL_AGAIN_FACTORY(mCurrentPlayer->GetID()),
         player->GetID());
    break;
  case status_transactionBlocking:
    Send(PIMP_WAITING_FOR_TRANSACTION_FACTORY(mCurrentPlayer->GetID(),
                                              mBlockingTransaction->GetID()),
         player->GetID());
    break;
  case status_propertySale:
    Send(PIMP_PROPERTY_SALE_FACTORY(mCurrentPlayer->GetID(), mPropertyOnSale->GetID(), mPropertyOnSale->GetPrice()),
         player->GetID());
    break;
  case status_propertyAuction:
    Send(PIMP_PROPERTY_AUCTION_FACTORY(mPropertyOnSale->GetID()),
         player->GetID());
    if (mLeadingBidder)
      Send(PIMP_PROPERTY_AUCTION_BID_FACTORY(mLeadingBidder->GetID(), mBid),
           player->GetID());
    int i;
    for (i = 0; i < mPlayers.size(); ++i) {
      if (mPlayers[i] && mPlayers[i]->IsPlaying() && !mPlayers[i]->GetBidding()) {
        Send(PIMP_PROPERTY_AUCTION_NO_BID_FACTORY(mPlayers[i]->GetID()),
           player->GetID());
      }
    }
    break;
  case status_squareBlocking:
    mCurrentPlayer->GetSquare()->RequestState(player);
    break;
  default:
    break;
  }
}

void Board::AddPlayer(Player* player)
{
  int id = player->GetID();
  if (id > mPlayers.size()) {
    mPlayers.resize(id);
  }
  mPlayers[id-1] = player;
}

bool Board::HasProperty(Property* property)
{
  return property->GetOwner() == BANK;
}

void Board::AddCard(Card* card)
{
  card->GetPile()->push_back(card);
}

void Board::RemoveCard(Card* card)
{
  std::list<Card*>* pile = card->GetPile();
  std::list<Card*>::iterator i;
  for (i = pile->begin(); i != pile->end(); ++i) {
    if ((*i) == card) {
      pile->erase(i);
      return;
    }
  }
}

bool Board::HasCard(Card* card)
{
  return card->GetOwner() == BANK;
}

void Board::RollDice(Player* player, int& die1, int& die2) {
  die1 = rand() % 6 + 1;
  die2 = rand() % 6 + 1;
  Broadcast(PIMP_DICE_ROLLED_FACTORY(player->GetID(), die1, die2));
}
