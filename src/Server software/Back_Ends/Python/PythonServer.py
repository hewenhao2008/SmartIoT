 
import errno
import functools
import socket
import tornado
import json
from tornado import ioloop, iostream
import time
from ast import literal_eval
from Crypto.Cipher import AES
import base64
import os
BLOCK_SIZE = 16
key = "\xff\xaf\xc3\x67\x67\xbb\xab\xbf\x88\x54\xf1\x22\x96\xf4\xaa\xaf" #key
iv = "\xcc\xfb\xc3\x44\x67\xbb\xba\xcf\x88\x54\xf1\x22\x96\xf4\xaa\xaf" #init vector
replace_plus = "BIN00101011BIN"
serverAddress = '127.0.0.1'  # should be 127.0.0.1  
clients = {} #list of houses(clients) to control

def enum(**enums):
    return type('Enum', (), enums)

def repeat_to_length(string_to_expand, length):
    return (string_to_expand * ((length/len(string_to_expand))+1))[:length]

pad = lambda s: s + (BLOCK_SIZE - len(s) % BLOCK_SIZE) \
* chr(BLOCK_SIZE - len(s) % BLOCK_SIZE)
unpad = lambda s : s[0:-ord(s[-1])]

# one-liners to encrypt/encode and decrypt/decode a string
# encrypt with AES, encode with base64
def EncodeAES(s):
    c = AES.new(key, AES.MODE_CBC, iv)
    s = pad(s)
    s = c.encrypt(s)
    s = base64.b64encode(s)
    return s.replace("+", replace_plus)

def DecodeAES(enc):
    c = AES.new(key, AES.MODE_CBC, iv)
    enc = enc.replace(replace_plus, "+")
    enc = base64.b64decode(enc)
    enc = c.decrypt(enc)
    return unpad(enc)


elisium_user_commands={"servudl","ping","ok"}
cli_status = enum(normal = 0, serv_res = 1) #for now ?


class Connection(object):
    def __init__(self, connection,address):
        self.connection = connection #for disconnection now :D 
        self.address=address #get ip
        self.stream = iostream.IOStream(connection) #get stream

        self.stream.set_close_callback(self._onClose) #client closed
      #  self._read()
        if self.address[0] == serverAddress: #connection from server
            self._serv_read() #go to serv read
            #print "Conneciton from server" #info for debug
        else: #if not server then client
            print "Connection from client"
            self._send("WELCOME TO ELISIUM :)\n") #send this message
            # sounds cool D:
            self.sch = [] #add schedule server list , if there is better way, implement
            self.uid = None #uid 
            self.devices = []#all devices client has
            self.passwd = None #his passwd ? after auth with mysql ?
            self.stat = None #stat of client
            self._get_login() #login client ?
    def __del__(self): #if class deleted 
        class_name = self.__class__.__name__ #get class name 
        if not self.address[0]==serverAddress:
	 	print class_name, "destroyed" #print to screen for debug
    def _gds(self): #get dev states
        return self.devices #return because will be sent to serv ? client? 
    def _read(self): #read client
        try: #if closed in transit
            self.stream.read_until('\n', self._eol_callback) #set callback when \n
        except  iostream.StreamClosedError:
            print "Error with open sock, closed"
    def _serv_read(self): #read server
        try: #closed in transit
            self.stream.read_until('\n',self._serv_eol_callback) #set callback when \n if timeouts ??
        except iostream.StreamClosedError:
            print "Error with open sock, closed"
    def _serv_eol_callback(self,data): #serv callback , get newline stripped and handle data
        data = data.strip('\n')
        self.handle_serv_data(data)
        #print("callback " + data)
    def _eol_callback(self, data): #client callback, stip newline and handle data
        data = DecodeAES(data)
        data = data.strip('\n')
        self.handle_data(data)
    def _close(self): #close the connection 
        self.stream.close()#close the stream
       # self.stream.close_fd() #was thinking about this , but it's not recommended
        self.connection.close()#close the connection
    def _send(self,message): #send 
        try:  
            if self.address[0] != serverAddress:
                message = EncodeAES(message)
            self.stream.write(message + '\n')
        except iostream.StreamClosedError: #if closed connection when send
            print "Error with open sock, closed"
    def _checkpass(self,passwd): # bla bla, should be sql check ? 
        if passwd:
            return True
        else:
            return False
    def _onClose(self): #on close remove from clients
        global clients
        if self.address[0] != serverAddress:
             if self.uid in clients:
                  clients.pop(self.uid,None) #pop it from list
                  print clients #print clients list ? if exists ? ?? ??
             
    def _auth(self,data): ##auth, every client must pass, or terminate, should add more security to server
        global clients #modify list of clientes 
        data = DecodeAES(data)
        if len(data.split(":")) == 3: #must be 3 splitted by :
          try: #try, might fali ? ?
            login=True #for now login is true
            splited = data.split(":") # after first think cleared, get the splitted data
            if splited[0].isdigit(): #uid must be digit ?
                if int(splited[0]) not in clients:
                    self.uid = int(splited[0])
                else:
                    self._send("Client with uid exists");
                    self._close()
            else:
                login=False #terminate client
                self._send("Invalid info\n") #should never happen
                self._close()
                
            if self._checkpass(splited[1]): # check pass 
                 self.passwd = splited[1]#id true, return
            else:
                login=False #terminate client
                self._send("Invalid info\n") #should never happen
                self._close()
            try:
                 tempdev = literal_eval(splited[2]) #raise syntax error if can't read ? ??
                 if isinstance(tempdev,list): #check if list  ?? 
                     self.devices=tempdev #append to client object
                 else:
                     login=False #terminate client connection
                     self._send("Invalid info\n") #should never happen
                     self._close()
            except SyntaxError: #literal eval might raise this if data malformed
                  login=False
                  print "Not good params!" #let them know ?? ?
                  self._send("Invalid info\n") #should never happen
                  self._close()
            if login: # if all passed append user if not, he's allready terminated
                clients[self.uid] = self #append object of CLIENT CONNECTION 
                print clients #debug client list
                self.stat == cli_status.normal
                self._read() #start reading
          except ValueError: # int ? ?
               self._send("Don't bother! You're distroyed")  #destroy client 
               self._close()
               pass
        else:
            print data #if not 3 arguments 
            self._send("Login failed\n") #should never happen except hacKK KKK KKKK KKK
            self._close()
            #self._get_login() #again ( we'll decide on the go if we want to enable another try)
    def _get_login(self): ## check for user cerenditials
            self.stream.read_until('\n',self._auth) ## read stream and call _auth
    
            
