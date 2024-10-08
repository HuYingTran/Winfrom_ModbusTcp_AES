#include <SPI.h>
#include <Ethernet.h>

// Địa chỉ MAC và IP của server
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192, 168, 1, 200);
EthernetServer server(502);

void setup() {
  Ethernet.begin(mac, ip);
  server.begin();
  Serial.begin(9600);
  Serial.println("Arduino start");
  Serial2.begin(9600);
}

/*AES-128 Cipher Key, plain_text, Output Array*/
unsigned char  key[16] = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };  /*Default Key*/
unsigned char  plain_text[16] = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
unsigned char  cipher_text[16] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
unsigned char  Output[16];
/****************************************/

/*AES-128 Define and Constant*/
// The number of columns comprising a state in AES. This is a constant in AES. Value=4
#define Nb 4
// The number of 32 bit words in a key.
#define Nk 4
// Key length in bytes [128 bit]
#define KEYLEN 16
// The number of rounds in AES Cipher.
#define Nr 10
// The array that stores the round keys.
unsigned char RoundKey[176];

// The Key input to the AES Program
unsigned char* Key;

// The lookup-tables are marked const so they can be placed in read-only storage instead of RAM
// The numbers below can be computed dynamically trading ROM for RAM -
// This can be useful in (embedded) bootloader applications, where ROM is often limited.
const unsigned char sbox[256] = {
  //0     1    2      3     4    5     6     7      8    9     A      B    C     D     E     F
  0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
  0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
  0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
  0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
  0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
  0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
  0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
  0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
  0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
  0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
  0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
  0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
  0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
  0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
  0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
  0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 
};

const unsigned char rsbox[256] ={ 
0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d 
};


// The round constant word array, Rcon[i], contains the values given by
// x to th e power (i-1) being powers of x (x is denoted as {02}) in the field GF(2^8)
// Note that i starts at 1, not 0).
const unsigned char Rcon[255] = {
  0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
  0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39,
  0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a,
  0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8,
  0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef,
  0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc,
  0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b,
  0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3,
  0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94,
  0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20,
  0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35,
  0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f,
  0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04,
  0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63,
  0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd,
  0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb 
};
typedef unsigned char state_t[4][4];
state_t* state;

unsigned char getSBoxValue(unsigned char num)
{
  return sbox[num];
}

unsigned char getSBoxInvert(unsigned char num)
{
  return rsbox[num];
}
void KeyExpansion(void)
{
  int i, j, k;
  unsigned char tempa[4]; // Used for the column/row operations

  // The first round key is the key itself.
  for (i = 0; i < Nk; ++i)
  {
    RoundKey[(i * 4) + 0] = Key[(i * 4) + 0];
    RoundKey[(i * 4) + 1] = Key[(i * 4) + 1];
    RoundKey[(i * 4) + 2] = Key[(i * 4) + 2];
    RoundKey[(i * 4) + 3] = Key[(i * 4) + 3];
  }

  // All other round keys are found from the previous round keys.
  for (; (i < (Nb * (Nr + 1))); ++i)
  {
    for (j = 0; j < 4; ++j)
    {
      tempa[j] = RoundKey[(i - 1) * 4 + j];
    }
    if (i % Nk == 0)
    {
      // This function rotates the 4 bytes in a word to the left once.
      // [a0,a1,a2,a3] becomes [a1,a2,a3,a0]

      // Function RotWord()
      {
        k = tempa[0];
        tempa[0] = tempa[1];
        tempa[1] = tempa[2];
        tempa[2] = tempa[3];
        tempa[3] = k;
      }

      // SubWord() is a function that takes a four-byte input word and
      // applies the S-box to each of the four bytes to produce an output word.

      // Function Subword()
    {
      tempa[0] = getSBoxValue(tempa[0]);
      tempa[1] = getSBoxValue(tempa[1]);
      tempa[2] = getSBoxValue(tempa[2]);
      tempa[3] = getSBoxValue(tempa[3]);
    }

      tempa[0] = tempa[0] ^ Rcon[i / Nk];
    }
    else if (Nk > 6 && i % Nk == 4)
    {
      // Function Subword()
      {
        tempa[0] = getSBoxValue(tempa[0]);
        tempa[1] = getSBoxValue(tempa[1]);
        tempa[2] = getSBoxValue(tempa[2]);
        tempa[3] = getSBoxValue(tempa[3]);
      }
    }
    RoundKey[i * 4 + 0] = RoundKey[(i - Nk) * 4 + 0] ^ tempa[0];
    RoundKey[i * 4 + 1] = RoundKey[(i - Nk) * 4 + 1] ^ tempa[1];
    RoundKey[i * 4 + 2] = RoundKey[(i - Nk) * 4 + 2] ^ tempa[2];
    RoundKey[i * 4 + 3] = RoundKey[(i - Nk) * 4 + 3] ^ tempa[3];
  }
}

