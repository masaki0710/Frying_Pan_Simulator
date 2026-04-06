#include <Servo.h>

const int X_PIN = 26;
const int Y_PIN = 27;
const int SERVO_PIN = 15;
const int BUTTON_PIN = 17;

Servo myServo;

struct Coordinate {
  long cx, cy;
};

struct Angle {
  int ax, ay;
};

const int N = 100;

void setup() {
  Serial.begin(115200);
  analogReadResolution(12);

  pinMode(BUTTON_PIN, INPUT_PULLUP);
  myServo.attach(SERVO_PIN, 500, 2400);
  myServo.write(0);
}

void loop() {
  Coordinate c = getCoordinate();
  Angle a = angleCalculation(c);

  int buttonState = digitalRead(BUTTON_PIN) == LOW ? 1 : 0;

  String message = String(a.ax) + "," + String(a.ay) + "," + String(buttonState);
  Serial.println(message);

  if (Serial.available() > 0) {
    String input = Serial.readStringUntil('\n');
    int progress = input.toInt();
    int angle = map(progress, 0, 100, 0, 180);

    if (progress >= 0 && progress <= 100){
      myServo.write(angle);
    }
  }

  delay(20);
}

Coordinate getCoordinate() {
  Coordinate c;
  long x = 0, y = 0;

  for (int i = 0; i < N; i++) {
    x += analogRead(X_PIN);
    y += analogRead(Y_PIN);
  }

  c.cx = x / N;
  c.cy = y / N;

  return c;
}

Angle angleCalculation(Coordinate c) {
  int MAX_X = 3105, MAX_Y = 3130;
  int MIN_X = 1910, MIN_Y = 1940;

  float oneAngleX = (MAX_X - MIN_X) / 180.000;
  float oneAngleY = (MAX_Y - MIN_Y) / 180.000;

  Angle a;
  a.ax = (c.cx - MIN_X) / oneAngleX - 90;
  a.ay = (c.cy - MIN_Y) / oneAngleY - 90;

  return a;
}