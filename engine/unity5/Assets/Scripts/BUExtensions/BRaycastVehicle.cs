using BulletSharp;
using BulletUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BUExtensions
{
    public class BRaycastVehicle : MonoBehaviour
    {
        private const float SuspensionToleranceCm = 2.5f;
        private const float RollingFriction = 0.0025f;

        private VehicleTuning vehicleTuning;

        /// <summary>
        /// The Bullet Physics RaycastVehicle associated with the BRaycastVehicle.
        /// </summary>
        public RaycastVehicle RaycastVehicle { get; private set; }

        /// <summary>
        /// Adds a wheel to the BRaycastVehicle from the given information.
        /// </summary>
        /// <param name="connectionPoint"></param>
        /// <param name="axle"></param>
        /// <param name="suspensionRestLength"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public int AddWheel(BulletSharp.Math.Vector3 connectionPoint, BulletSharp.Math.Vector3 axle, float suspensionRestLength, float radius)
        {
            WheelInfo w = RaycastVehicle.AddWheel(connectionPoint,
                -BulletSharp.Math.Vector3.UnitY, axle, suspensionRestLength,
                radius, vehicleTuning, false);

            w.RollInfluence = 0.5f;
            w.Brake = RollingFriction * radius;

            return RaycastVehicle.NumWheels - 1;     
        }

        /// <summary>
        /// Initializes the BRaycastVehicle.
        /// </summary>
        private void Awake()
        {
            ((DynamicsWorld)BPhysicsWorld.Get().world).SetInternalTickCallback(UpdateVehicle);

            BRigidBody rigidBody = GetComponent<BRigidBody>();

            if (rigidBody == null)
            {
                Destroy(this);
                return;
            }

            RaycastVehicle = new RaycastVehicle(vehicleTuning = new VehicleTuning
            {
                MaxSuspensionForce = 100000f,
                MaxSuspensionTravelCm = SuspensionToleranceCm,
                SuspensionDamping = 5f,
                SuspensionCompression = 10f,
                SuspensionStiffness = 300f,
                FrictionSlip = 2f
            },
            (RigidBody)rigidBody.GetCollisionObject(),
            new BVehicleRaycaster((DynamicsWorld)BPhysicsWorld.Get().world));
            
            RaycastVehicle.SetCoordinateSystem(0, 1, 2);
        }
        
        /// <summary>
        /// Updates each wheel's position for proper interpolation.
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < RaycastVehicle.NumWheels; i++)
                RaycastVehicle.UpdateWheelTransform(i, true);
        }

        /// <summary>
        /// Updates the vehicle (synced with the internal physics tick).
        /// </summary>
        /// <param name="world"></param>
        /// <param name="timeStep"></param>
        private void UpdateVehicle(DynamicsWorld world, float timeStep)
        {
            RaycastVehicle.UpdateVehicle(timeStep);
        }
    }
}
