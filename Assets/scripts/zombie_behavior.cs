using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class zombie_behavior : MonoBehaviour
{
    // Start is called before the first frame update

   
    [SerializeField]
    private Tilemap codeMap;
    [SerializeField]
    private Tilemap grassMap;
     [SerializeField]
     private Tilemap zombieMap;
     [SerializeField]

     private TileBase zombieTile;
    [SerializeField]
    private Rigidbody2D player;


    [SerializeField]

    // testing for Git Updates
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(player_behavior.position);
        
    }
}
