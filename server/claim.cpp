
#include "card.h"
#include "player.h"
#include "transaction.h"
#include "board.h"
#include "claim.h"

Claim::Claim(Board* board) : mBoard(board)
{
  mID = Transaction::NewID();
}

Claim::~Claim()
{
}

RentClaim::RentClaim(Board* board, Player* debtor, Player* creditor, Property* property, int rent) : Claim(board),
                                                                                                     mDebtor(debtor),
                                                                                                     mCreditor(creditor),
                                                                                                     mProperty(property),
                                                                                                     mRent(rent)
{
}
  
bool RentClaim::Dispatch(Message* message)
{
  if (message->GetType() == PIMP_CLAIM_RENT) {
    PIMP_CLAIM_RENT_CAST(message, details);
    if (details->GetPlayer() == mCreditor->GetID() &&
        details->GetField1() == mDebtor->GetID() &&
        details->GetField2() == mProperty->GetID()) {
      mBoard->Broadcast(PIMP_PLAYER_CLAIMED_RENT_FACTORY(mCreditor->GetID(),
                                                         mDebtor->GetID(),
                                                         mProperty->GetID(),
                                                         mRent));
      mBoard->AddTransaction(new RentTransaction(mBoard, mID, mCreditor, mDebtor, mProperty, mRent), false);
      return true;
    }
  }
  return false;
}

bool RentClaim::TransferNotification(Player* from, Player* to)
{
  if (mCreditor == from)
    mCreditor = to;
  if (mDebtor == from)
    mDebtor = to;
  return mCreditor && mDebtor && mCreditor != mDebtor;
}


GoClaim::GoClaim(Board* board, Player* claimant, Square* square, int salary) : Claim(board),
                                                                               mClaimant(claimant),
                                                                               mSquare(square),
                                                                               mSalary(salary)
{
}
  
bool GoClaim::Dispatch(Message* message)
{
  if (message->GetType() == PIMP_CLAIM_GO) {
    PIMP_CLAIM_GO_CAST(message, details);
    if (details->GetPlayer() == mClaimant->GetID() &&
        details->GetField1() == mSquare->GetID()) {
      mBoard->Broadcast(PIMP_PLAYER_CLAIMED_GO_FACTORY(mClaimant->GetID(),
                                                       mSquare->GetID(),
                                                       mSalary));
      mBoard->Broadcast(PIMP_SQUARE_GIVES_CASH_FACTORY(mClaimant->GetID(), mSquare->GetID(), mSalary));
      mBoard->Transfer(BANK, mClaimant, mSalary, mID);
      return true;
    }
  }
  return false;
}

bool GoClaim::TransferNotification(Player* from, Player* to)
{
  if (mClaimant == from)
    mClaimant = to;
  return mClaimant;
}
