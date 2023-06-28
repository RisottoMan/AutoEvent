using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoEvent.Events.Glass.Features
{
    internal class Platformes
    {
        public void CreateCheckPoint()
        {
            /*
            Model checkPoint = new Model("check", GameMap.GameObject.transform.position + new Vector3(0, 35f, 0f));
            checkPoint.AddPart(new ModelPrimitive(checkPoint, PrimitiveType.Cube, new Color32(113, 69, 69, 255), Vector3.zero, Vector3.zero, new Vector3(12, 1, 5)));
            checkPoint.AddPart(new ModelPrimitive(checkPoint, PrimitiveType.Cube, new Color32(113, 69, 69, 255), new Vector3(5.5f, 0.99f, 0.5f), new Vector3(0, 90, 0), new Vector3(4, 1, 1)));
            checkPoint.AddPart(new ModelPrimitive(checkPoint, PrimitiveType.Cube, new Color32(113, 69, 69, 255), new Vector3(-5.5f, 0.99f, 0.5f), new Vector3(0, 90, 0), new Vector3(4, 1, 1)));
            checkPoint.AddPart(new ModelPrimitive(checkPoint, PrimitiveType.Cube, new Color32(113, 69, 69, 255), new Vector3(0f, 0.99f, 2f), new Vector3(0, 0, 0), new Vector3(10.4f, 1, 1)));
            ModelCheckPoint = checkPoint;
            */
        }
        public void CreatePlatformes()
        {
            /*
            Platformes = new Model("platforme", GameMap.GameObject.transform.position);
            int parkourNumber = 1;
            int playerCount = Player.List.ToList().Count(r => r.Role != RoleType.Spectator);

            if (playerCount <= 5) parkourNumber = 1;
            else if (playerCount > 5 && playerCount <= 15) parkourNumber = 2;
            else if (playerCount > 15) parkourNumber = 3;

            Vector3 pos = new Vector3(0f, 35f, -16.23f); // -2.3 2.3
            Vector3 delta = new Vector3(0f, 0f, 4.77f);
            for (int i = 0; i < parkourNumber * 3; i++) // 3 6 9
            {
                var model = new ModelPrimitive(Platformes, PrimitiveType.Cube, new Color32(153, 153, 153, 255), pos + new Vector3(-2.3f, 0, 0), Vector3.zero, new Vector3(3, 0.2f, 3));
                var model1 = new ModelPrimitive(Platformes, PrimitiveType.Cube, new Color32(153, 153, 153, 255), pos + new Vector3(2.3f, 0, 0), Vector3.zero, new Vector3(3, 0.2f, 3));

                if (Random.Range(0, 2) == 0) model.GameObject.AddComponent<GlassComponent>();
                else model1.GameObject.AddComponent<GlassComponent>();

                Platformes.AddPart(model);
                Platformes.AddPart(model1);
                pos += delta;
            }
            ModelCheckPoint.GameObject.transform.position = GameMap.GameObject.transform.position + pos + new Vector3(0, 0, 2);
            */
        }
    }
}
