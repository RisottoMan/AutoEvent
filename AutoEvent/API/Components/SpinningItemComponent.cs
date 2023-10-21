// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PowerupItem.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 12:40 AM
//    Created Date:     10/17/2023 12:40 AM
// -----------------------------------------

using UnityEngine;

namespace AutoEvent.API.Components;
    using UnityEngine;
    
    /// <summary>
    /// Handles rotating a pickup indicator.
    /// </summary>
    public class SpinningItemComponent : MonoBehaviour
    {
        /// <summary>
        /// The spinning speed.
        /// </summary>
        public float Speed = 100f;

        /// <inheritdoc/>
        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * Speed);
        }
    }