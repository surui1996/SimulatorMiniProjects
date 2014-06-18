import socket
import time
from threading import Thread

class RobotClient(Thread):
    def __init__(self):
        Thread.__init__(self)
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		
        self.isUp = False
        self.stop = False
        self.GameState = "TELEOP"
		
        self.robotValues = {}
        self.robotValues["LeftOutput"] = 0.0
        self.robotValues["RightOutput"] = 0.0
		
        self.robotValues["EncoderLeft"] = 0.0
        self.robotValues["EncoderRight"] = 0.0
        self.robotValues["Gyro"] = 0.0
        
        self.keyboard = {}
        
        self.ballPossessed = False
        #self.robotValues["Accelerometer"] = 0.0

    def IsUp(self):
        return self.isUp
	
    def run(self):
        port = 4590
        self.client.connect(('127.0.0.1', port))
        self.isUp = True

        while True:
            self.UpdateRobotValues()
            
            messages = self.client.recv(1024)
            if messages.find(";") != -1:
                for msg in messages.split(";"):
                    if msg != "":
                        self.ParseMessage(msg)
            
            if self.stop == True:
                break
                
            time.sleep(0.005)
						
    def UpdateRobotValues(self):
        self.SendGetRequest("Gyro")
        self.SendGetRequest("EncoderLeft")
        self.SendGetRequest("EncoderRight")
    
    def ParseMessage(self, message):
        if message.find("KEY") != -1:
            s = message.split("KEY ")[1].split("=")
            self.keyboard[s[0]] = (s[1] == "True");
        elif message.find("=") != -1:
            self.SetInDictionary(message)
        elif message.find("STATE") != -1:
            self.GameState = message.split(" ")[1]
        elif message.find("POSSESS") != -1:
            self.ballPossessed = (message.split(" ")[1] == "True");
        elif message.find("STOP") != -1:
            self.stop = True
    
    def SetInDictionary(self, response):
        s = response.split("=")
        self.robotValues[s[0]] = float(s[1])	
    
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

    def IsPressed(self, keyString):
        if keyString not in self.keyboard:
            self.keyboard[keyString] = False
        msg = "KEY " + keyString + ";"
        self.client.send(msg)
        return self.keyboard[keyString]
    
    def TryToPossess(self):
        self.client.send("POSSESS;")
    
    def IsBallPossessed(self):
        return self.ballPossessed
        
    def Shoot(self):
        self.client.send("SHOOT;")
        self.ballPossesed = False
        
        
	

