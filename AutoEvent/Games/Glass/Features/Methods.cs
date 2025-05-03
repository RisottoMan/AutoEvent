using LabApi.Features.Wrappers;
using UnityEngine;

namespace AutoEvent.Games.Glass.Features;
public static class Methods
{
    public static void ChangePosition(GameObject gameObject, Vector3 position)
    {
        if (gameObject.TryGetComponent(out PrimitiveObjectToy primitiveObject))
        {
            primitiveObject.IsStatic = false;
            primitiveObject.Position += position;
        }
    }
}