// This function adds the round key to state.
// The round key is added to the state by an XOR function.
void AddRoundKey(unsigned char round)
{
  unsigned char i, j;
  for (i = 0; i<4; ++i)
  {
    for (j = 0; j < 4; ++j)
    {
      (*state)[i][j] ^= RoundKey[round * Nb * 4 + i * Nb + j];
    }
  }
}

// The SubBytes Function Substitutes the values in the
// state matrix with values in an S-box.
void SubBytes(void)
{
  unsigned char i, j;
  for (i = 0; i < 4; ++i)
  {
    for (j = 0; j < 4; ++j)
    {
      (*state)[j][i] = getSBoxValue((*state)[j][i]);
    }
  }
}

// The ShiftRows() function shifts the rows in the state to the left.
// Each row is shifted with different offset.
// Offset = Row number. So the first row is not shifted.
void ShiftRows(void)
{
  unsigned char temp;

  // Rotate first row 1 columns to left
  temp = (*state)[0][1];
  (*state)[0][1] = (*state)[1][1];
  (*state)[1][1] = (*state)[2][1];
  (*state)[2][1] = (*state)[3][1];
  (*state)[3][1] = temp;

  // Rotate second row 2 columns to left
  temp = (*state)[0][2];
  (*state)[0][2] = (*state)[2][2];
  (*state)[2][2] = temp;

  temp = (*state)[1][2];
  (*state)[1][2] = (*state)[3][2];
  (*state)[3][2] = temp;

  // Rotate third row 3 columns to left
  temp = (*state)[0][3];
  (*state)[0][3] = (*state)[3][3];
  (*state)[3][3] = (*state)[2][3];
  (*state)[2][3] = (*state)[1][3];
  (*state)[1][3] = temp;
}

unsigned char xtime(unsigned char x)
{
  return ((x << 1) ^ (((x >> 7) & 1) * 0x1b));
}

// MixColumns function mixes the columns of the state matrix
void MixColumns(void)
{
  unsigned char i;
  unsigned char Tmp, Tm, t;
  for (i = 0; i < 4; ++i)
  {
    t = (*state)[i][0];
    Tmp = (*state)[i][0] ^ (*state)[i][1] ^ (*state)[i][2] ^ (*state)[i][3];
    Tm = (*state)[i][0] ^ (*state)[i][1]; Tm = xtime(Tm);  (*state)[i][0] ^= Tm ^ Tmp;
    Tm = (*state)[i][1] ^ (*state)[i][2]; Tm = xtime(Tm);  (*state)[i][1] ^= Tm ^ Tmp;
    Tm = (*state)[i][2] ^ (*state)[i][3]; Tm = xtime(Tm);  (*state)[i][2] ^= Tm ^ Tmp;
    Tm = (*state)[i][3] ^ t;        Tm = xtime(Tm);  (*state)[i][3] ^= Tm ^ Tmp;
  }
}
unsigned char Multiply(unsigned char x, unsigned char y)
{
  return (((y & 1) * x) ^
    ((y >> 1 & 1) * xtime(x)) ^
    ((y >> 2 & 1) * xtime(xtime(x))) ^
    ((y >> 3 & 1) * xtime(xtime(xtime(x)))) ^
    ((y >> 4 & 1) * xtime(xtime(xtime(xtime(x))))));
}
// MixColumns function mixes the columns of the state matrix.
// The method used to multiply may be difficult to understand for the inexperienced.
// Please use the references to gain more information.
void InvMixColumns(void)
{
  int i;
  unsigned char a, b, c, d;

  for (i = 0; i<4; ++i)
  {
    a = (*state)[i][0];
    b = (*state)[i][1];
    c = (*state)[i][2];
    d = (*state)[i][3];

    (*state)[i][0] = Multiply(a, 0x0e) ^ Multiply(b, 0x0b) ^ Multiply(c, 0x0d) ^ Multiply(d, 0x09);
    (*state)[i][1] = Multiply(a, 0x09) ^ Multiply(b, 0x0e) ^ Multiply(c, 0x0b) ^ Multiply(d, 0x0d);
    (*state)[i][2] = Multiply(a, 0x0d) ^ Multiply(b, 0x09) ^ Multiply(c, 0x0e) ^ Multiply(d, 0x0b);
    (*state)[i][3] = Multiply(a, 0x0b) ^ Multiply(b, 0x0d) ^ Multiply(c, 0x09) ^ Multiply(d, 0x0e);
  }
}

