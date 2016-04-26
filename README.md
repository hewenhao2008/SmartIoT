#SmartIoT project

The idea about this project is to have devices attached to Arduino microcontroller( for now) directly controlled from web page.
The main program feature should be high modularity and portabillity.
##How to set up:
   - Download zip and unzip it under folder SmartIoT in your web server root
     - If *python* 2.7 not installed, install it using ```sudo apt-get install python``` if on Linux, or from https://www.python.org if on Windows.
     - Now use pip to install next requirements:
          - Tornado
          - PyCrypto [ from http://www.voidspace.org.uk/python/modules.shtml#pycrypto on Windows ]
          - After that go to directory Server software/Back_Ends/Python and run PythonServer.py - <br> ```python PythonServer.py```
     - Upload support script for your preffered Arduino board.
     - Yu'll have for now to compile VB client with your server_ip, user_id, passwd, and devices database [edit it to fit your devices needs, but for now you have only BOOL (ON/OFF devices) , PWM(0-255 that defines pulse width), RGB(HEX color as param, but really for PWM devices), DOOR("OTVORI" - stands for open, "ZATVORI"- stands for close, essentialy SERVO device)
     - After that run it and connect to Arduino, It will connect to python server alone..
     - Change your login credentials in mysql database  ```pametnakuca``` in ```user_login``` table to fit your username and password
     - Go to web location http://your_server_ip/SmartIoT
     - Log in with your credetials and you're all set, you should have device list and device control options


So I attached some Arduino breakout board to my computer and I'll use it as a test client... The address to control my devices
is http://knjigazaknjigu.com/pk/pk and there you'll login with User ID(Korisnicki ID) : 1 , Password(Lozinka) : test1, if my computer
is on you'll be able to control my devices :)


###So far :
- Visual Basic client 
- Python/PHP in combination serves as server
- AJAX device state pulling
- Arduino Mega support
- Arduino Uno support 
- Web interface
- Devices supported : PWM, BOOLEAN, RGB, DOOR(This is acctualy a servo device)
- Triggers , to make event based device reaction [are not yet added to webpage]


###Should be done : 
- Web iterface redesign
- Better encription [maybe]
- Performance optimization
- Rewrite a program in a couple of languages [programming and human]

- Add more drivers for different devices
