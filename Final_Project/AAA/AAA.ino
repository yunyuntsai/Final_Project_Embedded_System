#include <ArduinoJson.h>
#include <Servo.h>
#include <LiquidCrystal_I2C.h>
#include "pitches.h"

//LCD                       
LiquidCrystal_I2C lcd(0x3F, 2, 1, 0, 4, 5, 6, 7, 3, POSITIVE);

//Servo
#define SERVO_PIN 6
Servo myservo;

//Button 
#define BUTTON_PIN 2
#define BUTTONLIGHT_PIN 13

//Buzzer
#define BUZZER_PIN 7
#define NOTE_NUM 8
int melody[NOTE_NUM] = {
    NOTE_C5, NOTE_G4, NOTE_G4, NOTE_A4, NOTE_G4, 0, NOTE_B4, NOTE_C5
};
int melody1[NOTE_NUM] = {
    NOTE_C5, NOTE_B4, NOTE_A4, NOTE_G4, NOTE_F4,  NOTE_E4, NOTE_D4, NOTE_C4
};
int noteDurations[NOTE_NUM] = {
    4, 8, 8, 4, 4, 4, 4, 4  
};
int noteDurations1[NOTE_NUM] = {
    8, 8, 8, 8, 8, 8, 8, 8 
};
//RGB
int redPin = 11;
int greenPin = 10;
int bluePin = 9;

String readString;
int ButtonState = 0;

// setup() 函數只會於電源啟動時執行1次
void setup() 
{
    ButtonState = 0;
  
   //Serial Setting  
    Serial.begin(9600);  
    Serial.println("start");

    //initialize the lcd 
    lcd.begin(16, 2);    // initialize LCD
    lcd.backlight();    // open LCD backlight
    lcd.setCursor(0, 0);  // setting cursor
    lcd.print("Who is coming!");

    //Servo Setting
    myservo.attach(SERVO_PIN);
    myservo.write(0);

    //Pin Mode Setting
    pinMode(BUTTON_PIN,INPUT);
    pinMode(BUTTONLIGHT_PIN,OUTPUT);
    pinMode(BUZZER_PIN,OUTPUT);
    digitalWrite(BUTTONLIGHT_PIN, LOW);
    pinMode(redPin, OUTPUT);
    pinMode(greenPin, OUTPUT);
    pinMode(bluePin, OUTPUT);
          analogWrite(redPin, 255);
        analogWrite(greenPin, 255);
        analogWrite(bluePin, 255);
}

char C_msgJSON[70]= "";
String s = "";
DynamicJsonBuffer jsonBuffer(JSON_OBJECT_SIZE(8));
String Name;
bool Lock = false;
bool doorStatus;
int val_R = 0;
int val_G = 0;
int val_B = 0;
// loop() 函數會不斷的被重複執行
void loop()
{
   if (Serial.available()>0) {
      delay(3);  
      s = Serial.readString();
      //readString += c; 
      //readString.trim();
      s.toCharArray(C_msgJSON,70);  
      Serial.println("........");
      Serial.println(C_msgJSON);
      delay(500);
      JsonObject& root = jsonBuffer.parseObject(C_msgJSON);
      if (!root.success()) {
              Serial.println("parseObject() failed");
              return;
       }
        String n = root["name"];
        Name  = n;
        Lock = root["Lock"];                         
        Serial.print("Lock : ");
        Serial.println(Lock);
        val_R = root["R"];                      Serial.print("val_R:");Serial.println(val_R);
        val_G = root["G"];                      Serial.print("val_G:");Serial.println(val_G);
        val_B = root["B"];                      Serial.print("val_B:");Serial.println(val_B);
        lcd.setCursor(0, 1);
        lcd.print("                ");
        lcd.setCursor(0, 1);
        lcd.print(Name);
        analogWrite(redPin, val_R);
        analogWrite(greenPin, val_G);
        analogWrite(bluePin, val_B);
  }
  if (doorStatus != Lock) {
        if (Lock == 1){
            OpenDoor();
            lcd.setCursor(11, 1);
            lcd.print("      ");
            lcd.setCursor(11, 1);
            lcd.print("Open !");
            doorStatus = Lock;
        }
        else
        {
            CloseDoor();
            lcd.setCursor(11, 1);
            lcd.print("      ");
            lcd.setCursor(11, 1);
            lcd.print("Close !");
            doorStatus = Lock;
        }
  } 
   else if(doorStatus == 1){
        ButtonState = digitalRead(BUTTON_PIN);
        if(ButtonState == HIGH){
             //Serial.println("ButtonState :");
            //Serial.println(ButtonState);
            CloseDoor();
            digitalWrite(BUTTONLIGHT_PIN, LOW);
            lcd.setCursor(0, 1);
            lcd.print("                ");
            lcd.setCursor(0, 1);
            lcd.print("User close");     
            doorStatus = false;    
            Lock = 0;
            delay(2000);
            lcd.clear();
            lcd.setCursor(0, 0);  // setting cursor
            lcd.print("Who is coming!");
            analogWrite(redPin, 255);
            analogWrite(greenPin, 255);
            analogWrite(bluePin, 255);
        }
    }
}


void OpenDoor(){
    PlayMusic1();
    for(int i = 00; i <= 90; i+=2){
        myservo.write(i); // 使用write，傳入角度，從0度轉到180度
        delay(5);    
    }
}
void CloseDoor(){
    PlayMusic2();
    for(int i = 90; i >= 0; i-=2){
        myservo.write(i);// 使用write，傳入角度，從180度轉到0度
        delay(5);   
    }
}

void PlayMusic1(){
    for(int thisNote = 0; thisNote < NOTE_NUM ; thisNote++){
        int noteDuration = 1000 / noteDurations[thisNote];
        tone(BUZZER_PIN, melody[thisNote], noteDuration);
        int pauseNote = noteDuration * 1.30; 
        delay(pauseNote);
        noTone(BUZZER_PIN);    
    }
} 

void PlayMusic2(){
    for(int thisNote = 0; thisNote < NOTE_NUM ; thisNote++){
        int noteDuration = 1000 / noteDurations1[thisNote];
        tone(BUZZER_PIN, melody1[thisNote], noteDuration);
        int pauseNote = noteDuration * 1.30; 
        delay(pauseNote);
        noTone(BUZZER_PIN);    
    }
} 

