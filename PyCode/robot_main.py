from robot_client import RobotClient
import time

class RobotBase:
    def __init__(self):
        self.client = RobotClient()
        self.client.start()

    def IsEnabled(self):
        return self.IsAutonomous() or self.IsOperatorControl()

    def IsDisabled(self):
        return not self.IsEnabled()

    def IsAutonomous(self):
        return self.client.GameState == "AUTO"

    def IsOperatorControl(self):
        return self.client.GameState == "TELEOP"
	
    def Wait(self, secs):
        time.sleep(secs)
    
    def GetGyroAngle(self):
        return self.client.robotValues["Gyro"]
    
    def ResetGyro(self):
        return self.client.SendResetMessage("GYRO")
	
    def ResetEncoders(self):
        return self.client.SendResetMessage("ENCODERS")
    
    def GetEncoderLeft(self):
        return self.client.robotValues["EncoderLeft"]
		
    def GetEncoderRight(self):
        return self.client.robotValues["EncoderRight"]
	
    def ArcadeDrive(self, forward, curve):
        self.client.SendArcadeDriveRequest(forward, curve)
		
    def TankDrive(self, left, right):
        self.client.SendTankDriveRequest(left, right)	
	
    def IsKeyPressed(self, keyString):
        return self.client.IsPressed(keyString)
    
    def TryToPossess(self):
        self.client.TryToPossess()
    
    def IsBallPossessed(self):
        return self.client.IsBallPossessed()
    
    def Shoot(self):
        self.client.Shoot()
		

class SimpleRobot(RobotBase):
    """The SimpleRobot class is intended to be subclassed by a user creating
    a robot program. Overridden Autonomous() and OperatorControl() methods
    are called at the appropriate time as the match proceeds. In the
    current implementation, the Autonomous code will run to completion
    before the OperatorControl code could start."""

    def __init__(self):
        RobotBase.__init__(self)
    
    def Autonomous(self):
        """Autonomous should go here.
        Programmers should override this method to run code that should run
        while the field is in the autonomous period. This will be called
        once each time the robot enters the autonomous state."""
        pass

    def OperatorControl(self):
        """Operator control (tele-operated) code should go here.
        Programmers should override this method to run code that should run
        while the field is in the Operator Control (tele-operated) period.
        This is called once each time the robot enters the teleop state."""
        pass
        
    def Disabled(self):
        """Disabled should go here.
        Programmers should override this method to run code that should run
        while the field is disabled."""
        pass
    

    def StartCompetition(self):
        """Start a competition.
        This code needs to track the order of the field starting to ensure
        that everything happens in the right order. Repeatedly run the correct
        method, either Autonomous or OperatorControl when the robot is
        enabled. After running the correct method, wait for some state to
        change, either the other mode starts or the robot is disabled. Then go
        back and wait for the robot to be enabled again."""
		
        #self.RobotInit()
        while not self.client.IsUp():
            time.sleep(0.005)
			
        while True:
            if self.IsDisabled():
                self.Disabled()
                while self.IsDisabled():
                    self.Wait(0.005)
            elif self.IsAutonomous():
                self.Autonomous()
                while self.IsAutonomous() and self.IsEnabled():
                    self.Wait(0.005)
            elif self.IsOperatorControl():
                self.OperatorControl()
                while self.IsOperatorControl() and self.IsEnabled():
                    self.Wait(0.005)
