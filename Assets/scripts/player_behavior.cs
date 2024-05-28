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
  private Tilemap flagMap;
  [SerializeField]
  private Tilemap arrowMap;
  [SerializeField]
  private TileBase zombieTile;
  [SerializeField]
  private TileBase mineTile;
  [SerializeField]
  private TileBase flagTile;
  [SerializeField]
  private TileBase arrowUp;
  [SerializeField]
  private TileBase arrowDown;
  [SerializeField]
  private TileBase arrowLeft;
  [SerializeField]
  private TileBase arrowRight;
  [SerializeField]
  // current player position
  private Vector2Int position;
  [SerializeField]
  // set amount of zombies on board from unity
    private int numZombies;
  // all zombie locations
  private Vector3Int[] zombies;
// first arrow press, next press will do action in this direction 
  private Vector2 currDirection;
  // 1 press = active, 2 is action, 0 is inactive
  private int presses = 0;
  private bool lose = false;
  private bool active = false;


  //methods
  private void Awake() { }
  private void OnDisable() { }
  private void OnEnable() { }

  void Start()
  {
    SpawnZombies();
  }
  // randomly spawns zombies in valid locations throughout the board on game start
  void SpawnZombies()
  {
    zombies = new Vector3Int[numZombies];

    codeMap.CompressBounds();
    var size = codeMap.size;
    for (var i = 0; i < numZombies; i++)
    {
      var random = new Vector3Int(Random.Range(1, size.x) - 7, Random.Range(1, size.y) - 6, 0);
      while (Math.Abs(position.x - random.x) < 6 && Math.Abs(position.y - random.y) < 6)
      {
        random = new Vector3Int(Random.Range(1, size.x) - 7, Random.Range(1, size.y) - 6, 0);
      }
      zombies[i] = random;
      zombieMap.SetTile(random, zombieTile);
    }

  }
  // checks validity of next move on second key press, move helper function
  private bool CanMove(Vector2 direction)
  {
    var position = grassMap.WorldToCell(transform.position + (Vector3)direction);
    if (!codeMap.HasTile(position) || grassMap.HasTile(position) || flagMap.HasTile(position))
    {
      return false;
    }

    return true;
  }

  private void Move(Vector2 direction)
  {
    if (CanMove(direction))
    {
      transform.position += (Vector3)direction;
    }
    // moveZombies();
  }

  // called on second arrow press, checks if arrow key was 2nd press, then moves if next move is valid
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
      arrowMap.ClearAllTiles();
      if (direction == currDirection)
      {
        active = false;

        Move(direction);
        presses = 0;
        setZombies();


      }
      else
      {
        currDirection = direction;
        presses = 1;
        active = true;
      }

    }
  }
  // called on first arrow key press, shows arrow in direction the user pressed
  void SetArrow()
  {
    arrowMap.ClearAllTiles();
    if (active)
    {
      if (codeMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
      {
        if (currDirection.x > 0)
        {
          arrowMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), arrowRight);
        }
        else if (currDirection.x < 0)
        {
          arrowMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), arrowLeft);
        }

        else if (currDirection.y > 0)
        {
          arrowMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), arrowUp);
        }
        else
        {
          arrowMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), arrowDown);
        }
      }
    }

  }


  // called when user clicks V key, checks if this click is the "action click"
  private void tryDig()
  {
    if (active)
    {
      grassMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), null);
      presses = 0;
      active = false;
    }
  }
  // called when F key is pressed after arrow, then places flag in valid area, arrow isn't visible in non valid directions
  private void flag()
  {
    if (active)
    {
      if (codeMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
      {
        if (!flagMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
        {
          if (!zombieMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
          {
            flagMap.SetTile(flagMap.WorldToCell(transform.position + (Vector3)currDirection), flagTile);
          }
        }
        else
        {
          flagMap.SetTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection), null);
        }
      }

      presses = 0;
      active = false;
    }

  }
  private void setZombies()
  {
    // refresh map
    zombieMap.ClearAllTiles();
    // iterate through all current zombie locations
    for (int i = 0; i < zombies.Length; i++)
    {
      // grabs horizontal and vertical distance
      var xDist = (int)(transform.position.x - zombies[i].x);
      var yDist = (int)(transform.position.y - zombies[i].y);

      // checks if zombie[i] is on left or right side of player
      if (xDist > 0)
      {
        // checks if next move is valid (applies to the rest of if statements below), it moves on that axis, else it will remains still on that axis
        var temp = new Vector3Int(zombies[i].x + 1, zombies[i].y, 0);
        if (codeMap.HasTile(codeMap.WorldToCell(temp)) && !zombieMap.HasTile(codeMap.WorldToCell(temp)))
        {
          zombies[i].x += 1;
        }
      }
      else if (xDist < 0)
      {
        var temp = new Vector3Int(zombies[i].x - 1, zombies[i].y, 0);
        if (codeMap.HasTile(codeMap.WorldToCell(temp)) && !zombieMap.HasTile(codeMap.WorldToCell(temp)))
        {
          zombies[i].x -= 1;
        }
      }
      // checks if zombie[i] is above or below player
      if (yDist > 0)
      {
        var temp = new Vector3Int(zombies[i].x, zombies[i].y + 1, 0);
        if (codeMap.HasTile(codeMap.WorldToCell(temp)) && !zombieMap.HasTile(codeMap.WorldToCell(temp)))
        {
          zombies[i].y += 1;
        }
      }
      else if (yDist < 0)
      {
        var temp = new Vector3Int(zombies[i].x, zombies[i].y - 1, 0);
        if (codeMap.HasTile(codeMap.WorldToCell(temp)) && !zombieMap.HasTile(codeMap.WorldToCell(temp)))
        {
          zombies[i].y -= 1;
        }
      }
      // displays new zombie location
      zombieMap.SetTile(codeMap.WorldToCell(zombies[i]), zombieTile);
    }
  }




  // Update is called once per frame
  void Update()
  {

    Controls();
    LoseCheck();
    SetArrow();

  }
  // checks movement/action buttons
  void Controls()
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

  }
  // checks if player is touching a zombie tile or bomb tile
  void LoseCheck()
  {
    if (zombieMap.HasTile(codeMap.WorldToCell(transform.position)))
    {
      lose = true;
      Debug.Log("LOSERFIKHBFS");
    }
    if (codeMap.GetTile(codeMap.WorldToCell(transform.position)).Equals(mineTile))
    {
      lose = true;
    }
  }
}
