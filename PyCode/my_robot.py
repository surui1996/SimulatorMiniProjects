from robot_main import SimpleRobot
import time

class MySimpleRobot(SimpleRobot):
    def Autonomous(self):
        i = 0
        p = 5 / 90.0
        error = 0.0
        setpoint = 90
        
        self.TryToPossess()
        self.ResetEncoders()
        self.Wait(0.5)
        for i in range(1):
            self.ArcadeDrive(0.6, 0.0)
            while (self.GetEncoderLeft() + self.GetEncoderLeft()) / 2 < 2.0:
                self.Wait(0.02)
            
            self.ResetGyro()
            #while i < 20:
            #    error = setpoint - self.GetGyroAngle()
            #    self.ArcadeDrive(0.0, -p * error)
            #    if error < 0.1:
            #        i += 1
            #    else:
            #        i = 0
            #    self.Wait(0.02)
                
        self.ArcadeDrive(0.0, 0.0)
        self.Shoot()

    def OperatorControl(self):
        while self.IsOperatorControl():
            self.JoystickTankDrive()
            
            if self.IsXKeyPressed("LB"):
                self.TryToPossess()
            if self.IsXKeyPressed("RB"):
                self.Shoot()
            if self.IsXKeyPressed("X"):
                self.ResetEncoders()
            if self.IsXKeyPressed("Y"):
                self.ResetGyro()    
            #if self.IsBallPossessed():
            #    self.Shoot()
            self.Wait(0.02)

    def Disabled(self):
         self.ArcadeDrive(0.0, 0.0)

if __name__ == "__main__":			
    myRobot = MySimpleRobot()
    myRobot.StartCompetition()
    exit()