
#include <iostream>
#include <termios.h>
#include <stdio.h>
#include <unistd.h>
#include "../../server/message.h"
#include "../../server/exceptions.h"
#include "network.h"

// same as server
#define PORT 13220

/* STATE */

Network* network = NULL;
bool loop = true;
char* lastPrompt;

// connection status
enum statuses {
  status_handshake, // just sent handshake
  status_joining,
  status_rejoin,
  status_gameState,
  status_waiting,
  status_throwDice,
  status_sale,
  status_auction,
  status_jail,
  status_tax
} status = status_handshake;

// player data
unsigned int gameNumber = 0;
unsigned int player;
unsigned int password;

// player prefs
char name[MAX_STRING_LENGTH+1] = "";
int piece = 0x00;
bool playing = true;

// various game state
int currentPlayer = 0;
int candidate = 0;
int bid = 0;

/* UTILITY FUNCTIONS */

void prompt(char* string, statuses newStatus) {
  lastPrompt = string;
  cout << string << endl;
  status = newStatus;
}

void reprompt() {
  cout << lastPrompt << endl;
}

int promptInt(char* string) {
  int i;
  cout << string << " ";
  cin >> i;
  while (cin.fail()) {
    cin.clear();
    cin.get(); // remove the non-digit character
    cout << endl << string << " [enter a valid number] ";
    cin >> i;
  }
  if (cin.peek() == '\n') {
    char c;
    cin.get(c);
  } else {
    cout << endl;
  }
  return i;
}

void unexpectedPacket(Message* message) {
  network->SendAndDeleteMessage(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
}

void getRejoinData() {
  char c;
  do {
    cout << "Rejoin an existing game, or join a new one? [r/j] ";
    cin.get(c);
    cout << endl;
  } while (c != 'r' && c != 'j');
  if (c == 'r') {
    gameNumber = promptInt("What was the game number?");
    player = promptInt("What player number were we?");
    password = promptInt("What was our password?");
  }
}

void connect() {
  if (network) {
    free(network);
    network = NULL;
    cerr << "disconnected" << endl;
  }
  cerr << "connecting";
  while (!network) {
    try {
      network = new Network("monopoly.damowmow.com", PORT);
    } catch(...) {
      network = NULL;
      cerr << ".";
      sleep(1);
    }
  }
  cerr << endl;
  // send the handshake packet
  network->SendAndDeleteMessage(PIMP_HANDSHAKE_FACTORY(1));
  status = status_handshake;
  cerr << "connected" << endl;
}

void join() {
  cout << "Name: ";
  // should use readline
  if (strlen(name)) {
    cout << name;
    if (playing)
      cout << " (playing)";
    else
      cout << " (observing)";
    cout << endl;
    network->SendAndDeleteMessage(PIMP_JOIN_FACTORY(piece, playing, name));
  } else {
    cin.getline(name, MAX_STRING_LENGTH+1);
    if (strlen(name) >= MAX_STRING_LENGTH) {
      cout << endl;
    }
    char c;
    do {
      cout << "Observe, or Play? [o/p] ";
      cin.get(c);
      cout << endl;
    } while (c != 'o' && c != 'p');
    switch (c) {
    case 'o':
      // observer
      network->SendAndDeleteMessage(PIMP_JOIN_FACTORY(piece, false, name));
      playing = false;
      break;
    case 'p':
      // player
      network->SendAndDeleteMessage(PIMP_JOIN_FACTORY(piece, true, name));
      playing = true;
      break;
    }
  }
}

void trade() {
  char c;
  do {
    cout << "Trade number ('n' to start a new trade): ";
    cin.get(c);
    cout << endl;
  } while (c != 'n' && (c < '0' || c > '9'));
  if (c == 'n') {
    // create a new trade
    int other = promptInt("Start trade with which player?");
    network->SendAndDeleteMessage(PIMP_TRANSACTION_REQUEST_TRADE_FACTORY(other));
  } else {
    // get the trade number
    int id = 0;
    do {
      id *= 10;
      id += c - '0';
      cin.get(c);
    } while (c >= '0' && c <= '9');
    while (c < 'a' || c > 'z') {
      cout << "m: set money; p: add property; y: remove property; c: add card; d: remove card" << endl;
      cout << "f: finish; r: reopen; a: agreed; b: bankrupt; z: cancel" << endl;
      cin.get(c);
    }
    cout << endl;
    switch (c) {
    case 'm':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_SET_CASH_FACTORY(id, promptInt("Set cash to how much?")));
      break;
    case 'p':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_ADD_PROPERTY_FACTORY(id, promptInt("Add which property?")));
      break;
    case 'y':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_REMOVE_PROPERTY_FACTORY(id, promptInt("Remove which property?")));
      break;
    case 'c':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_ADD_CARD_FACTORY(id, promptInt("Add which card?")));
      break;
    case 'd':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_REMOVE_CARD_FACTORY(id, promptInt("Remove which card?")));
      break;
    case 'f':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_FINISH_FACTORY(id));
      break;
    case 'r':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_REOPEN_FACTORY(id));
      break;
    case 'a':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_AGREE_FACTORY(id));
      break;
    case 'b':
      network->SendAndDeleteMessage(PIMP_BANKRUPT_TRANSACTION_FACTORY(id));
      break;
    case 'z':
      network->SendAndDeleteMessage(PIMP_TRANSACTION_CANCEL_FACTORY(id));
      break;
    default:
      cout << "no action taken." << endl;
      break;
    }
  }
}

