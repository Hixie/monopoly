#!/usr/bin/perl -w
use strict;

open(SPEC, '../../pimp-design.txt');

print "using System;  // -*- Mode: Java -*-\n";

my %types = (
             'byte' => 'byte',
             '4 byte unsigned integer' => 'uint',
             'signed byte' => 'sbyte',
             '4 byte signed integer' => 'int',
             'string' => 'string',
             'bool' => 'bool',
             );

my @messages;

while (<SPEC>) {
      if (m/^(PIMP_[^ ]+) +0x([0-9A-F]{2})(?: +(.+))?\n$/os) {
          my $message = $1;
          push(@messages, $message);
          next if $message eq 'PIMP_PURCHASE_HOUSES'; # done as a special case
          my $code = $2;
          my $types = $3;
          $types = '' unless (defined($types));
          my @types = split(/, /os, $types);
          print "
          class $message : Message {
              public const byte MessageType = 0x$code;
              override public byte GetMessageType() { return MessageType; }";
          my $count = 0;
          foreach my $type (@types) {
              $count++;
              $type =~ s/ \(.+\)$//gos;
              die "Type $type doesn't exist\n" unless exists($types{$type});
              print "\n              public $types{$type} Field$count;";
          }
          print "\n              public $message() : base() { }\n";
          if ($count) {
          print "              public $message(";
              my $i = 0;
              foreach my $type (@types) {
                  if ($i++) { print ', '; }
                  print "$types{$type} field$i";
              }
              print ") : base() {";
              for (my $i = 1; $i <= $count; ++$i) {
                  print "\n                  Field$i = field$i;";
              }
              print "
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {";
              for (my $i = 1; $i <= $count; ++$i) {
                  print "\n                  Write(Field$i, ref payload, ref index);";
              }
              print "
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;";
              for (my $i = 1; $i <= $count; ++$i) {
                  print "\n                  Read(out Field$i, ref payload, ref index);";
              }
              print "\n              }";
          }
          print "
          }\n";
      }
}

print "
class MessageMap {
    public static Message CreateMessage(byte type) {
        switch (type) {
";
foreach (@messages) {
    print "\n            case $_.MessageType: return new $_();";
}
print "
          default: throw new UnexpectedMessageException();
        }
    }
}
";

close(SPEC);
