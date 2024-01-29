using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class RunningState : IBossState
{
    private GameObject _target;
    private GameObject _santaObject;
    private List<GameObject> _pointList;
    private GameObject _giftObject;
    private float speed = 0.5f;
    public string Name { get; } = "Running";
    public string Description { get; } = "The state in which Santa Claus runs around the arena around the center";
    public int Stage { get; } = 0;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 15);

    public void Init(Plugin plugin)
    {
        _santaObject = plugin.santaObject;
        _pointList = plugin.MapInfo.Map.AttachedBlocks.Where(r => r.name == "Running_Point").ToList();
        _giftObject = plugin.MapInfo.Map.AttachedBlocks.Where(r => r.name == "Gift").FirstOrDefault();
    }

    public void Update()
    {
        if (_target is null)
        {
            _target = _pointList.RandomItem();
            DebugLogger.LogDebug($"target position = {_target.transform.position}");
        }

        Vector3 direction = _target.transform.position - _santaObject.transform.position;
        Vector3 velocity = new Vector3(direction.x, 0, direction.z).normalized * speed;

        if (Vector3.Distance(_target.transform.position, _santaObject.transform.position) > 0.5f)
        {
            DebugLogger.LogDebug(Vector3.Distance(_target.transform.position, _santaObject.transform.position).ToString());
            _santaObject.transform.position += new Vector3(velocity.x, 0, velocity.y);
        }
        else
        {
            GameObject newGift = GameObject.Instantiate(_giftObject);
            newGift.transform.position = _target.transform.position;
            newGift.transform.rotation = Quaternion.identity;
            newGift.transform.localScale = Vector3.one * 2;

            _pointList.Remove(_target);
            _target = null;
        }
    }

    public void Deinit()
    {

    }
}