void houses() {
  cout << "House Construction" << endl;
  int count = promptInt("How many properties do you want to change?");
  if (count > 0 && count < 255) {
    int* properties = new int[count];
    int* amountHouses = new int[count];
    int* amountHotels = new int[count];
    int i;
    for (i = 0; i < count; ++i) {
      properties[i] = promptInt("Property?");
      amountHouses[i] = promptInt("Build how many houses?");
      amountHotels[i] = promptInt("Build how many hotels?");
    }
    network->SendAndDeleteMessage(PIMP_PURCHASE_HOUSES_FACTORY(count, properties, amountHouses, amountHotels));
    delete[] properties;
    delete[] amountHouses;
    delete[] amountHotels;
  }
}

void mortgage() {
  int p = promptInt("Which property do you want to change?");
  char c;
  do {
    cout << "Do you want to mortgage, or unmortgage the property? [m/u]";
    cin.get(c);
    cout << endl;
  } while (c != 'm' && c != 'u');
  if (c == 'm') {
    network->SendAndDeleteMessage(PIMP_MORTGAGE_PROPERTY_FACTORY(p));
  } else {
    network->SendAndDeleteMessage(PIMP_UNMORTGAGE_PROPERTY_FACTORY(p));
  }
}


/* HANDLERS */

void handleHandshake(Message* message) {
  PIMP_HANDSHAKE_ACKNOWLEDGE_CAST(message, handshake);
  if (gameNumber == handshake->GetField1()) {
    // rejoin
    status = status_rejoin;
    network->SendAndDeleteMessage(PIMP_REJOIN_FACTORY(player, password));
  } else {
    // new game
    status = status_joining;
    gameNumber = handshake->GetField1();
    cout << endl << "New Game. (number " << gameNumber << ")" << endl;
    join();
  }
}

void handleWelcome(Message* message) {
  PIMP_WELCOME_DETAILS_CAST(message, details);
  player = details->GetField1();
  password = details->GetField2();
  cout << "Welcome to the game. (player number " << player << " with password " << password << ")" << endl;
  status = status_gameState;
}


/* MAIN EVENT LOOP */

// XXX handle disconnect errors that occur during sends

// hide the goto statement so as to make us look more structural...
#define end goto always

