#include <Servo.h>
#include <Wire.h>

#define I2C_ADDRESS 0x50

#define NUM_SAMPLES 10
#define SERVO_MIN 0
#define SERVO_MAX 180

const int trigPin = 9;
const int echoPin = 10;
const int servoPin = 9;

Servo servo;
int sweepPosition;
int sweepDirection;
bool status;
int ranges[NUM_SAMPLES];

void setup()
{
    pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
    pinMode(echoPin, INPUT);  // Sets the echoPin as an Input

    servo.attach(servoPin);

    for (int i = 0; i < NUM_SAMPLES; i++)
        ranges[i] = -1;

    sweepPosition = 0;
    sweepDirection = 1;
    status = false;

    Wire.begin(I2C_ADDRESS);
    Wire.onReceive(i2cRecieved);

    Serial.begin(9600); // Starts the serial communication
}

void loop()
{
    if (status)
    {
        //TODO: Do we need to disable interupts during getDistance?
        int servoPosition = SERVO_MIN + ((SERVO_MAX - SERVO_MIN) * ((float)sweepPosition / NUM_SAMPLES));

        servo.write(servoPosition);
        delay(15);

        ranges[sweepPosition] = getDistance();

        sweepPosition += sweepDirection;

        if (sweepPosition == NUM_SAMPLES - 1)
            sweepDirection = -1;
        else if (sweepPosition == 0)
            sweepDirection = 1;
    }
}

int getDistance()
{
    // Clears the trigPin
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);

    // Sets the trigPin on HIGH state for 10 micro seconds
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);

    // Reads the echoPin, returns the sound wave travel time in microseconds
    long duration = pulseIn(echoPin, HIGH);

    // Calculating the distance
    return duration * 0.034 / 2;
}

void i2cRecieved(int numBytes)
{
    int registerAddress = Wire.read();
    switch (registerAddress)
    {
    case 0x1:
        if (Wire.available() > 0)
        {
            //Set status
            status = Wire.read() == 1;
        }
        else
        {
            //Return status
            Wire.write(status ? 1 : 0);
        }

        break;
    case 0x2:
        Wire.write(NUM_SAMPLES);

        for (int i = 0; i < NUM_SAMPLES; i++)
            Wire.write(ranges[i]);

        break;
    }
}