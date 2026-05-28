use strict;

local $" = '';

open(SPEC, '../pimp-design.txt') or die "can't open spec: $!";
my %messages = ();
my %types = ();
while (<SPEC>) {
    if (m/^(PIMP_[^ ]+) +0x(..)(?: (.+?))?\n?$/gos) {
        my $name = $1;
        my $code = $2;
        my $type = $3;
        my $subtypes;
        if ($type eq 'byte = n, n times (byte, signed byte, signed byte)') {
            $type = 'HouseChangeList';
        } else {
            my @types = split(', ', $type);
            foreach (@types) {
                /^bool$/ and $_ = 'Bool' and next;
                /^byte(?: \(.+\))?$/ and $_ = 'Byte' and next;
                /^string(?: \(.+\))?$/ and $_ = 'String' and next;
                /^signed byte$/ and $_ = 'SignedByte' and next;
                /^4 byte unsigned integer$/ and $_ = 'UnsignedInt' and next;
                /^4 byte signed integer$/ and $_ = 'Int' and next;
                die "unknown type in spec: '$_'\n";
            }
            $type = "@types";
            my @subtypes;
            foreach (@types) {
                push(@subtypes, $_);
                $subtypes = "@subtypes";
                $types{$subtypes} = [@subtypes];
            }
        }
        $messages{$code} = [$name, $type];
    }
}
close(SPEC);

my $map = '';
my $classes = '';
my $definitions = '';

#########################################################################################

my $fieldArray = {
    'Bool' => 'bool',
    'Byte' => 'uint8',
    'String' => 'char*',
    'SignedByte' => 'int8',
    'UnsignedInt' => 'unsigned int',
    'Int' => 'int',
};

my %typeArgs = ();