int main()
{
  cout << "Kaspar PIMP Command Line Client version 1.0" << endl;

  // Set the TTY settings so we have no line buffering

  termios newtty; // Declare a termios structure for the new TTY settings
  termios oldtty; // ...and one to save the current settings.

  if (setvbuf(stdin, NULL, _IONBF, 0)) { // Turn off line buffering on the input stream
    cerr << "Failed to turn of line buffering." << endl;
    throw IOConfigurationError();
  }

  if (tcgetattr(0, &oldtty) == -1) { // Get the current TTY attributes.
    cerr << "Unable to save the TTY attributes." << endl;
    throw IOConfigurationError();
  }

  newtty = oldtty; // Start out with the same TTY attributes.
  newtty.c_lflag &= ~ICANON; // Turn of the wait for end of line (EOL).
  newtty.c_cc[VMIN] =  1; // Set the minimum character buffer.
  newtty.c_cc[VTIME] =  0; // Set the minimum wait time.

  if (tcsetattr(0, TCSAFLUSH, &newtty) == -1) { // Set our new tty attributes.
    cerr << "Unable to set the TTY attributes." << endl;
    throw IOConfigurationError();
  }

  getRejoinData();

  // set up the network object
  connect();
  do {
    Message* message;
    try {
      message = network->GetMessage();
    } catch(NetworkDisconnectedError) {
      connect();
      continue;
    } catch(MessageError) {
      network->SendAndDeleteMessage(PIMP_ERROR_UNPARSEABLE_FACTORY());
      continue;
    }
    if (message) {
      switch (message->GetType()) {
      case PIMP_ERROR_UNEXPECTED_MESSAGE: {
        PIMP_ERROR_UNEXPECTED_MESSAGE_CAST(message, details);
        cerr << "/!\\ server complained of unexpected message: " << getMessageName(details->GetField1()) << endl;
        end;
      }
      case PIMP_ERROR_INVALID_PAYLOAD:
      case PIMP_ERROR_UNPARSEABLE:
        cerr << "error packet: ignored" << endl;
        end;
      case PIMP_JOIN_PENDING:
        cout << "Joining..." << endl;
        end;
      case PIMP_QUERY_JOIN_PLAY:
      case PIMP_QUERY_JOIN_OBSERVE: {
        // we assume that PIMP_QUERY_JOIN_PLAY and PIMP_QUERY_JOIN_OBSERVE
        // have the same data structure and can thus be cast to the same
        // class:
        PIMP_QUERY_JOIN_PLAY_CAST(message, details);
        char* name = details->GetField2();
        cout << "Someone calling themselves '" << name << "' wants to ";
        if (details->GetType() == PIMP_QUERY_JOIN_PLAY)
          cout << "play";
        else
          cout << "observe";
        cout << '.' << endl;
        free(name);
        cout << "Accept, or Refuse? [a/r]" << endl;
        candidate = details->GetField1();
        end;
      }
      case PIMP_JOIN_REFUSED: {
        PIMP_JOIN_REFUSED_CAST(message, details);
        char* name = details->GetField2();
        cout << "Person with name '" << name << "' was refused." << endl;
        free(name);
        candidate = 0;
        end;
      }
      case PIMP_WELCOME_PLAYER: {
        PIMP_WELCOME_PLAYER_CAST(message, details);
        char* name = details->GetField3();
        cout << "Player " << (int)details->GetField1() << ": "
             << "Piece " << (int)details->GetField2() << ", "
             << "called " << name << "." << endl;
        cout << "Player " << (int)details->GetField1() << " is at square 0." << endl;
        free(name);
        candidate = 0;
        end;
      }
      case PIMP_WELCOME_OBSERVER: {
        PIMP_WELCOME_OBSERVER_CAST(message, details);
        char* name = details->GetField3();
        cout << "Observer " << (int)details->GetField1() << ": "
             << "Piece " << (int)details->GetField2() << ", "
             << "called " << name << "." << endl;
        free(name);
        candidate = 0;
        end;
      }
      case PIMP_PLAYER_BECAME_OBSERVER_KICKED:
      case PIMP_PLAYER_BECAME_OBSERVER_TRANSFER:
      case PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT: {
        PIMP_PLAYER_BECAME_OBSERVER_KICKED_CAST(message, details);
        // first person alternative XXX
        cout << "Player " << (int)details->GetField1() << " has become an observer." << endl;
        switch (message->GetType()) {
        case PIMP_PLAYER_BECAME_OBSERVER_KICKED:
          cout << "They were kicked." << endl;
          break;
        case PIMP_PLAYER_BECAME_OBSERVER_TRANSFER:
          cout << "They quit." << endl;
          break;
        case PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT: 
          cout << "They went bankrupt." << endl;
          break;
        }
        if (details->GetField1() == player)
          playing = false;
        end;
      }
      case PIMP_OBSERVER_BECAME_PLAYER: {
        PIMP_OBSERVER_BECAME_PLAYER_CAST(message, details);
        // first person alternative XXX
        cout << "Observer " << (int)details->GetField1() << " has become a player." << endl;
        cout << "Player " << (int)details->GetField1() << " is at square 0." << endl;
        if (details->GetField1() == player)
          playing = true;
        end;
      }
      case PIMP_PLAYER_TAKEOVER: {
        PIMP_PLAYER_TAKEOVER_CAST(message, details);
        cout << "Player " << (int)details->GetField2() << " is giving everything to observer "
             << (int)details->GetField1() << "." << endl;
        if (details->GetField1() == player)
          playing = true;
        end;
      }
      case PIMP_PLAYER_WON: {
        PIMP_PLAYER_WON_CAST(message, details);
        cout << "Player " << (int)details->GetField1() << " won." << endl;
        end;
      }
      case PIMP_HANDSHAKE_ACKNOWLEDGE:
        handleHandshake(message);
        end;
      case PIMP_ERROR_UNKNOWN_PROTOCOL:
        cerr << "server does not support our protocol" << endl;
        loop = false;
        end;
      case PIMP_ERROR_NAME_IN_USE:
        cout << "That name is already in use. Pick another." << endl;
        name[0] = '\0';
        join();
        end;
      case PIMP_ERROR_TOO_MANY_USERS:
        cout << "Server is full." << endl;
        loop = false;
        end;
      case PIMP_ERROR_NOT_WELCOME:
        cout << "You are not welcome on this server." << endl;
        loop = false;
        end;
      case PIMP_WELCOME_DETAILS:
        handleWelcome(message);
        end;
      case PIMP_WELCOME_BACK: {
        PIMP_WELCOME_BACK_CAST(message, details);
        char* name = details->GetField3();
        cout << "We have rejoined as "
             << (details->GetField4() ? "observer " : "player ")
             << (int)details->GetField1() << ": "
             << "Piece " << (int)details->GetField2() << ", "
             << "called " << name << "." << endl;
        free(name);
        status = status_gameState;
        end;
      }
      case PIMP_ERROR_WRONG_PASSWORD:
        status = status_joining;
        cout << "We are no longer recognised in this game." << endl;
        join();
        end;
      case PIMP_ERROR_INVALID_RENT_CLAIM: {
        PIMP_ERROR_INVALID_RENT_CLAIM_CAST(message, details);
        cout << "You aren't owed any rent." << endl;
        end;
      }
      case PIMP_ERROR_INVALID_GO_CLAIM: {
        PIMP_ERROR_INVALID_GO_CLAIM_CAST(message, details);
        cout << "You aren't owed any salary." << endl;
        end;
      }
      case PIMP_STATE_BOARD: {
        PIMP_STATE_BOARD_CAST(message, details);
        if (details->GetField1() != 0) {
          // don't support anything but board 0
          cout << "This server is using an unknown board." << endl;
          loop = false;
        }
        status = status_gameState;
        cout << endl;
        cout << "Game State" << endl;
        cout << "==========" << endl;
        cout << "Board: 0. We are player " << player << "." << endl;
        end;
      }
      case PIMP_STATE_OBSERVER: {
        PIMP_STATE_OBSERVER_CAST(message, details);
        char* name = details->GetField3();
        cout << "Observer " <<(int) details->GetField1() << ": "
             << "Piece " << (int)details->GetField2() << ", "
             << "called " << name << "." << endl;
        free(name);
        end;
      }
      case PIMP_STATE_PLAYER: {
        PIMP_STATE_PLAYER_CAST(message, details);
        char* name = details->GetField3();
        cout << "Player " << (int)details->GetField1() << ": "
             << "Piece " << (int)details->GetField2() << ", "
             << "called " << name << "." << endl;
        cout << "Player " << (int)details->GetField1() << " is at square "
             << (int)details->GetField4() << "." << endl;
        cout << "Player " << (int)details->GetField1() << " has $"
             << details->GetField5() << "." << endl;
        if (details->GetField6()) {
          cout << "Player " << (int)details->GetField1() << " is in jail for turn "
               << (int)details->GetField6() << "." << endl;
        }
        free(name);
        end;
      }
      case PIMP_STATE_PROPERTY: {
        PIMP_STATE_PROPERTY_CAST(message, details);
        cout << "Property " << (int)details->GetField1() << ": "
             << "Owned by player " << (int)details->GetField2() << ", "
             << (details->GetField3() ? "is mortgaged, " : "")
             << "with " << (int)details->GetField4() << " houses "
             << "and " << (int)details->GetField5() << " hotels." << endl;
        end;
      }
      case PIMP_STATE_CARD: {
        PIMP_STATE_CARD_CAST(message, details);
        cout << "Card " << (int)details->GetField1() << ": "
             << "Owned by player " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_STATE_POT: {
        PIMP_STATE_POT_CAST(message, details);
        cout << "Pot: $" << details->GetField1() << endl;
        cout << endl;
        status = status_waiting;
        end;
      }
      case PIMP_START_OF_TURN: {
        PIMP_START_OF_TURN_CAST(message, details);
        if (details->GetField1() == player) {
          if (details->GetField2()) {
            prompt("Our turn. Hit 'd' to throw the dice.", status_throwDice);
          } else {
            cout << "Our turn. Awaiting further instructions..." << endl;
          }
        } else {
          cout << "It's now the turn of player " << (int)details->GetField1() << "." << endl;
        }
        currentPlayer = details->GetField1();
        end;
      }
      case PIMP_DICE_ROLLED: {
        PIMP_DICE_ROLLED_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We threw the dice and got ";
        } else {
          cout << "Player " << (int)details->GetField1() << " threw the dice and got ";
        }
        cout << (int)details->GetField2() << " and "
             << (int)details->GetField3() << "." << endl;
        end;
      }
      case PIMP_DICE_MOVED_PLAYER: {
        PIMP_DICE_MOVED_PLAYER_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are moving to square ";
        } else {
          cout << "Player " << (int)details->GetField1() << " is moving to square ";
        }
        cout << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_SQUARE_MOVED_PLAYER: {
        PIMP_SQUARE_MOVED_PLAYER_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are moving to square ";
        } else {
          cout << "Player " << (int)details->GetField1() << " is moving to square ";
        }
        cout << (int)details->GetField3() << " because of square "
             << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_CARD_MOVED_PLAYER: {
        PIMP_CARD_MOVED_PLAYER_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are moving to square ";
        } else {
          cout << "Player " << (int)details->GetField1() << " is moving to square ";
        }
        cout << (int)details->GetField3() << " because of card "
             << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_PLAYER_PASSING_BY_SQUARE: {
        PIMP_PLAYER_PASSING_BY_SQUARE_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "|- We are moving past square ";
        } else {
          cout << "|- Player " << (int)details->GetField1() << " is moving past square ";
        }
        cout << (int)details->GetField2() << "." << endl;
        if (details->GetField1() == player && details->GetField2() == 0) {
        }
        end;
      }
      case PIMP_PLAYER_LANDING_ON_SQUARE: {
        PIMP_PLAYER_LANDING_ON_SQUARE_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "`- We are landing on square ";
        } else {
          cout << "`- Player " << (int)details->GetField1() << " is landing on square ";
        }
        cout << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_THREE_DOUBLES: {
        PIMP_THREE_DOUBLES_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We got ";
        } else {
          cout << "Player " << (int)details->GetField1() << " got ";
        }
        cout << "three doubles." << endl;
        end;
      }
      case PIMP_ROLL_AGAIN: {
        PIMP_ROLL_AGAIN_CAST(message, details);
        if (details->GetField1() == player) {
          prompt("Our turn again. Hit 'd' to throw the dice.", status_throwDice);
        } else {
          cout << "It's again the turn of player " << (int)details->GetField1() << "." << endl;
        }
        end;
      }
      case PIMP_PROPERTY_SALE: {
        PIMP_PROPERTY_SALE_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We have been given an opportunity to purchase property "
               << (int)details->GetField2() << " for $"
               << (int)details->GetField3() << "." << endl;
          prompt("Press 'b' to buy it, 'a' to auction it.", status_sale);
        } else {
          cout << "Player " << (int)details->GetField1()
               << " has been given an opportunity to purchase property "
               << (int)details->GetField2() << " for $"
               << (int)details->GetField3() << "." << endl;
        }
        end;
      }
      case PIMP_ERROR_PROPERTY_TOO_EXPENSIVE: {
        PIMP_ERROR_PROPERTY_TOO_EXPENSIVE_CAST(message, details);
        cout << "Can't afford it, it costs $" << details->GetField2() << "." << endl;
        prompt("Press 'a' to auction it, or 'b' to buy it if you have found the funds.", status_sale);
        end;
      }
      case PIMP_PROPERTY_AUCTION: {
        PIMP_PROPERTY_AUCTION_CAST(message, details);
        cout << "Property " << (int)details->GetField1() << " is going for auction." << endl;
        prompt("Type 'b' to bid, 'n' to retract from the bidding.", status_auction);
        bid = 0;
        end;
      }
      case PIMP_PROPERTY_AUCTION_BID: {
        PIMP_PROPERTY_AUCTION_BID_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are winning the bidding ";
        } else {
          cout << "Player " << (int)details->GetField1() << " is winning the bidding ";
        }
        cout << "with a bid for $" << (int)details->GetField2() << "." << endl;
        bid = details->GetField2();
        end;
      }
      case PIMP_PROPERTY_AUCTION_NO_BID: {
        PIMP_PROPERTY_AUCTION_NO_BID_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are out of the bidding." << endl;
        } else {
          cout << "Player " << (int)details->GetField1() << " is out of the bidding." << endl;
        }
        end;
      }
      case PIMP_PROPERTY_AUCTION_WON: {
        PIMP_PROPERTY_AUCTION_WON_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "Auction over. We won." << endl;
        } else {
          cout << "Auction over. Player " << (int)details->GetField1() << " won." << endl;
        }
        status = status_waiting;
        end;
      }
      case PIMP_PROPERTY_AUCTION_VOID: {
        PIMP_PROPERTY_AUCTION_VOID_CAST(message, details);
        cout << "Auction over. Reserve not met." << endl;
        status = status_waiting;
        end;
      }
      case PIMP_JAIL_PAY_OR_ROLL: {
        PIMP_JAIL_PAY_OR_ROLL_CAST(message, details);
        if (details->GetField1() == player) {
          if (details->GetField2()) {
            prompt("Jail: Type 'b' to pay bail, 'd' to throw dice.", status_jail);
          } else {
            network->SendAndDeleteMessage(PIMP_JAIL_PAY_BAIL_FACTORY());
          }
        } else {
          cout << "Player " << (int)details->GetField1() << " is in jail." << endl;
        }
        end;
      }
      case PIMP_TAX_SELECT_OPTION: {
        PIMP_TAX_SELECT_OPTION_CAST(message, details);
        if (details->GetField1() == player) {
          prompt("Income Tax: Type '$' to pay $200, '%' to pay 10%.", status_tax);
        } else {
          cout << "Player " << (int)details->GetField1() << " is on income tax." << endl;
        }
        end;
      }
      case PIMP_GOT_CARD: {
        PIMP_GOT_CARD_CAST(message, details);
        cout << "Player " << (int)details->GetField1()
             << " just picked up card " << (int)details->GetField3()
             << " at square " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_GOING_TO_JAIL: {
        PIMP_GOING_TO_JAIL_CAST(message, details);
        if (details->GetField1() == player) {
          cout << "We are going to jail." << endl;
        } else {
          cout << "Player " << (int)details->GetField1() << " is going to jail." << endl;
        }
        end;
      }
      case PIMP_JAIL_FREE: {
        PIMP_JAIL_FREE_CAST(message, details);
        cout << "Player " << (int)details->GetField1() << " has been released from jail." << endl;
        end;
      }
      case PIMP_TRANSACTION_TRADE_REQUESTED: {
        PIMP_TRANSACTION_TRADE_REQUESTED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "Started with player " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_TRANSACTION_RENT_REQUESTED: {
        PIMP_TRANSACTION_RENT_REQUESTED_CAST(message, details);
        if (details->GetField4() == player) {
          cout << "Trade " << details->GetField1() << ": "
               << "Rent request from us to player " << (int)details->GetField2()
               << " for landing on property " << (int)details->GetField3() << "." << endl
               << "Rent on this property is $" << details->GetField5() << "." << endl;
        } else {
          cout << "Trade " << details->GetField1() << ": "
               << "Rent request from player " << (int)details->GetField2()
               << " for landing on property " << (int)details->GetField3() << "." << endl
               << "Rent on this property is $" << details->GetField5() << "." << endl;
        }
        end;
      }
      case PIMP_TRANSACTION_CARD_REQUESTED: {
        PIMP_TRANSACTION_CARD_REQUESTED_CAST(message, details);
        if (details->GetField4() == player) {
          cout << "Trade " << (int)details->GetField1() << ": "
               << "Player " << (int)details->GetField2()
               << " owes us $" << details->GetField5()
               << " because of card " << (int)details->GetField3() << "." << endl;
        } else {
          cout << "Trade " << details->GetField1() << ": "
               << "We owe player " << (int)details->GetField2()
               << " $" << details->GetField5()
               << " because of card " << (int)details->GetField3() << "." << endl;
        }
        end;
      }
      case PIMP_TRANSACTION_SQUARE_REQUESTED: {
        PIMP_TRANSACTION_SQUARE_REQUESTED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We owe square " << (int)details->GetField2()
             << " the amount of $" << details->GetField3() << "." << endl;
        end;
      }
      case PIMP_TRANSACTION_BANK_REQUESTED: {
        PIMP_TRANSACTION_BANK_REQUESTED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We owe the bank the amount of $" << details->GetField2() << "." << endl;
        end;
      }
      case PIMP_TRANSACTION_JAIL_REQUESTED: {
        PIMP_TRANSACTION_JAIL_REQUESTED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We owe the jail the amount of $" << details->GetField2() << " or a get out of jail free card." << endl;
        end;
      }
      case PIMP_TRANSACTION_CASH_SET: {
        PIMP_TRANSACTION_CASH_SET_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have set our offer to $" << details->GetField2() << "." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_CASH_SET: {
        PIMP_TRANSACTION_OTHER_CASH_SET_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have set their offer to $" << details->GetField2() << "." << endl;
        end;
      }
      case PIMP_TRANSACTION_PROPERTY_ADDED: {
        PIMP_TRANSACTION_PROPERTY_ADDED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have put property " << (int)details->GetField2() << " on the table." << endl;
        end;
      }
      case PIMP_TRANSACTION_PROPERTY_REMOVED: {
        PIMP_TRANSACTION_PROPERTY_REMOVED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have retracted property " << (int)details->GetField2() << " from the deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_PROPERTY_ADDED: {
        PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have put property " << (int)details->GetField2() << " on the table." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED: {
        PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have retracted property " << (int)details->GetField2() << " from the deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_CARD_ADDED: {
        PIMP_TRANSACTION_CARD_ADDED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have put card " << (int)details->GetField2() << " on the table." << endl;
        end;
      }
      case PIMP_TRANSACTION_CARD_REMOVED: {
        PIMP_TRANSACTION_CARD_REMOVED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have retracted card " << (int)details->GetField2() << " from the deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_CARD_ADDED: {
        PIMP_TRANSACTION_OTHER_CARD_ADDED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have put card " << (int)details->GetField2() << " on the table." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_CARD_REMOVED: {
        PIMP_TRANSACTION_OTHER_CARD_REMOVED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have retracted card " << (int)details->GetField2() << " from the deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_FINISHED: {
        PIMP_TRANSACTION_FINISHED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have said we are ready." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_FINISHED: {
        PIMP_TRANSACTION_OTHER_FINISHED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have said they are ready." << endl;
        end;
      }
      case PIMP_TRANSACTION_REOPENED: {
        PIMP_TRANSACTION_REOPENED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We are no longer ready." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_REOPENED: {
        PIMP_TRANSACTION_OTHER_REOPENED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They are no longer ready." << endl;
        end;
      }
      case PIMP_TRANSACTION_AGREED: {
        PIMP_TRANSACTION_AGREED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "We have agreed to this deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_OTHER_AGREED: {
        PIMP_TRANSACTION_OTHER_AGREED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "They have agreed to this deal." << endl;
        end;
      }
      case PIMP_TRANSACTION_FINALISED: {
        PIMP_TRANSACTION_FINALISED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "The deal has been finalised." << endl;
        end;
      }
      case PIMP_TRANSACTION_CANCELLED: {
        PIMP_TRANSACTION_CANCELLED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "The deal has been cancelled." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE: {
        PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "You don't have $" << details->GetField2() << "." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED: {
        PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "You do not own property " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED: {
        PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "You do not own card " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_NOT_SUITABLE: {
        PIMP_ERROR_TRANSACTION_NOT_SUITABLE_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "This deal is not acceptable." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE: {
        PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "Property " << (int)details->GetField2() << " still has buildings on it." << endl;
        end;
      }
      case PIMP_ERROR_TRANSACTION_NOT_BANKRUPT: {
        PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_CAST(message, details);
        if (details->GetField2()) {
          cout << "Trade " << details->GetField1() << ": "
               << "You must complete trade " << details->GetField2() << " before claiming bankruptcy on this deal." << endl;
        } else {
          cout << "Trade " << details->GetField1() << ": "
               << "You can afford to pay the requested amount." << endl;
        }
        end;
      }
      case PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED: {
        PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "This trade cannot be cancelled." << endl;
        end;
      }
      case PIMP_TRANSACTION_UNUSUAL: {
        PIMP_TRANSACTION_UNUSUAL_CAST(message, details);
        cout << "Trade " << details->GetField1() << ": "
             << "This trade is unusual." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_NOT_OWNED: {
        PIMP_ERROR_HOUSES_NOT_OWNED_CAST(message, details);
        cout << "To build on property " << (int)details->GetField1() << " you need to own it." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_NEED_MONOPOLY: {
        PIMP_ERROR_HOUSES_NEED_MONOPOLY_CAST(message, details);
        cout << "To build on property " << (int)details->GetField1() << " you need the complete monopoly." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_MORTGAGED: {
        PIMP_ERROR_HOUSES_MORTGAGED_CAST(message, details);
        cout << "To build on property " << (int)details->GetField1() << " you need to unmortgage it first." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP: {
        PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP_CAST(message, details);
        cout << "You cannot build on property " << (int)details->GetField1() << ", it's not a colour group." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_NOT_YOUR_TURN: {
        PIMP_ERROR_HOUSES_NOT_YOUR_TURN_CAST(message, details);
        cout << "You may only build houses on your own turn." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_TOO_EXPENSIVE: {
        PIMP_ERROR_HOUSES_TOO_EXPENSIVE_CAST(message, details);
        cout << "You need $" << (int)details->GetField1() << " to build those houses." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT: {
        PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT_CAST(message, details);
        cout << "There aren't enough house pieces left." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER: {
        PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER_CAST(message, details);
        cout << "You cannot build that number of houses on property " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_ERROR_HOUSES_UNBALANCED: {
        PIMP_ERROR_HOUSES_UNBALANCED_CAST(message, details);
        cout << "You asked for an uneven number of houses on property " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE: {
        PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_CAST(message, details);
        cout << "You need $" << details->GetField2()
             << " to unmortgage property " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_SQUARE_GIVES_CASH: {
        PIMP_SQUARE_GIVES_CASH_CAST(message, details);
        cout << "Square " << (int)details->GetField2()
             << " just gave $" << (int)details->GetField3()
             << " to player " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_SQUARE_TAKES_CASH: {
        PIMP_SQUARE_TAKES_CASH_CAST(message, details);
        cout << "Square " << (int)details->GetField2()
             << " just took $" << (int)details->GetField3()
             << " from player " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_SQUARE_GIVES_CARD: {
        PIMP_SQUARE_GIVES_CARD_CAST(message, details);
        cout << "Square " << (int)details->GetField2()
             << " just gave card " << (int)details->GetField3()
             << " to player " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_SQUARE_TAKES_CARD: {
        PIMP_SQUARE_TAKES_CARD_CAST(message, details);
        cout << "Square " << (int)details->GetField2()
             << " just took card " << (int)details->GetField3()
             << " from player " << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_DELTA_CASH: {
        PIMP_DELTA_CASH_CAST(message, details);
        cout << "$" << (int)details->GetField3()
             << " just went from player " << (int)details->GetField1()
             << " to player " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_DELTA_PROPERTY: {
        PIMP_DELTA_PROPERTY_CAST(message, details);
        cout << "Property " << (int)details->GetField3()
             << " just went from player " << (int)details->GetField1()
             << " to player " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_DELTA_CARD: {
        PIMP_DELTA_CARD_CAST(message, details);
        cout << "Card " << (int)details->GetField3()
             << " just went from player " << (int)details->GetField1()
             << " to player " << (int)details->GetField2() << "." << endl;
        end;
      }
      case PIMP_DELTA_HOUSES_PURCHASED: {
        PIMP_DELTA_HOUSES_PURCHASED_CAST(message, details);
        cout << "Player " << (int)details->GetField1()
             << " just changed the houses on property " << (int)details->GetField2()
             << " to be " << (int)details->GetField3()
             << " houses and " << (int)details->GetField4() << " hotels." << endl;
        end;
      }
      case PIMP_DELTA_POT: {
        PIMP_DELTA_POT_CAST(message, details);
        cout << "The pot now contains $" << (int)details->GetField1() << "." << endl;
        end;
      }
      case PIMP_DELTA_PROPERTY_MORTGAGED: {
        PIMP_DELTA_POT_CAST(message, details);
        cout << "Property " << (int)details->GetField1() << " is now mortgaged." << endl;
        end;
      }
      case PIMP_DELTA_PROPERTY_UNMORTGAGED: {
        PIMP_DELTA_POT_CAST(message, details);
        cout << "Property " << (int)details->GetField1() << " is now mortgaged." << endl;
        end;
      }
      case PIMP_RENT_COLLECTION_MORATORIUM: {
        PIMP_RENT_COLLECTION_MORATORIUM_CAST(message, details);
        cout << "Someone missed their chance to claim rent or go money." << endl;
        end;
      }
      case PIMP_WAITING_FOR_TRANSACTION: {
        PIMP_WAITING_FOR_TRANSACTION_CAST(message, details);
        cout << "We are waiting for player " << (int)details->GetField1()
             << " to conclude trade " << (int)details->GetField2() << "." << endl;
        end;
      }
      }
      unexpectedPacket(message);
    always: delete message; // 'end' jumps to this line
    } else {
      char c;
      cin.get(c);
      cout << endl;
      switch (c) {
      case 'Q':
        if (gameNumber) {
          cout << "You can rejoin this game by using the following details:" << endl;
          cout << "  Game number: " << gameNumber << endl;
          cout << "Player number: " << player << endl;
          cout << "     Password: " << password << endl;
        }
        loop = false;
        break;
      case 'K':
        cout << "Requesting current player be kicked." << endl;
        network->SendAndDeleteMessage(PIMP_KICK_FACTORY(currentPlayer));
        break;
      case 'T':
        if (playing) {
          cout << "The bank is player 0." << endl;
          int target = promptInt("Transfer to which player?");
          cout << "Requesting transfer to player " << target << "." << endl;
          network->SendAndDeleteMessage(PIMP_TRANSFER_FACTORY(target));
        } else {
          cout << "Requesting transfer to player status." << endl;
          network->SendAndDeleteMessage(PIMP_SWITCH_PLAY_FACTORY());
        }
        break;
      case 's':
        cout << "Requesting status update." << endl;
        network->SendAndDeleteMessage(PIMP_REQUEST_STATE_FACTORY());
        break;
      case 'c': {
        if (playing) {
          cout << "To claim go money, claim rent from player 0 (the bank)." << endl;
          int target = promptInt("Claim rent from which player?");
          if (target == 0) {
            cout << "Requesting go money from bank." << endl;
            network->SendAndDeleteMessage(PIMP_CLAIM_GO_FACTORY(0));
          } else {
            int property = promptInt("Claim rent for landing on which property?");
            cout << "Requesting rent from player " << target
                 << " for landing on property " << property << "." << endl;
            network->SendAndDeleteMessage(PIMP_CLAIM_RENT_FACTORY(target, property));
          }
        }
        break;
      }
      case 't':
        if (playing)
          trade();
        break;
      case 'h':
        if (playing)
          houses();
        break;
      case 'm':
        if (playing)
          mortgage();
        break;
      case 'a':
        // accept
        if (candidate) {
          network->SendAndDeleteMessage(PIMP_ACCEPT_JOIN_FACTORY(candidate));
          break;
        } // else, fallthrough:
      case 'r':
        // refuse
        if (candidate) {
          network->SendAndDeleteMessage(PIMP_REFUSE_JOIN_FACTORY(candidate));
          break;
        } // else, fallthrough:
      default:
        switch (status) {
        case status_throwDice:
          switch (c) {
          case 'd':
            network->SendAndDeleteMessage(PIMP_THROW_DICE_FACTORY());
            status = status_waiting;
            break;
          default:
            reprompt();
            break;
          }
          break;
        case status_sale:
          switch (c) {
          case 'b':
            network->SendAndDeleteMessage(PIMP_BUY_PROPERTY_FACTORY());
            status = status_waiting;
            break;
          case 'a':
            network->SendAndDeleteMessage(PIMP_AUCTION_PROPERTY_FACTORY());
            status = status_waiting;
            break;
          default:
            reprompt();
            break;
          }
          break;
        case status_auction:
          switch (c) {
          case 'b':
            network->SendAndDeleteMessage(PIMP_BID_FACTORY(bid + 10));
            break;
          case 'n':
            network->SendAndDeleteMessage(PIMP_NO_BID_FACTORY());
            break;
          default:
            reprompt();
            break;
          }
          break;
        case status_jail:
          switch (c) {
          case 'b':
            network->SendAndDeleteMessage(PIMP_JAIL_PAY_BAIL_FACTORY());
            status = status_waiting;
            break;
          case 'd':
            network->SendAndDeleteMessage(PIMP_JAIL_ROLL_DICE_FACTORY());
            status = status_waiting;
            break;
          default:
            reprompt();
            break;
          }
          break;
        case status_tax:
          switch (c) {
          case '$':
            network->SendAndDeleteMessage(PIMP_TAX_PAY_FLAT_FEE_FACTORY());
            status = status_waiting;
            break;
          case '%':
            network->SendAndDeleteMessage(PIMP_TAX_PAY_TEN_PERCENT_FACTORY());
            status = status_waiting;
            break;
          default:
            reprompt();
            break;
          }
          break;
        default:
          // key 'h' is reserved to bring this up too.
          cout << "Q: quit; K: kick; T: transfer; s: status";
          if (playing)
            cout << "; c: claim rent; t: trade; h: houses; m: mortgage";
          cout << endl;
          break;
        }
        break;
      }
    }
  } while (loop);
  delete network;

  // Restore the original TTY settings.
  if (tcsetattr(0, TCSAFLUSH, &oldtty) == -1) {
    cerr << "Unable to restore the TTY attributes." << endl;
    throw IOConfigurationError();
  }

  return 0;
}
