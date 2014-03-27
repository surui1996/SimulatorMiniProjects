import socket
import time
from robot_server import RobotServer

server = RobotServer()
server.start()
c = 0.0
while True:
	if server.IsUp():
		server.SendGetRequest("Gyro")
		server.SendSetRequest("Gyro", c)
		c += 0.001
	time.sleep(1)