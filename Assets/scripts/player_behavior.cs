using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using playerMove;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEditor;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using System;
using UnityEngine.UIElements;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class player_behavior : MonoBehaviour
{
  [SerializeField]
  private Tilemap grassMap;
  [SerializeField]
  private Tilemap zombieMap;
  [SerializeField]
  private Tilemap codeMap;
  [SerializeField]
  private TileBase zombieTile;
  [SerializeField]
  private TileBase mineTile;
  [SerializeField]
  private PlayerMovement controls;
  [SerializeField]
  private Rigidbody2D player;
  [SerializeField]
  private Tilemap flagMap;
  [SerializeField]
  private TileBase flagTile;
  [SerializeField]
  private Vector2Int position;

  private Vector2 currDirection;
  private int presses = 0;

  private bool lose = false;
  private bool active = false;

  private void Awake()
  {
    // controls = new PlayerMovement();
  }

  private void OnDisable()
  {
    // controls.Disable();
  }
  private void OnEnable()
  {
    // controls.Enable();
  }
  // Start is called before the first frame update
  void Start()
  {
    // controls.main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());


    // this is whats next thursday

    // controls.main.Actions.performed += ctx => ;
    // player.MovePosition(position);
    // compress bounds for num rows and num cols, save value
    // adjustable amount of zombies for difficulty
    // create vector3 for cell location of zombie spawn, then set tile to zombie
    // be mindful of offset, fix in unity to center first (unless i can fix that problem here)
    // check if zombie is too close, while loop until fixed
    grassMap.CompressBounds();
    var size = grassMap.size;
    var numZombies = 5;
    for (var i = 0; i < numZombies; i++)
    {

      var random = new Vector3Int(Random.Range(1, size.x) - 7, Random.Range(1, size.y) - 6, 0);
      while (Math.Abs(position.x - random.x) < 4 && Math.Abs(position.y - random.y) < 4)
      {
        random = new Vector3Int(Random.Range(1, size.x) - 7, Random.Range(1, size.y) - 6, 0);
      }
      zombieMap.SetTile(random, zombieTile);
    }
  }

  private void Move(Vector2 direction)
  {
    if (CanMove(direction))
    {
      transform.position += (Vector3)direction;
    }
  }

  private bool CanMove(Vector2 direction)
  {
    var position = grassMap.WorldToCell(transform.position + (Vector3)direction);
    if (!codeMap.HasTile(position) || grassMap.HasTile(position) || flagMap.HasTile(position))
    {
      return false;
    }

    return true;
  }
  private void tryMove(Vector2Int direction)
  {
    presses += 1;
    if (presses == 1)
    {
      currDirection = direction;
      active = true;
    }
    if (presses == 2)
    {
      if (direction == currDirection)
      {
        Move(direction);
        presses = 0;
        active = false;
      }
      else
      {
        currDirection = direction;
        presses = 1;
        active = true;
      }
    }
  }

  private void tryDig()
  {
    if (active)
    {
        grassMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), null);
      presses = 0;
      active = false;
    }
  }
  private void flag()
  {
    if (active)
    {
      if (!flagMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
      {
        flagMap.SetTile(flagMap.WorldToCell(transform.position + (Vector3)currDirection), flagTile);
      }
      else
      {
        flagMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), null);
      }
      presses = 0;
      active = false;
    }

  }
  // Update is called once per frame
  void Update()
  {
    if (Input.GetButtonUp("Horizontal"))
    {
      if (Input.GetAxis("Horizontal") > 0)
      {
        tryMove(new Vector2Int(1, 0));
      }
      else
      {
        tryMove(new Vector2Int(-1, 0));
      }
    }
    else if (Input.GetButtonUp("Vertical"))
    {
      if (Input.GetAxis("Vertical") < 0)
      {
        tryMove(new Vector2Int(0, -1));
      }
      else
      {
        tryMove(new Vector2Int(0, 1));
      }
    }
    else if (Input.GetButtonUp("V"))
    {
      tryDig();
    }
    else if (Input.GetButtonUp("F"))
    {
      flag();
    }



    // if( Input.GetKeyUp("f")){
    //   currKey = "f";
    //   // if( grassMap.HasTile()){}
    // }
    // else if( Input.GetKeyUp(KeyCode.UpArrow)){
    //   currKey = "u";
    // }
    // else if( Input.GetKeyUp(KeyCode.LeftArrow)){
    //   currKey = "l";
    // }
    // else if( Input.GetKeyUp(KeyCode.DownArrow)){
    //   currKey = "d";
    // }
    // else if( Input.GetKeyUp(KeyCode.RightArrow)){
    //   currKey = "r";
    // }
    // Debug.Log(currKey);
    if (zombieMap.HasTile(codeMap.WorldToCell(transform.position)))
    {
      lose = true;
    }
    if (codeMap.GetTile(codeMap.WorldToCell(transform.position)).Equals(mineTile))
    {
      lose = true;
    }


  }
}
