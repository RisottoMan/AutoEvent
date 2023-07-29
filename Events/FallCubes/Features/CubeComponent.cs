using AutoEvent.Events.FallCubes;
using Exiled.API.Features;
using MEC;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace AutoEvent.Events.FallCubes.Features;

public class FallCubeComponent : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject, 15f);
    }

    public void Update()
    {
        Timing.WaitForSeconds(1f);
        Destroy(gameObject);
    }
}