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

int ledStatusAux = HIGH;

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
      } else {
        ledStatusAux = LOW;
      }
      
      // set the LED with the ledStatusAux of the variable:
      digitalWrite(ledpin, ledStatusAux);
    }
  }
  else
    digitalWrite(ledpin, LOW);

  val = digitalRead(Key);
  if (val == LOW)
    Serial.println("BONUS");

  delay(150);
}
