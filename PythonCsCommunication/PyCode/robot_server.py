import socket
import time
from threading import Thread

class RobotServer(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.client = None
		
        self.GameState = "TELEOP"
		
        self.robotValues = {}
        self.robotValues["LeftOutput"] = 0.0
        self.robotValues["RightOutput"] = 0.0
		
        self.robotValues["EncoderLeft"] = 0.0
        self.robotValues["EncoderRight"] = 0.0
        self.robotValues["Gyro"] = 0.0
        
        self.keyboard = {}
        #self.robotValues["Accelerometer"] = 0.0

    def IsUp(self):
        return self.client is not None
	
    def SendGetRequest(self, name):
        if name == "LeftOutput" or name == "RightOutput":
            return
        get = "GET " + name + ";"
        self.client.send(get)

    def SendTankDriveRequest(self, left, right):
        self.robotValues["LeftOutput"] = left
        self.robotValues["RightOutput"] = right
        set = "TANK " + str(left) + "," + str(right) + ";"
        self.client.send(set)
			
    def SendArcadeDriveRequest(self, forward, rotate):
        set = "ARCADE " + str(forward) + "," + str(rotate) + ";"
        self.client.send(set)
    
    #"ENCODERS" / "GYRO"
    def SendResetMessage(self, reset):
        set = "RESET " + reset + ";"
        self.client.send(set)
        
        if reset == "ENCODERS":
            self.robotValues["EncoderLeft"] = 0.0
            self.robotValues["EncoderRight"] = 0.0
        elif reset == "GYRO":
            self.robotValues["Gyro"] = 0.0
	
    def SetInDictionary(self, response):
        s = response.split("=")
        self.robotValues[s[0]] = float(s[1])
        #self.client.send(s[0] + ":" + str(self.robotValues[s[0]]) + ";")
	
    def ParseMessage(self, message):
        if message.find("KEY") != -1:
            s = message.split("KEY ")[1].split("=")
            self.keyboard[s[0]] = (s[1] == "True");
        elif message.find("=") != -1:
            self.SetInDictionary(message)
        elif message.find("STATE") != -1:
            self.GameState = message.split(" ")[1]
    
    def IsPressed(self, keyString):
        if keyString not in self.keyboard:
            self.keyboard[keyString] = False
        msg = "KEY " + keyString + ";"
        self.client.send(msg)
        return self.keyboard[keyString]
	
    def run(self):
        port = 4590
        self.sock.bind(('127.0.0.1', port))
        self.sock.listen(5)
        self.client, addr = self.sock.accept()

        while True:
            messages = self.client.recv(1024)
            if messages.find(";") != -1:
                for msg in messages.split(";"):
                    if msg != "":
                        #try:
                        self.ParseMessage(msg)
                        #except:
                        #    pass
            self.UpdateRobotValues()
            time.sleep(0.005)
						
    def UpdateRobotValues(self):
        self.SendGetRequest("Gyro")
        self.SendGetRequest("EncoderLeft")
        self.SendGetRequest("EncoderRight")
