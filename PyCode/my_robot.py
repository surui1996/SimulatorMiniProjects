from robot_main import SimpleRobot
import time

class MySimpleRobot(SimpleRobot):
    def Autonomous(self):
        i = 0
        p = 5 / 90.0
        error = 0.0
        setpoint = 90

        for i in range(2):
            self.ResetEncoders()
            #self.Wait(0.1)
            self.ArcadeDrive(0.6, 0.0)
            while (self.GetEncoderLeft() + self.GetEncoderLeft()) / 2 < 2.0:
                self.Wait(0.005)
            
            self.ResetGyro()
            while i < 20:
                error = setpoint - self.GetGyroAngle()
                self.ArcadeDrive(0.0, p * error)
                if error < 0.1:
                    i += 1
                else:
                    i = 0
                self.Wait(0.005)
                
        self.ArcadeDrive(0.0, 0.0)

    def OperatorControl(self):
        while self.IsOperatorControl():
            if self.IsKeyPressed("Z"):
                self.ArcadeDrive(0.8, 0.0)
            else:
                self.ArcadeDrive(0.2, 0.0)
            self.Wait(0.005)

    def Disabled(self):
         self.ArcadeDrive(0.0, 0.0)

if __name__ == "__main__":			
    myRobot = MySimpleRobot()
    myRobot.StartCompetition()
    exit()