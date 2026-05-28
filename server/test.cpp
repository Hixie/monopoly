#include "iostream.h"
#include "player.h"
#include "board.h"
#include "message.h"
#include <stdlib.h>
#include <time.h>

int main() {
  cout << "Kaspar Test App version 0.9" << endl;
  Board* board = new Board();
  cout << "Adding player" << endl;
  Message* message = PIMP_WELCOME_PLAYER_FACTORY(1, 0x00, "Blake");
  board->Dispatch(message);
  delete message;
  int dice1, dice2;
  int count = 0;
  while(1) {
    cout << "Round " << ++count << endl;
    /*
    blake->Roll(board, dice1, dice2);
    cout << "You rolled " << dice1 << " and " << dice2 << endl;
    cout << "You're now at spot " << blake->Move(board, dice1+dice2) << endl;
    */
    char n;
    cout << "roll again? y/n ";
    cin >> n;
    if (n != 'y') break;
  }
  delete board;
  return 0;
}
