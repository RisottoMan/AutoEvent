// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         BaseCollisionComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 5:33 PM
//    Created Date:     10/17/2023 5:33 PM
// -----------------------------------------

using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.API.Components;

public class BaseCollisionDetectorComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the raycast distance.
        /// </summary>
        public virtual float RaycastDistance { get; set; } = 1;

        /// <summary>
        /// Gets or sets the time delay (in seconds) before performing raycast collision checks during each update.
        /// </summary>
        public virtual float Delay { get; set; } = 0.1f;

        private float elapsedTime = 0;

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        internal virtual void Update()
        {

            if(elapsedTime > Delay)
            {
                // Raycast upwards
                RaycastHit hitUp;
                if (Physics.Raycast(transform.position, Vector3.up, out hitUp, RaycastDistance))
                {
                    OnCollisionWithSomething(hitUp.collider.transform.root.gameObject);
                }

                // Raycast downwards
                RaycastHit hitDown;
                if (Physics.Raycast(transform.position, Vector3.down, out hitDown, RaycastDistance))
                {

                    OnCollisionWithSomething(hitDown.collider.transform.root.gameObject);
                }

                // Raycast forwards

                RaycastHit hitForward;
                if (Physics.Raycast(transform.position, transform.forward, out hitForward, RaycastDistance))
                {
                    OnCollisionWithSomething(hitForward.collider.transform.root.gameObject);
                }

                // Raycast backwards
                RaycastHit hitBackward;
                if (Physics.Raycast(transform.position, -transform.forward, out hitBackward, RaycastDistance))
                {
                    OnCollisionWithSomething(hitBackward.collider.transform.root.gameObject);
                }

                // Raycast to the left
                RaycastHit hitLeft;
                if (Physics.Raycast(transform.position, -transform.right, out hitLeft, RaycastDistance))
                {
                    OnCollisionWithSomething(hitLeft.collider.transform.root.gameObject);
                }

                // Raycast to the right
                RaycastHit hitRight;
                if (Physics.Raycast(transform.position, transform.right, out hitRight, RaycastDistance))
                {
                    OnCollisionWithSomething(hitRight.collider.transform.root.gameObject);
                }

                elapsedTime = 0;
            }

            elapsedTime += Time.deltaTime;
        }

        /// <summary>
        /// Called when a collision with an object is detected.
        /// </summary>
        /// <param name="gameObject">The GameObject involved in the collision.</param>
        public virtual void OnCollisionWithSomething(GameObject gameObject)
        {
            // This method can be overridden in derived classes to handle collisions.
        }

        public bool IsPlayer(GameObject gameObject, out Player? player)
        {
            if (gameObject == base.gameObject || gameObject.tag != "Player")
            {
                player = null;
                return false;
            }

            if(Player.TryGet(gameObject, out player))
            {
                return true;
            }
            else
            {
                player = null;
                return false;
            }
        }
    }