// The SubBytes Function Substitutes the values in the
// state matrix with values in an S-box.
void InvSubBytes(void)
{
  unsigned char i, j;

  for (i = 0; i<4; ++i)
  {
    for (j = 0; j<4; ++j)
    {
      (*state)[j][i] = getSBoxInvert((*state)[j][i]);
    }
  }
}

void InvShiftRows(void)
{
  unsigned char temp;

  // Rotate first row 1 columns to right
  temp = (*state)[3][1];
  (*state)[3][1] = (*state)[2][1];
  (*state)[2][1] = (*state)[1][1];
  (*state)[1][1] = (*state)[0][1];
  (*state)[0][1] = temp;

  // Rotate second row 2 columns to right
  temp = (*state)[0][2];
  (*state)[0][2] = (*state)[2][2];
  (*state)[2][2] = temp;

  temp = (*state)[1][2];
  (*state)[1][2] = (*state)[3][2];
  (*state)[3][2] = temp;

  // Rotate third row 3 columns to right
  temp = (*state)[0][3];
  (*state)[0][3] = (*state)[1][3];
  (*state)[1][3] = (*state)[2][3];
  (*state)[2][3] = (*state)[3][3];
  (*state)[3][3] = temp;
}


// Cipher is the main function that encrypts the plain_text.
void Cipher(void)
{
  unsigned char round = 0;

  // Add the First round key to the state before starting the rounds.
  AddRoundKey(0);

  // There will be Nr rounds.
  // The first Nr-1 rounds are identical.
  // These Nr-1 rounds are executed in the loop below.
  for (round = 1; round < Nr; ++round)
  {
    SubBytes();
    ShiftRows();
    MixColumns();
    AddRoundKey(round);
  }

  // The last round is given below.
  // The MixColumns function is not here in the last round.
  SubBytes();
  ShiftRows();
  AddRoundKey(Nr);
}

void InvCipher(void)
{
  unsigned char round = 0;

  // Add the First round key to the state before starting the rounds.
  AddRoundKey(Nr);

  // There will be Nr rounds.
  // The first Nr-1 rounds are identical.
  // These Nr-1 rounds are executed in the loop below.
  for (round = Nr - 1; round>0; round--)
  {
    InvShiftRows();
    InvSubBytes();
    AddRoundKey(round);
    InvMixColumns();
  }

  // The last round is given below.
  // The MixColumns function is not here in the last round.
  InvShiftRows();
  InvSubBytes();
  AddRoundKey(0);
}

void BlockCopy(unsigned char* output, unsigned char* input)
{
  unsigned char i;

  for (i = 0; i<KEYLEN; ++i)
  {
    output[i] = input[i];
  }
}
void AES128_ECB_encrypt(unsigned char *input, unsigned char *key, unsigned char *output)
{
  // Copy input to output, and work in-memory on output
  BlockCopy(output, input);
  state = (state_t*)output;

  Key = key;
  KeyExpansion();

  // The next function call encrypts the plain_text with the Key using AES algorithm.
  Cipher();
}

void AES128_ECB_decrypt(unsigned char *input, unsigned char *key, unsigned char *output)
{
  // Copy input to output, and work in-memory on output
  BlockCopy(output, input);
  state = (state_t*)output;

  // The KeyExpansion routine must be called before encryption.
  Key = key;
  KeyExpansion();

  InvCipher();
}


uint16_t crc16(const uint8_t *ptr, uint16_t len) {
    int i;
    uint16_t crc = 0xffff;
    while (len--) {
        crc ^= *ptr++;
        for (i = 0; i < 8; ++i) {
            crc = (crc & 1) ? ((crc >> 1) ^ 0xA001) : (crc >> 1);
        }
    }
    return ((crc >> 8) & 0xff) | ((crc << 8) & 0xff00);
}

