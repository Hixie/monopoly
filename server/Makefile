CXXFLAGS = -g

CXXSRCS = \
	board.cpp \
	card.cpp \
	message.cpp \
	player.cpp \
	property.cpp \
	square.cpp \
	transaction.cpp \
	claim.cpp \
	$(NULL)

MONOPOLY_SRCS = $(CXXSRCS) network.cpp main.cpp
TEST_SRCS = $(CXXSRCS) test.cpp

all: monopoly # test

monopoly: $(patsubst %.cpp, %.o, $(MONOPOLY_SRCS))
	c++ -o monopoly $^

test: $(patsubst %.cpp, %.o, $(TEST_SRCS))
	c++ -o test $^

board.cpp: message.h

message.cpp: message-factories.cpp.inc

message.h: message-factories.h.inc

message-factories.cpp.inc: message-factories.h.inc

message-factories.h.inc: message-factories.pl ../pimp-design.txt
	/usr/bin/perl message-factories.pl
