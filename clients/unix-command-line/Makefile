CXXFLAGS = -g -fpermissive -std=c++03 -w

CXXSRCS = \
	network.cpp \
	../../server/message.cpp \
	$(NULL)

CLIENT_SRCS = $(CXXSRCS) main.cpp

all: client

client: $(patsubst %.cpp, %.o, $(CLIENT_SRCS))
	c++ -o monopoly $^

../../server/message.cpp: ../../server/message-factories.cpp.inc ../../server/message.h

../../server/message.h: ../../server/message-factories.h.inc

../../server/message-factories.cpp.inc: ../../server/message-factories.h.inc

../../server/message-factories.h.inc: ../../server/message-factories.pl ../../pimp-design.txt
	cd ../../server; perl message-factories.pl
