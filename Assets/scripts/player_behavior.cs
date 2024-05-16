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
  private Vector2Int position;
  [SerializeField]
  private int numZombies;

  [SerializeField]  private Vector2 currDirection;
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
  void SpawnZombies()
  {
    grassMap.CompressBounds();
    var size = grassMap.size;
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
      arrowMap.ClearAllTiles();
      if (direction == currDirection)
      {
        active = false;

        Move(direction);
        presses = 0;


      }
      else
      {
        currDirection = direction;
        presses = 1;
        active = true;
      }

    }
  }

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
        if (!zombieMap.HasTile(codeMap.WorldToCell(transform.position + (Vector3)currDirection)))
        {
          flagMap.SetTile(flagMap.WorldToCell(transform.position + (Vector3)currDirection), flagTile);
        }
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
    Controls();
    LoseCheck();
    SetArrow();

  }
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
  void LoseCheck()
  {
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
