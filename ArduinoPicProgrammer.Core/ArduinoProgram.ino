#define CMD_PREFORM_PROGRAM_MEMORY 0x1
#define CMD_INCREMENT_ADDRESS 0x2
#define CMD_INCREMENT_ADDRESS_COUNT 0x3
#define CMD_LOAD_CONFIGURATION 0x4
#define CMD_BULK_ERASE_PROGRAM_MEMORY 0x5
#define CMD_READ_DATA_FROM_PROGRAM_MEMORY 0x6
#define CMD_RESET 0x7

#define OPERATION_FINISHED 0x8

#define END_PROGRAMMING 0B1110
#define BEGIN_PROGRAMMING 0B11000
#define INCREMENT_ADDRESS 0B110
#define LOAD_CONFIGURATION 0B0 
#define BULK_ERASE_PROGRAM_MEMORY 0B1001
#define LOAD_DATA_FOR_PROGRAM_MEMORY 0B10
#define READ_DATA_FROM_PROGRAM_MEMORY 0B100


typedef unsigned int Word; //uint_16
typedef unsigned int Address; //uint_16

#define T_SET1     1       // Data in setup time before lowering clock
#define T_HLD1     1       // Data in hold time after lowering clock
#define T_DLY2     2       // Delay between commands or data
#define T_DLY3     1       // Delay until data bit read will be valid
#define T_PROG     1000    // Time for a program memory write to complete
#define T_ERA      5000    // Time for a word erase to complete
#define T_DIS      120     // Time delay to next command

#define  PIN_CLOCK  9
#define  PIN_DATA   8
#define  PIN_VPP1   6
#define  PIN_VPP2   5
#define  PIN_VPP3   4
#define  PIN_PGM    10

void setup() {
  pinMode(PIN_CLOCK, OUTPUT);
  pinMode(PIN_VPP1, OUTPUT);
  pinMode(PIN_VPP2, OUTPUT);
  pinMode(PIN_VPP3, OUTPUT);
  pinMode(PIN_DATA, OUTPUT);
  pinMode(PIN_PGM, OUTPUT);
  Serial.begin(9600);
  //start();
}
void loop() {
  if (Serial.available() > 0) {
    byte command = Serial.read();
    execute(command);
  }
}

void execute(byte command) {
  Word data;
  switch (command)
  {
  case CMD_PREFORM_PROGRAM_MEMORY:
    data = readWord();
    preformProgramMemory(data);
    break;
  case CMD_INCREMENT_ADDRESS:
    sendMicrocontrollerCommandAndWait(INCREMENT_ADDRESS);
    break;
  case CMD_INCREMENT_ADDRESS_COUNT:
    data = readWord();
    incrementAddress(data);
    break;
  case CMD_LOAD_CONFIGURATION:
    sendMicrocontrollerCommandAndWait(LOAD_CONFIGURATION);
    break;
  case CMD_BULK_ERASE_PROGRAM_MEMORY:
    sendMicrocontrollerCommandAndWait(BULK_ERASE_PROGRAM_MEMORY, T_ERA);
    break;
  case CMD_READ_DATA_FROM_PROGRAM_MEMORY:
    data = readDataFromProgramMemory();
    sendWord(data);
    goto skipDone;
  case CMD_RESET:
    reset();
    break;
  default:
    goto skipDone;
  }
  Serial.write(OPERATION_FINISHED);
skipDone:;
}

void sendMicrocontrollerCommandAndWait(byte command) {
  sendMicrocontrollerCommandAndWait(command, T_DLY2);
}
void sendMicrocontrollerCommandAndWait(byte command, int d) {
  sendMicrocontrollerCommand(command);
  delayMicroseconds(d);
}

void incrementAddress(Word count) {
  for (unsigned int i = 0; i < count; i++) {
    sendMicrocontrollerCommandAndWait(INCREMENT_ADDRESS);
  }
}

void sendMicrocontrollerCommand(byte command) {
  sendMicrocontrollerWrite(command, 6);
}

void sendMicrocontrollerWrite(Word data, byte bitsToWrite) {

  for (byte bit = 0; bit < bitsToWrite; ++bit) {
    digitalWrite(PIN_CLOCK, HIGH);
    delayMicroseconds(T_SET1);

    if (data & 1)
      digitalWrite(PIN_DATA, HIGH);
    else
      digitalWrite(PIN_DATA, LOW);

    delayMicroseconds(T_HLD1);

    digitalWrite(PIN_CLOCK, LOW);
    data >>= 1;
  }
}

void preformProgramMemory(Word data) {
  sendMicrocontrollerCommandAndWait(LOAD_DATA_FOR_PROGRAM_MEMORY);
  sendMicrocontrollerWrite(data << 1, 16);
  sendMicrocontrollerCommandAndWait(BEGIN_PROGRAMMING, T_PROG);
  sendMicrocontrollerCommandAndWait(END_PROGRAMMING, T_DIS);
}

Word readDataFromProgramMemory() {
  sendMicrocontrollerCommand(READ_DATA_FROM_PROGRAM_MEMORY);

  Word data = 0;
  digitalWrite(PIN_DATA, LOW);
  pinMode(PIN_DATA, INPUT);
  delayMicroseconds(T_DLY2);

  for (byte bit = 0; bit < 16; ++bit) {
    data >>= 1;
    digitalWrite(PIN_CLOCK, HIGH);
    delayMicroseconds(T_DLY3);
    if (digitalRead(PIN_DATA))
      data |= 0x8000;
    digitalWrite(PIN_CLOCK, LOW);
    delayMicroseconds(T_HLD1);
  }
  pinMode(PIN_DATA, OUTPUT);
  delayMicroseconds(T_DLY2);

  return (data  >> 1) & 0x3FFF;
}
void reset() {
  digitalWrite(PIN_VPP1, LOW);
  digitalWrite(PIN_VPP2, LOW);
  digitalWrite(PIN_VPP3, LOW);
  delay(30);

  start();
}

void start() {
  digitalWrite(PIN_VPP1, HIGH);
  digitalWrite(PIN_VPP2, HIGH);
  digitalWrite(PIN_VPP3, HIGH);
}

Word readWord() {
  while (Serial.available() == 0);
  byte high = Serial.read();
  while (Serial.available() == 0);
  byte low = Serial.read();
  return (high << 8) | low;
}

void sendWord(Word data) {
  byte high = data >> 8;
  byte low = data & 0xFF;
  Serial.write(high);
  //delay(1);
  Serial.write(low);
}