foreach my $code (sort { @{$types{$a}} <=> @{$types{$b}} # sort by length
                         || $a cmp $b } keys %types) {   # then by compound type name

    my @typeNames = @{$types{$code}};
    my $fieldNumber = @typeNames;

    my @types = @typeNames;
    my @arguments;
    my @argumentNames;
    my $number = 0;
    foreach (@types) {
        ++$number;
        push(@arguments, ", $fieldArray->{$_} field$number");
        push(@argumentNames, ", field$number");
    }
    my @superclassArguments = @argumentNames;
    pop @superclassArguments;

    my @superclassNames = @typeNames;
    my $fieldName = pop @superclassNames;
    my $fieldType = $fieldArray->{$fieldName};

    $typeArgs{$code} = "@argumentNames";


############################################## CLASSES ##################################

    $classes .= "
class Message@typeNames : public Message@superclassNames {
 public:
  Message@typeNames(int player, uint8 type@arguments);
  Message@typeNames(int player);
  Message@typeNames(Message@typeNames* message);";
    if ($fieldName eq 'String') {
        $classes .= "\n  Message@{typeNames}::~Message@typeNames();";
    }
    $classes .= "
  Message* Copy() { return new Message@typeNames(this); }
  $fieldType GetField$fieldNumber() { ";
    if ($fieldName eq 'String') {
        $classes .= "return strdup(mField$fieldNumber);";
    } else {
        $classes .= "return mField$fieldNumber;";
    }
    $classes .= " }
  void SetField$fieldNumber($fieldType field$fieldNumber) { ";
    if ($fieldName eq 'String') {
        $classes .= "free(mField$fieldNumber); mField$fieldNumber = strdup(field$fieldNumber);";
    } else {
        $classes .= "mField$fieldNumber = field$fieldNumber;";
    }
    $classes .= " }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  $fieldType mField$fieldNumber;
};
Message* Message@typeNames\Factory(int player, uint8 type@arguments);
Message* Message@typeNames\Decoder(int player, MessageBuffer buffer, int length);
";

##################################### DEFINITIONS #######################################

    ################### CONSTRUCTORS ##################
    $definitions .= "
Message@{typeNames}::Message@typeNames(int player, uint8 type@arguments) :
  Message@superclassNames(player, type@superclassArguments),
  ";

    if ($fieldName eq 'String') {
        $definitions .= "mField$fieldNumber(strdup(field$fieldNumber))";
    } else {
        $definitions .= "mField$fieldNumber(field$fieldNumber)";
    }

    $definitions .= "
{ }

Message@{typeNames}::Message@typeNames(int player) :
  Message@superclassNames(player),";

    if ($fieldName eq 'String') {
        $definitions .= "\n  mField$fieldNumber(NULL)";
    } else {
        $definitions .= "\n  mField$fieldNumber(0)";
    }

    $definitions .= "
{ }

Message@{typeNames}::Message@typeNames(Message@typeNames* message) :
  Message@superclassNames(message),
  mField$fieldNumber(message->GetField$fieldNumber())
{ }\n";

    ################### DESTRUCTOR ####################
    if ($fieldName eq 'String') {
        $definitions .= "

Message@{typeNames}::~Message@typeNames() {
  free(mField$fieldNumber);
}
";
    }


    ################### PARSER ########################
    $definitions .= "
int Message@{typeNames}::ParsePayload(MessageBuffer buffer, int length) {
  int index = Message@superclassNames\::ParsePayload(buffer, length);";

    if ($fieldName eq 'Byte') {
      $definitions .= "
  if (length - index >= 1) {
    mField$fieldNumber = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
    return index + 1;
";      
    } elsif ($fieldName eq 'Bool' or $fieldName eq 'SignedByte') {
      $definitions .= "
  if (length - index >= 1) {
    Convertor data;
    data.Byte = buffer[index];
    mField$fieldNumber = data.$fieldName;
    if (DEBUG_MESSAGES)
      cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
    return index + 1;
";
    } elsif ($fieldName eq 'Int' or $fieldName eq 'UnsignedInt') {
      $definitions .= "
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField$fieldNumber = ntohl(data.$fieldName);
    if (DEBUG_MESSAGES)
      cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
    return index + 4;
";
    } else {
      $definitions .= "
  mField$fieldNumber = (char*)(malloc(MAX_STRING_LENGTH+1));
  if (length - index >= 1) {
    // first byte is the string length
    int size = buffer[index++];
    if (index + size > length) 
      throw ParseError();
    if (size > MAX_STRING_LENGTH) 
      throw ParseError();
    for (int i = 0; i < size; ++i) {
      mField$fieldNumber\[i] = buffer[i+index];
    }
    mField$fieldNumber\[size] = '\\0';
    if (DEBUG_MESSAGES)
      cerr << \"(\" << mField$fieldNumber << \")\";
    return index+size;
";
    }

    ################### SERIALISER ####################
    $definitions .= "  } else {
    throw ParseError();
  }
}

uint8 Message@{typeNames}::SerializePayload(MessageBuffer buffer) {
  uint8 length = Message@superclassNames\::SerializePayload(buffer);";

    if ($fieldName eq 'Byte') {
      $definitions .= "
  if (DEBUG_MESSAGES)
    cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
  buffer[length] = mField$fieldNumber;
  return length + 1;
";
    } elsif ($fieldName eq 'Bool' or $fieldName eq 'SignedByte') {
      $definitions .= "
  if (DEBUG_MESSAGES)
    cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
  Convertor data;
  data.$fieldName = mField$fieldNumber;
  buffer[length] = data.Byte;
  return length + 1;
";
    } elsif ($fieldName eq 'Int' or $fieldName eq 'UnsignedInt') {
      $definitions .= "
  if (DEBUG_MESSAGES)
    cerr << \"(\" << dec << (int)mField$fieldNumber << \")\";
  Convertor data;
  data.$fieldName = htonl(mField$fieldNumber);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
";
    } else {
      $definitions .= "
  if (DEBUG_MESSAGES)
    cerr << \"(\" << mField$fieldNumber << \")\";
  int size = strlen(mField$fieldNumber);
  buffer[length] = size;
  for (int i = 0; i < size; ++i) {
    buffer[length+i+1] = mField$fieldNumber\[i];
  }
  return length+size+1;
";
    }

    ################### FACTORIES #####################
    $definitions .= "}

Message* Message@typeNames\Factory(int player, uint8 type@arguments) {
  return new Message@typeNames(player, type@argumentNames);
};

Message* Message@typeNames\Decoder(int player, MessageBuffer buffer, int length) {
  Message* message = new Message@typeNames(player);
  message->Parse(buffer, length);
  return message;
};\n\n";

}

################################################ MAP ####################################


my $count = scalar(keys %messages);

foreach my $code (sort { $messages{$a}->[0] cmp $messages{$b}->[0] } keys %messages) {
    my $name = $messages{$code}->[0];
    my $type = $messages{$code}->[1];
    $map .= "#define $name 0x$code\n";

    if ($type eq 'HouseChangeList') {
        $map .= "#define ${name}_FACTORY(count, properties, amountHouses, amountHotels) MessageHouseChangeListFactory(0, 0x$code, count, properties, amountHouses, amountHotels)\n";
    } else {
        my $argsWithComma = $typeArgs{$type};
        my $argsWithoutComma = $argsWithComma;
        $argsWithoutComma =~ s/^, //os;
        $map .= "#define ${name}_FACTORY($argsWithoutComma) Message${type}Factory(0, 0x$code$argsWithComma)\n";
    }
    $map .= "#define ${name}_CAST(message, cast) Message$type* cast = static_cast<Message$type*> (message)\n";
}

    $definitions .= "
#define NUM_MESSAGE_FACTORIES $count
struct MessageFactoryMap {
  uint8 type;
  MessageFactoryPointer factory;
  char* name;
} messageFactories[NUM_MESSAGE_FACTORIES] = {
  // This absolutely positively must be in ascending numerical order (it is binary tree searched)
";

$count = 0;
foreach my $code (sort keys %messages) {
    $definitions .= ",\n" if ($count++);
    my $name = $messages{$code}->[0];
    my $type = $messages{$code}->[1];
    $definitions .= "  { $name, &Message${type}Decoder, \"$name\" }";
}

$definitions .= "\n};\n";

#########################################################################################

open(OUT1, '>message-factories.h.inc');

print OUT1 " // THIS FILE IS AUTO-GENERATED: DO NOT EDIT

#include <stdlib.h>
#include <string.h>
$classes

// C function pointer syntax is retarded
// define a typedef to make it less cumbersome
typedef Message* (*MessageFactoryPointer) (int, MessageBuffer, int);

$map";

close(OUT1);

open(OUT2, '>message-factories.cpp.inc');

print OUT2 " // THIS FILE IS AUTO-GENERATED: DO NOT EDIT
$definitions";

close(OUT2);
