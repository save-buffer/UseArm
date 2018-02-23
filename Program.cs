using System;

namespace UseArm
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ArmMotorController a = new ArmMotorController();
            a.MoveMotors();
            /*var Vis = new ArmVisualizer(a);
            Vis.Show();
            Application.Run(Vis);*/
        }
    }
}
