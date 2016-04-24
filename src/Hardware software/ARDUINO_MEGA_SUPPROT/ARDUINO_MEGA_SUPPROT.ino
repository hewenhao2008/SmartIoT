/*Created 10.04.2016.
This project is distributed under MIT Licence.
Here you have so far software for Arduino Mega IoT .. 
The idea is to support most of the hardware and write effective script for controlling any hardware directly from web
using simple functions..
Client in further text is software that is connected to Arduino with serial port, and communicates with server


This only applies to controllers that have same pinout as ARDUINO MEGA and are compatible with Arduino 
P.S. sorry if there is too many comments :) 
*/
#include <Servo.h> // included so we can control servo motor
#include <Arduino.h>             
#include <wiring_private.h> 
#include <pins_arduino.h>  //included for digitalReadOutputPin();
Servo servo_motor; //for controlling servo motor
#define INPUT_SIZE 250 // define  the max size of serial message that can be recived 
byte analogPins[] = {A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15}; //define all analog pins to be read.
byte pwmPins[] =    {2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 44, 45, 46}; //pwm pins
byte pwmValues[] =  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; // initialize all pwm values to 0
byte servoPos[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; //initialize servo positions for pwm pins 
char ret[300]; //used in printing states back to client software 
char buf[10]; // holding pin and value in printing states
unsigned long sendAgain = 0; // used to determine inteval for sending current states bact to client
void setup() {
  Serial.begin(115200); //being serial communication
  for (byte i = 2; i < 54; i++) {  //suppose all pins are output for now, don't touch rx, tx
    pinMode(i, OUTPUT);
    digitalWrite(i, 0); // set states to 0 
  }
  for(byte i = 0;i<sizeof(analogPins);i++){
    pinMode(analogPins[i],INPUT); // set them as inputs anyway
  }
  sendAgain = millis(); //  not needed ? 
}
void loop() {
  if (millis() - sendAgain > 100) { //send all states 
    sendAgain = millis();
    printDigitalStates(); // send states of all digital pins in form <ds>pin:value,pin1:value1,...</ds> and at client side user regex to pull inner data
    delay(20); // leave a bit of time not needed so fast..it's still realtime 
    printPwmStates();// send states of all pwm pins in form <pwm>pin:value,pin1:value1,...</pwm> 
    delay(20);
    printAnalogStates();// send states of all analog pins in form <an>pin:value,pin1:value1,...</an> 
    delay(20);
    printServoStates();// send states of all servos in form <servo>pin:angle,pin1:angle1,...</servo> 
    delay(20);
    discardBuffer(); // send to client so it can discard serial buffer.
  }
  if (Serial.available() > 0) { //serial data incoming
    char input[INPUT_SIZE + 1]; // initialize input char array 
    memset(input, 0, sizeof(input)); //give it a bit of space 
    byte index = 0; // start filling it
    while (Serial.available() > 0) {
      input[index] = Serial.read();
      index++; // next char
      delayMicroseconds(600); // commands will be strings so we have to be sure that every message arrives in good shape
    }

    char *pch; //try to parse data in format x1:x2:x3 , maybe later we'll use more but for now..
    pch = strtok(input, ":"); // get first part
    if (pch != 0) { // if it can be spllited once
      char* function = pch; // called first parameter function ? 
      pch = strtok(0, ":"); //next parameter
      char* pin = pch; // second parameter is pin
      pch = strtok(0, ":"); //third time split
      if (pch != 0) {
        char* state = pch; // third param is state 
        if (strcmp(function, "digitalWrite") == 0) { // digitalWrite:2:1 should write 1 at pin 2
          digitalWrite(atoi(pin), atoi(state)); // write it down
          if (digitalReadOutputPin(atoi(pin)) == atoi(state)) {
            Serial.println("<s>OK</s>"); // if ok return ok to program, might be used later, but it should always be OK.. don't need if if is needed
           }
        } else if (strcmp(function, "pinMode") == 0) {//pinMode:2:0 -> set pin 2 to OUTPUT, pinMode:2:1 -> set pin 2 to INPUT
          if (atoi(state) == 0) {
            pinMode(atoi(pin), OUTPUT);
          } else if (atoi(state) == 1) {
            pinMode(atoi(pin), INPUT);
          }
          Serial.println("<s>OK</s>"); // return OK
        } else if (strcmp(function, "analogWrite") == 0) { //analogWrite for PWM pin .. analogWrite:pwmPIN:0-255 -> write analog value 0-255 to pin pwmPIN
          for (int i = 0; i < sizeof(pwmPins); i++) {
            if (pwmPins[i] == atoi(pin)) {
              analogWrite(atoi(pin), atoi(state));
              pwmValues[i] = atoi(state); // change state in pwmValues array, because it's used for sending states
              Serial.println("<s>OK</s>"); //return OK
            }
          }

        } else if (strcmp(function, "servoMove") == 0) { //servoMove for PWMpin
          int p = atoi(pin);
          for (int i = 0; i < sizeof(pwmPins); i++) {
            if (pwmPins[i] == p) {
              if(servo_motor.attached()){ // if attached to pin, detach it
                servo_motor.detach();
              }
              servo_motor.attach(p); //attach it to current pin
              servo_motor.write(atoi(state)); // move to position
              servoPos[i] = atoi(state); // change servoPos because it's used for sending states
              Serial.println("<s>OK</s>"); //return ok
              }
          }


        } else { // command not found 
          Serial.println("Wrong :P"); // means nothing, maybe say to program the function does not exist
        }
      delay(10);
      Serial.flush();
      }


    }

  }



}
void discardBuffer() {
  Serial.println("<clb></clb>"); // send to the client 
}

//these down are same functions printing every item in array with formating explained at beggining of file..
void printAnalogStates() { 
  strcpy(ret,"<an>");
  for (byte i = 0; i < sizeof(analogPins); i++) {
    if (i != sizeof(analogPins) - 1) {
      snprintf(buf, sizeof buf, "%i", analogPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", analogRead(analogPins[i]));
      strcat(ret, buf);
      strcat(ret, ",");
    } else {
      snprintf(buf, sizeof buf, "%i", analogPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", analogRead(analogPins[i]));
      strcat(ret, buf);
      strcat(ret, "</an>\n");
    }
  }
  Serial.print(ret);
}
void printPwmStates() {
  strcpy(ret, "<pwm>");
  for (byte i = 0; i < sizeof(pwmPins); i++) {
    if (i != sizeof(pwmPins) - 1) {
      snprintf(buf, sizeof buf, "%i", pwmPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", pwmValues[i]);
      strcat(ret, buf);
      strcat(ret, ",");
    } else {
      snprintf(buf, sizeof buf, "%i", pwmPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", pwmValues[i]);
      strcat(ret, buf);
      strcat(ret, "</pwm>\n");
    }
  }
  Serial.print(ret);
}
void printDigitalStates() { // only this goes from 2 to 53 .. 
  strcpy(ret, "<ds>");
  for (byte i = 2; i < 54; i++) {
    if (i != 53) {
      snprintf(buf, sizeof buf, "%i", i);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", digitalReadOutputPin(i));
      strcat(ret, buf);
      strcat(ret, ",");
    } else {
      snprintf(buf, sizeof buf, "%i", i);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", digitalReadOutputPin(i));
      strcat(ret, buf);
      strcat(ret, "</ds>\n");
    }
  }
  Serial.print(ret);
}
void printServoStates() {
  strcpy(ret, "<servo>");
  for (byte i = 0; i < sizeof(servoPos); i++) {
    if (i != sizeof(servoPos) - 1) {
      snprintf(buf, sizeof buf, "%i", pwmPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", servoPos[i]);
      strcat(ret, buf);
      strcat(ret, ",");
    } else {
      snprintf(buf, sizeof buf, "%i", pwmPins[i]);
      strcat(ret, buf);
      strcat(ret, ":");
      snprintf(buf, sizeof buf, "%i", servoPos[i]);
      strcat(ret, buf);
      strcat(ret, "</servo>\n");
    }
  }
  Serial.print(ret);
}
int digitalReadOutputPin(uint8_t pin) //used to get pin state
{
  uint8_t bit = digitalPinToBitMask(pin);
  uint8_t port = digitalPinToPort(pin);
  if (port == NOT_A_PIN)
    return LOW;

  return (*portOutputRegister(port) & bit) ? HIGH : LOW;
}

