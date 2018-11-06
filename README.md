MiniRover
-----

The minirover is a small roving robotic platform. It has 4 wheels with independent steering and drive. It is capable of lowering or raising itself.

For sensors it has an IMU, a ultra sound based radar system and stereo vision cameras.

# Electronics
The main electronics is a Rasperry PI 2 with an Adafruit servo shield. Specific subsystems like the radar uses its own Arduino. 

The main communication bus used between devices is I2C.

# Software
The onboard software is written in C# running on .NET Core.

