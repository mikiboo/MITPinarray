using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PinTableTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var obj = new PinTableTranslator(2, 1, 1, 1);
        print(obj.Translate(@"
    {
        ""pins"": [
            {""id"": ""00.00"", ""position"": 100},
            {""id"": ""00.01"", ""position"": 101},
            {""id"": ""00.02"", ""position"": 102},
            {""id"": ""00.03"", ""position"": 103},
            {""id"": ""00.04"", ""position"": 104},
            {""id"": ""00.05"", ""position"": 105},
            {""id"": ""00.06"", ""position"": 106},
            {""id"": ""00.07"", ""position"": 107},
            {""id"": ""00.08"", ""position"": 108},
            {""id"": ""00.09"", ""position"": 109},
            {""id"": ""01.00"", ""position"": 110},
            {""id"": ""01.01"", ""position"": 111},
            {""id"": ""01.02"", ""position"": 112},
            {""id"": ""01.03"", ""position"": 113},
            {""id"": ""01.04"", ""position"": 114},
            {""id"": ""01.05"", ""position"": 115},
            {""id"": ""01.06"", ""position"": 116},
            {""id"": ""01.07"", ""position"": 117},
            {""id"": ""01.08"", ""position"": 118},
            {""id"": ""01.09"", ""position"": 119},
            {""id"": ""02.00"", ""position"": 100},
            {""id"": ""02.01"", ""position"": 101},
            {""id"": ""02.02"", ""position"": 102},
            {""id"": ""02.03"", ""position"": 103},
            {""id"": ""02.04"", ""position"": 104},
            {""id"": ""02.05"", ""position"": 105},
            {""id"": ""02.06"", ""position"": 106},
            {""id"": ""02.07"", ""position"": 107},
            {""id"": ""02.08"", ""position"": 108},
            {""id"": ""02.09"", ""position"": 109},
            {""id"": ""03.00"", ""position"": 110},
            {""id"": ""03.01"", ""position"": 111},
            {""id"": ""03.02"", ""position"": 112},
            {""id"": ""03.03"", ""position"": 113},
            {""id"": ""03.04"", ""position"": 114},
            {""id"": ""03.05"", ""position"": 115},
            {""id"": ""03.06"", ""position"": 116},
            {""id"": ""03.07"", ""position"": 117},
            {""id"": ""03.08"", ""position"": 118},
            {""id"": ""03.09"", ""position"": 119}
        ]
    }"));
    }

// Update is called once per frame
    void Update()
    {
    }
}
