using Leap;
using Leap.Unity;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MovePlayerWithLeapMotion : HandDataStructure
{
    public FirstPersonController firstPersonController;
    public LeapServiceProvider leapServiceController;

    void FixedUpdate()
    {
        MoveAndRotate();
    }
    void MoveAndRotate()
    {
        Hand hand = new Hand { Id = -1 };
        if (leapServiceController.GetLeapController() == null) return;
        foreach (var item in leapServiceController.GetLeapController().Frame(0).Hands) // Get last frame
        {
            if (item.IsRight) hand = item;
        }
        if (hand.Id == -1) return;

        float yaw = hand.Direction.Yaw;
        float pitch = hand.Direction.Pitch;
        float roll = hand.PalmNormal.Roll;
        //Debug.Log(string.Format("yaw:{0} pitch:{1} roll:{2}", yaw, pitch, roll));

        float speedX;
        if (pitch > 1.6f) speedX = 0.05f; // avance        
        else if (pitch < 0.6f) speedX = -0.05f; // recule
        else speedX = 0;

        float speedY;
        if (roll < 2 && roll > 0) speedY = 0.05f;
        else if (roll < 0) speedY = -0.05f;
        else speedY = 0;

        if (speedX != 0) speedY = 0; // block axis to move only one axe at the same time

        float rotationY;
        if (yaw > 1) rotationY = -0.8f;
        else if (yaw < 0) rotationY = 0.8f;
        else rotationY = 0;

        if (speedX > 0) rotationY = 0;  // block rotation when going forward
        if (rotationY != 0) speedY = 0; // block move left right when rotate

        firstPersonController.Move(speedY, speedX);
        firstPersonController.RotateView(rotationY, 0);
    }
}