void receiveModbusTcp(EthernetClient client, byte* modbusTcpFrame, unsigned int& length)
{
    byte modbusTcpFrame_temp[200] ; // Giới hạn kích thước bộ đệm tạm thời
    unsigned int length_temp = 0;
    uint16_t M1 = 0;
    uint16_t M = 0;
    unsigned long minisecl, minisech; 
    while (client.available()) {
        if (length < 39) { // Đảm bảo không vượt quá kích thước bộ đệm
            modbusTcpFrame_temp[length_temp] = client.read();
            length_temp++;
        } else {
            break;
        }
    }
    
    byte ecrypted_modbus[16];
    byte decrypted_modbus[16];

    // Tách lấy 32byte bị mã hóa
    for(int i = 0; i < 16; i++){
      ecrypted_modbus[i] = modbusTcpFrame_temp[7+i];
    }
    AES128_ECB_decrypt(ecrypted_modbus, key, decrypted_modbus); // Giải mã

    // Gộp lại thành chuỗi modbus ban đầu khi chưa mã hóa
    for(int i = 0; i < 16; i++){
      Serial.print(decrypted_modbus[i]);
      modbusTcpFrame_temp[i+7] = decrypted_modbus[i];
    }

    // Tách lấy 32byte bị mã hóa
    for(int i = 0; i < 16; i++){
      ecrypted_modbus[i] = modbusTcpFrame_temp[7+16+i];
    }
    AES128_ECB_decrypt(ecrypted_modbus, key, decrypted_modbus); // Giải mã

    // Gộp lại thành chuỗi modbus ban đầu khi chưa mã hóa
    for(int i = 0; i < 16; i++){
      Serial.print(decrypted_modbus[i]);
      modbusTcpFrame_temp[i+7+16] = decrypted_modbus[i];
    }

    byte functionCode = modbusTcpFrame_temp[15]; // FC nằm ở byte thứ 16 của gói TCP/IP
    Serial.print("Nhận được function: ");
    Serial.println(functionCode);

    if (functionCode == 0x03)
    {
      length = 20;
      M1 = crc16(modbusTcpFrame_temp, 37);

      // Tạo bộ đệm để chứa hai byte CRC cần giải mã
      unsigned char crcBytes[2];
      crcBytes[0] = modbusTcpFrame_temp[37];
      crcBytes[1] = modbusTcpFrame_temp[38];

      unsigned char decryptedCRC[2];
      AES128_ECB_decrypt(crcBytes, key, decryptedCRC);
      
      M = (decryptedCRC[1] << 8)  | decryptedCRC[0];
    }

    else if (functionCode == 0x06)
    {
      length = 20;
      M1 = crc16(modbusTcpFrame_temp, 37);
      
      // Tạo bộ đệm để chứa hai byte CRC cần giải mã
      unsigned char crcBytes[2];
      crcBytes[0] = modbusTcpFrame_temp[37];
      crcBytes[1] = modbusTcpFrame_temp[38];

      unsigned char decryptedCRC[2];
      AES128_ECB_decrypt(crcBytes, key, decryptedCRC);
      
      M = (decryptedCRC[1] << 8)  | decryptedCRC[0];
    }

    else if (functionCode == 0x10)
    {
      length = 20 ;//+ byteCount;
      M1 = crc16(modbusTcpFrame_temp, 37);
      
      // Tạo bộ đệm để chứa hai byte CRC cần giải mã
      unsigned char crcBytes[2];
      crcBytes[0] = modbusTcpFrame_temp[37];
      crcBytes[1] = modbusTcpFrame_temp[38];

      unsigned char decryptedCRC[2];
      AES128_ECB_decrypt(crcBytes, key, decryptedCRC);
      
      M = (decryptedCRC[1] << 8)  | decryptedCRC[0];
    }

    else {
        return; // Mã chức năng không hợp lệ, thoát sớm
    }
    // Kiếm tra CRC
    if (M == M1)
    {
      for (int i = 0; i < length; i++)
      {
        modbusTcpFrame[i] = modbusTcpFrame_temp[i];
      }
      for (int i = 0; i <50; i++)
      {
        modbusTcpFrame_temp[i] = 0;
      }
    }

}


