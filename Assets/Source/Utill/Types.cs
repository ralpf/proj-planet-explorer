using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct MinMax
{
    [SerializeField] float _min;
    [SerializeField] float _max;
    
    public float min { get => _min; set => _min = value; }
    public float max { get => _max; set => _max = value; }
    
    public float m { get => _min; set => _min = value; }
    public float M { get => _max; set => _max = value; }

    public float RollRandom => Random.Range(_min, _max);

    public MinMax(float min, float max)
    {
        _min = min;
        _max = max;
    }
   
}