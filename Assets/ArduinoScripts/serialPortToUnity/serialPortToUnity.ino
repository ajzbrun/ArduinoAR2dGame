//script for trying connection with unity
#define echoPin 2
#define trigPin 3
long duration;
int distance;
int ledState = 0;
int incoming;
unsigned long previousMillis = 0;
int ledpin = 12;
int Key = 10; // Declaration of the sensor input pin
int val;
int buzzer = 11;

int ledStatusAux = HIGH;
int currentNote = 0;
int lastNote = 0;
int melody[] = {
  262, 196, 196, 220, 196, 0, 247, 262
};
int noteDurations[] = {
  4, 8, 8, 4, 4, 4, 4, 4
};

void setup() {
  pinMode(ledpin, OUTPUT);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  pinMode (Key, INPUT_PULLUP) ;
  Serial.begin(9600);  
}

void loop() {  
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin HIGH (ACTIVE) for 10 microseconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);
  // Calculating the distance
  distance = duration * 0.034 / 2; // Speed of sound wave divided by 2 (go and back)
  // Displays the distance on the Serial Monitor
  
  Serial.println(distance);

  //start light and beeping region
  if (Serial.available() > 0) {
    while(Serial.peek() == 'L')
    {
      Serial.read();
      ledState = Serial.parseInt();
    }
    while(Serial.available() > 0) {
      Serial.read();
    }
  }

  if(ledState == 1)
  {
    unsigned long currentMillis = millis();
    if (currentMillis - previousMillis >= 300) {
      previousMillis = currentMillis;
      // if the LED is off turn it on and vice-versa:
      if (ledStatusAux == LOW) {
        ledStatusAux = HIGH;
        tone(buzzer, 523); //buzzer, tone  
      } else {
        noTone(buzzer);
        ledStatusAux = LOW;
      }

      // set the LED with the ledStatusAux of the variable:
      digitalWrite(ledpin, ledStatusAux);
    }

    if(currentNote < 8)
    {
      if(noteDurations[currentNote] <= 0)
        currentNote++;
      else{
        if(lastNote != currentNote)
        {
          int noteDuration = 1000 / noteDurations[currentNote];
          tone(buzzer, melody[currentNote], noteDuration);
        }
      
        noteDurations[currentNote]--;
      }
  
      lastNote = currentNote;
    }
    else
    {
      noTone(buzzer);
      //4, 8, 8, 4, 4, 4, 4, 4
      noteDurations[0] = 4;
      noteDurations[1] = 8;
      noteDurations[2] = 8;
      noteDurations[3] = 4;
      noteDurations[4] = 4;
      noteDurations[5] = 4;
      noteDurations[6] = 4;
      noteDurations[7] = 4;
      currentNote = 0;
      lastNote = 0;
    }
  }
  else{
    digitalWrite(ledpin, LOW); 
    noTone(buzzer);
  }

  val = digitalRead(Key);
  if (val == LOW)
    Serial.println("BONUS");

  delay(150);
}