void convertTcpToRtu(byte* modbusTcpFrame, byte* modbusRtuFrame, int length) {
    // Address Slave
    modbusRtuFrame[0] = modbusTcpFrame[6];
    // Function
    modbusRtuFrame[1] = modbusTcpFrame[15];
    if(modbusTcpFrame[6] == 0x01) {
        if(modbusTcpFrame[15] == 0x03) {
            // Address first reg
            modbusRtuFrame[2] = modbusTcpFrame[16];
            modbusRtuFrame[3] = modbusTcpFrame[17];
            // Total reg
            modbusRtuFrame[4] = modbusTcpFrame[18];
            modbusRtuFrame[5] = modbusTcpFrame[19];
            // crc
            uint16_t crc = crc16(modbusRtuFrame, 6);
            modbusRtuFrame[6] = crc / 256;
            modbusRtuFrame[7] = crc % 256;
            Serial2.write(modbusRtuFrame, 8);
        }
        
        if(modbusTcpFrame[15] == 0x06) {
            // Address reg
            modbusRtuFrame[2] = modbusTcpFrame[16];
            modbusRtuFrame[3] = modbusTcpFrame[17];
            // Value reg
            modbusRtuFrame[4] = modbusTcpFrame[18];
            modbusRtuFrame[5] = modbusTcpFrame[19];
            // crc
            uint16_t crc = crc16(modbusRtuFrame, 6);
            modbusRtuFrame[6] = crc / 256;
            modbusRtuFrame[7] = crc % 256;
            Serial2.write(modbusRtuFrame, 8);
        }

        if(modbusTcpFrame[15] == 0x10) {
            // Address the first reg
            modbusRtuFrame[2] = modbusTcpFrame[16];
            modbusRtuFrame[3] = modbusTcpFrame[17];
            // Total reg
            modbusRtuFrame[4] = modbusTcpFrame[18];
            modbusRtuFrame[5] = modbusTcpFrame[19];
            // Number byte
            modbusRtuFrame[6] = modbusTcpFrame[20];
            // Value reg
            for (int i = 21; i < length; i++) {
                modbusRtuFrame[i - 14] = modbusTcpFrame[i];
            }
            // crc
            uint16_t crc = crc16(modbusRtuFrame, length - 14);
            modbusRtuFrame[length - 14] = crc / 256;
            modbusRtuFrame[length - 13] = crc % 256;
            Serial2.write(modbusRtuFrame, length - 12);
        }
    }
}

