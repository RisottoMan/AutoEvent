using System;
using UnityEngine;

namespace AutoEvent.Games.Boss;

public class WaitingState : IBossState
{
    private GameObject _target;
    private GameObject _santaObject;
    private float speed = 0.5f;
    public string Name { get; } = "WaitingState";
    public string Description { get; } = "The transitional state between all states";
    public int Stage { get; } = 0;
    public Animator Animation { get; set; }
    public TimeSpan Timer { get; set; } = new TimeSpan(0, 0, 15);

    public void Init(Plugin plugin)
    {
        _target = new GameObject();
        _target.transform.position = plugin.MapInfo.Map.Position;
        _santaObject = plugin.santaObject;

        //Animation = _santaObject.GetComponent<Animator>();
        //Animation.Play("Left");
    }

    public void Update()
    {
        Vector3 direction = _target.transform.position - _santaObject.transform.position;
        Vector3 velocity = direction.normalized * speed;

        if (Vector3.Distance(_target.transform.position, _santaObject.transform.position) > 0.5f)
        {
            DebugLogger.LogDebug(Vector3.Distance(_target.transform.position, _santaObject.transform.position).ToString());
            _santaObject.transform.position += new Vector3(velocity.x, 0, velocity.y) * speed;
        }
    }

    public void Deinit()
    {

    }
}
