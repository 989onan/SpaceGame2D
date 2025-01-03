﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.ThreadSafePhysicsSolver
{
    public interface ICollideable
    {
        AABB Collider { get; }
        void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal);
        bool HasCollision { get; }
    }
}
