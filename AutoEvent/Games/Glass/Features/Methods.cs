using UnityEngine;
using MapEditorReborn.API.Features.Objects;

namespace AutoEvent.Games.Glass.Features;
public static class Methods
{
    public static void ChangePosition(GameObject gameObject, Vector3 position)
    {
        if (gameObject.TryGetComponent(out PrimitiveObject primitiveObject))
        {
            primitiveObject.IsStatic = false;
            primitiveObject.Position += position;
        }
    }
}