void loop() {
    EthernetClient client = server.available();

    if (client) {
        byte modbusTcpFrame[39];
        byte modbusRtuFrame[39];
        byte modbusRtuResponse[39];

        unsigned int length = 0;
        unsigned int rtulenth = 0;
        unsigned int len = 0;
        long LastTime = 0;

        // Nhận và xử lý dữ liệu
        receiveModbusTcp(client, modbusTcpFrame, length);
        if (length >= 20) 
        {          
          for (int i = 0; i< length; i++)
          {
            Serial.print(modbusTcpFrame[i]);
            Serial.print("");
          }
          Serial.println("");
          convertTcpToRtu(modbusTcpFrame, modbusRtuFrame, length);
          delay(40);
          LastTime = millis();
          len = 0;
          while (1) 
          {  
            if ((millis() - LastTime) > 5) break;
            if (Serial2.available()) 
            {
              if ((millis() - LastTime) > 5) break;
              do 
              {                    
                modbusRtuResponse[len] = Serial2.read();
                if (len < 19) len++;
                LastTime = millis();
              } while (Serial2.available());
            }
          }
          for (int i = 0; i < len; i++)
          {
            Serial.print(modbusRtuResponse[i]);
            Serial.print("");
          } Serial.println("");

          modbusTcpFrame[2] = 0x00;
          modbusTcpFrame[3] = 0x00;

            if(modbusRtuResponse[0] == 0x01) {
                if(modbusRtuResponse[1] == 0x03) {
                    modbusTcpFrame[4] = 0x00;
                    modbusTcpFrame[5] = 0x07;

                    // ID
                    modbusTcpFrame[6] = 0x01;

                    // Timestamp
                    uint64_t TS = millis();
                    // Lưu dấu thời gian vào mảng send_data_tcp
                    uint64_t TS1 = TS;
                    modbusTcpFrame[7] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[8] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[9] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[10] = (byte)(TS & 0xFF); TS >>= 8;

                    modbusTcpFrame[11] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[12] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[13] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[14] = (byte)(TS & 0xFF);

                    // FC
                    modbusTcpFrame[15] = 0x03;
                    // Length data
                    modbusTcpFrame[16] = 0x04;
                    // data
                    modbusTcpFrame[17] = modbusRtuResponse[3];
                    modbusTcpFrame[18] = modbusRtuResponse[4];
                    modbusTcpFrame[19] = modbusRtuResponse[5];
                    modbusTcpFrame[20] = modbusRtuResponse[6];
                    // thêm 0 vào cuối để đủ 32 byte tính từ timestamp đến cuói
                    for(int i = 21; i < 37; i++)
                    {
                      modbusTcpFrame[i] = 0;
                    }

                    // Bổ sung CRC
                    uint16_t CRC = crc16(modbusTcpFrame, 37);
                    modbusTcpFrame[37] = CRC / 256;
                    modbusTcpFrame[38] = CRC % 256;
                    // Mã hóa từ TS (index = 7) đến cuối gói tin
                    for (int i = 0; i < 32; i++)
                    {
                      plain_text[i] = modbusTcpFrame[7+ i];
                    }
                    AES128_ECB_encrypt(plain_text, key, cipher_text);
                    for (int i = 0; i < 32; i++)
                    {
                      modbusTcpFrame[7 + i] = cipher_text[i];
                    }
                    // Gửi gói yêu cầu Modbus TCP tới Server
                    for(int i=0; i<37; i++)
                    {
                    Serial.print(modbusTcpFrame[i]);
                    Serial.print(" ");
                    } Serial.println(" ");

                    client.write(modbusTcpFrame, 39);
                }
                
                if(modbusTcpFrame[7] == 0x06) {
                    modbusTcpFrame[4] = 0x00;
                    modbusTcpFrame[5] = 0x07;

                    // ID
                    modbusTcpFrame[6] = 0x01;

                    // Timestamp
                    uint64_t TS = millis();
                    // Lưu dấu thời gian vào mảng send_data_tcp
                    uint64_t TS1 = TS;
                    modbusTcpFrame[7] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[8] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[9] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[10] = (byte)(TS & 0xFF); TS >>= 8;

                    modbusTcpFrame[11] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[12] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[13] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[14] = (byte)(TS & 0xFF);

                    // FC
                    modbusTcpFrame[15] = 0x06;
                    // data
                    modbusTcpFrame[16] = modbusRtuResponse[3];
                    modbusTcpFrame[17] = modbusRtuResponse[4];
                    modbusTcpFrame[18] = modbusRtuResponse[5];
                    modbusTcpFrame[19] = modbusRtuResponse[6];
                    // thêm 0 vào cuối để đủ 32 byte tính từ timestamp đến cuói
                    for(int i = 20; i < 37; i++)
                    {
                      modbusTcpFrame[i] = 0;
                    }

                    // Bổ sung CRC
                    uint16_t CRC = crc16(modbusTcpFrame, 37);
                    modbusTcpFrame[37] = CRC / 256;
                    modbusTcpFrame[38] = CRC % 256;
                    // Mã hóa từ TS (index = 7) đến cuối gói tin
                    for (int i = 0; i < 32; i++)
                    {
                      plain_text[i] = modbusTcpFrame[7+ i];
                    }
                    AES128_ECB_encrypt(plain_text, key, cipher_text);
                    for (int i = 0; i < 32; i++)
                    {
                      modbusTcpFrame[7 + i] = cipher_text[i];
                    }
                    // Gửi gói yêu cầu Modbus TCP tới Server
                    for(int i=0; i<37; i++)
                    {
                    Serial.print(modbusTcpFrame[i]);
                    Serial.print(" ");
                    } Serial.println(" ");

                    client.write(modbusTcpFrame, 39);
                    for(int i=0;i<39;i++){
                      Serial.print(modbusTcpFrame[i]);
                    }
                    Serial.println();
                }

                if(modbusTcpFrame[7] == 0x10) {
                    modbusTcpFrame[4] = 0x00;
                    modbusTcpFrame[5] = 0x06;

                    // ID
                    modbusTcpFrame[6] = 0x01;
                    // Timestamp
                    uint64_t TS = millis();
                    // Lưu dấu thời gian vào mảng send_data_tcp
                    uint64_t TS1 = TS;
                    modbusTcpFrame[7] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[8] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[9] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[10] = (byte)(TS & 0xFF); TS >>= 8;

                    modbusTcpFrame[11] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[12] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[13] = (byte)(TS & 0xFF); TS >>= 8;
                    modbusTcpFrame[14] = (byte)(TS & 0xFF);
                    // FC
                    modbusTcpFrame[15] = 0x10;
                    // data
                    modbusTcpFrame[16] = modbusRtuResponse[3];
                    modbusTcpFrame[17] = modbusRtuResponse[4];
                    modbusTcpFrame[18] = modbusRtuResponse[5];
                    modbusTcpFrame[19] = modbusRtuResponse[6];
                }
                // Gửi dữ liệu với timestamp
                client.write(modbusTcpFrame, 14); 
            }
        }
    }
}
