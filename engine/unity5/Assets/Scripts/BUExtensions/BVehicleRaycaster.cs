using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletSharp.Math;
using BulletUnity;

namespace Assets.Scripts.BUExtensions
{
    public class BVehicleRaycaster : IVehicleRaycaster
    {
        protected DynamicsWorld dynamicsWorld;

        public BVehicleRaycaster(DynamicsWorld world)
        {
            dynamicsWorld = world;
        }

        public object CastRay(ref Vector3 from, ref Vector3 to, VehicleRaycasterResult result)
        {
            AllHitsRayResultCallback rayCallback = new AllHitsRayResultCallback(from, to);

            dynamicsWorld.RayTest(from, to, rayCallback);

            int i = 0;

            foreach (CollisionObject co in rayCallback.CollisionObjects)
            {
                BRigidBody brb = co.UserObject as BRigidBody;

                if (brb != null)
                {
                    if (!brb.gameObject.name.StartsWith("node_"))
                    {
                        result.HitPointInWorld = rayCallback.HitPointWorld[i];
                        result.HitNormalInWorld = rayCallback.HitNormalWorld[i];
                        result.HitNormalInWorld.Normalize();
                        result.DistFraction = rayCallback.HitFractions[i];

                        return RigidBody.Upcast(co);
                    }
                }

                i++;
            }

            return null;
        }
    }
}
