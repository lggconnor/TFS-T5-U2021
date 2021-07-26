using UnityEngine;

namespace UnityMovementAI
{
    public class WallAvoidanceUnit : MonoBehaviour
    {
        public LinePath path;

        SteeringBehaviour steeringBasics;
        WallAvoidance wallAvoidance;
        FollowPath followPath;

        void Start()
        {
            path.CalcDistances();

            steeringBasics = GetComponent<SteeringBehaviour>();
            wallAvoidance = GetComponent<WallAvoidance>();
            followPath = GetComponent<FollowPath>();
        }

        void FixedUpdate()
        {
            if (followPath.IsAtEndOfPath(path))
            {
                path.ReversePath();
            }

            Vector3 accel = wallAvoidance.GetSteering();

            if (accel.magnitude < 0.005f)
            {
                accel = followPath.GetSteering(path);
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();

            path.Draw();
        }
    }
}