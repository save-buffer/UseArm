# UseArm
Inverse Kinematics algorithm for Robotic Arm

# Novel Algorithm for Inverse Kinematics
The traditional Inverse Kinematics approach is to use the Jacobian Inverse technique. We find that that is computationally expensive, and instead opt to use gradient descent to converge to a solution. The function D to minimize is the Euclidian distance between the position of the end effector and the target position. 
The partial derivative of D with respect to each angle yields the gradient.
