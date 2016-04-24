#SmartIoT project

The idea about this project is to have devices attached to Arduino microcontroller( for now) directly controlled from web page.
The main program feature should be high modularity and portabillity.
##How to set up:
   - Download zip and unzip it under folder SmartIoT in your web server root
     - If *python* 2.7 not installed, install it using ```sudo apt-get install python```
     - Now use pip to install next requirements:
          - <b>Tornado</b>
          - <b>PyCrypto</b>
     - After that go to directory Server software/Back_Ends/Python and run PythonServer.py - ```python PythonServer.py```
     - Use client software to connect to device and to server, just build it with yours **server ip**, your **user id**, and your **password**. Change device database to your needs.
     - Change your login credentials in mysql database ```smartiot``` in ```user_login``` table to fit your username and password
     - Go to web location http://your_server_ip/SmartIoT
     - Log in with your credetials and you're all done 


###So far :
- Visual Basic client 
- Python/PHP in combination serves as server
- AJAX device state pulling
- Arduino Mega support
- Arduino Uno support 
- Web interface
- Devices supported : PWM, BOOLEAN, RGB, DOOR(This is acctualy a servo device)



###Should be done : 
- Web iterface redesign
- Better encription
- Performance optimization
- Rewrite a program in a couple of languages [programming and human]
- Add more drivers for different devices