def connection_ready(sock, fd, events): #accept connections, and add handlers
    while True:
        try:
            connection, address = sock.accept()
        except socket.error, e:
            if e[0] not in (errno.EWOULDBLOCK, errno.EAGAIN):
                  print e
		  raise
            return
        else:
            connection.setblocking(0)
            CommunicationHandler(connection,address)

class CommunicationHandler(Connection): # main functtions of connection class
    """Put your app logic here"""
    def handle_data(self, data): #this is client data we reciverd,and what is done is by status  ??
        if data.lower().split(":")[0] not in elisium_user_commands:
              if not self.sch: #if there is no more schedule get back to normal
                   self.stat=cli_status.normal
              if self.stat == cli_status.serv_res: # we have to respond to server? ??
                   if self.sch: #if sch not empty
                        serv = self.sch[0] #get serv object /
                        serv._send(data + "\n") #send him back the data of client ? shouldd be positeive ? 
                        serv._close() #don't ever maintainn connection with server ?
                        del self.sch[0] #delte first in schedule ?
                   else:self.stat = cli_status.normal
              elif self.stat == cli_status.normal: #if normal return back to client( has to be overwritten)
                   self._send(data+'\n')
                   print self.uid," send and repsonded", data
        else:
            if len(data.split(":")) > 0:
                cmd = data.lower().split(":")[0]
                cmd_data = None
		if len(data.split(":")) == 2:
		    cmd_data = data.split(":")[1]
		    print cmd_data
                try:
                    if cmd == "servudl":
                         tempdev = literal_eval(cmd_data) #raise syntax error if can't read ? ??
                         if isinstance(tempdev,list): #check if list  ?? 
                             self.devices=tempdev
                             print self.uid , self.devices
                         else:
                             self._send("Invalid info\n") #should never happen
                             self._close()
                    if cmd == "ping":
		         self._send("pong\n")
		    if cmd == "ok":
		 	 pass
                except SyntaxError: #literal eval might raise this if data malformed
                    self._send("Invalid info\n") #should never happen
		    print "Closed cause of sintax"
                    self._close()
                except ValueError:
		    print "Not good !!"
                    self._send("Invalid info\n") #should never happen
                    self._close()
            else:
                self._send("Invalid info\n") #should never happen
                self._close()

        
	self._read()#fire again
               
    def handle_serv_data(self,data): #process server data  ?? 
          serv_data = data.split(":")
         # print data
          try:
                function = serv_data[0]
                uid = int(serv_data[1])
                if uid in clients:
                        password = serv_data[2]
                        if  password == clients[uid].passwd: 
                            function=function.lower()
                            params = serv_data[3:]
                           # print params
                          #server core
                            if function == "changestate":
                                
                                clients[uid]._send("%s:%s\n"%(function,':'.join(params)))#send function string with params
                                self._send("OK\n")
                            elif function == "getdevicestate":
                                self._send("DEVICES:"+ json.dumps(clients[uid].devices))
                            else:
                                self._send("ERROR:000:No function \n")
                                self._close()
                            #print clients[uid].sch #just to see  
                        else:
                            self._send("ERROR:001:Incorrect password\n") #should not happen 
                            self._close()
                else:
                        self._send("ERROR:002:Client not online\n")
                        self._close()
       

          except ValueError:
               self._send("Invalid data") #should not happen
               self._close()
          except IndexError:
               self._send("Invalid data") #should not happen
               self._close()
          except:
               self._send("Data couldn't be sent to client") #should not happen
               self._close()

if __name__ == '__main__':
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM, 0)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    sock.setblocking(0)
    sock.bind(("", 5000))
    sock.listen(128)
    io_loop = ioloop.IOLoop.instance()
    callback = functools.partial(connection_ready, sock)
    io_loop.add_handler(sock.fileno(), callback, io_loop.READ)
    try:
        io_loop.start()
    except KeyboardInterrupt:
        io_loop.stop()
        print "Exited cleanly"

