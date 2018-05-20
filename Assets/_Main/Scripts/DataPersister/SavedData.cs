﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData {

    public List<string> stringKeys;
    public List<string> stringData;

    public List<string> intKeys;
    public List<int> intData;

    public List<string> floatKeys;
    public List<float> floatData;

    public List<string> boolKeys;
    public List<bool> boolData;

    public List<string> vectorKeys;
    public List<Vector3> vectorData;

}