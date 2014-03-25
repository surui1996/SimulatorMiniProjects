import socket
import time
from threading import Thread

class RobotServer(Thread):
	def __init__(self):
		Thread.__init__(self)
		self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.client = None
		
		self.robotValues = {}
		self.robotValues["LeftOutput"] = 0.0
		self.robotValues["RightOutput"] = 0.0
		
		self.robotValues["EncoderLeft"] = 0.0
		self.robotValues["EncoderRight"] = 0.0
		self.robotValues["Gyro"] = 0.0
		self.robotValues["Accelerometer"] = 0.0
	
	#TODO: add checks whether the name is valid
	def SendGetRequest(self, name):
		get = "GET " + name + ";"
		self.client.send(get)
	
	def SendSetRespone(self, name, value):
		self.robotValues[name] = value
		set = "SET " + name + "=" + str(value) + ";"
		self.client.send(set)
	
	def SetInDictionary(self, response):
		s = response.split("=")
		self.robotValues[s[0]] = float(s[1])
		self.client.send("Hi I got from you " + s[0] + ":" + str(self.robotValues[s[0]]))
	
	def run(self):
		port = 4590
		self.sock.bind(('127.0.0.1', port))
		self.sock.listen(5)
		self.client, addr = self.sock.accept()

		while True:
			responses = self.client.recv(1024)
			if responses.find(";") != -1:
				for r in responses.split(";"):
					if r != "":
						self.SetInDictionary(r)
			
			time.sleep(0.02)
			
			