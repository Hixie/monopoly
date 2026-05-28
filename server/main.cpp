
#include <iostream>
#include <string>
#include "board.h"
#include "message.h"
#include "network.h"

// random number
#define PORT 13220

bool promptBool(char* prompt, char yes, char no) {
  cout << prompt << " [" << yes << "/" << no << "] ";
  char c;
  do {
    cin.get(c);
  } while (c != yes && c != no);
  cout << endl;
  return c == yes;
}

int main(int argc, char *argv [])
{
  cout << "Kaspar PIMP Server version 1.0" << endl;
  srand(time(NULL));
  DEBUG_MESSAGES = true;
  bool optionsSet = false;
  bool potEnabled, needToPassGoToBuy, doubleGoMoneyEnabled, bankruptcyTransfersToPlayer;
  int port = PORT;
  if (argc > 1) {
    port = atoi(argv[1]);
  }
  if (argc < 3 && cin.good()) {
    potEnabled = promptBool("Enable the pot on Free Parking?", 'y', 'n');
    needToPassGoToBuy = promptBool("Require that players pass Go before buying property?", 'y', 'n');
    doubleGoMoneyEnabled = promptBool("Double the Go money when players land on it?", 'y', 'n');
    bankruptcyTransfersToPlayer = promptBool("Should bankruptcy transfer to the player, or the bank?", 'p', 'b');
    optionsSet = true;
  } else {
    int opts = atoi(argv[2]);
    potEnabled = (opts & 0x01) > 0;
    needToPassGoToBuy = (opts & 0x02) > 0;
    doubleGoMoneyEnabled = (opts & 0x04) > 0;
    bankruptcyTransfersToPlayer = (opts & 0x08) == 0;
    optionsSet = true;
  }

  // server loop: each time a game ends, restart the server ready for the next one
  bool serverRunning = true;
  do {
    Board* board = new Board();
    if (optionsSet) {
      if (potEnabled) cout << "Pot Enabled." << endl;
      if (needToPassGoToBuy) cout << "Players need to pass Go before buying properties." << endl;
      if (doubleGoMoneyEnabled) cout << "Landing on Go doubles Go money." << endl;
      if (bankruptcyTransfersToPlayer)
        cout << "Bankruptcy transfers to the player." << endl;
      else
        cout << "Bankruptcy transfers to the bank." << endl;
      board->OverrideSettings(potEnabled, needToPassGoToBuy,
                              doubleGoMoneyEnabled, bankruptcyTransfersToPlayer);
    }
    Network* network = new Network(port, time(NULL));
    // event loop
    do {
      Message* message = network->GetMessage();
      if (message) {
        board->Dispatch(message);
        delete message;
        while (message = board->PopQueue()) {
          network->Send(message);
          delete message;
        }
      }
    } while ((network->NumConnections() > 1) || (!board->IsGameOver()));
    delete network;
    delete board;
  } while (serverRunning);
  return 0;
}
