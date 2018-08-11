using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private float _waterFloodVelocity = 0.05f;

        [SerializeField] private int _mapWidth;
        [SerializeField] private int _mapHeight;

        [SerializeField] private Ground _groundPrefab;
        [SerializeField] private Wall _wallPrefab;
        [SerializeField] private Door _doorPrefab;

        private Ground[,] _grounds;
        private Wall[,] _wallsHorizontal;
        private Wall[,] _wallsVertical;

        // Use this for initialization
        void Start()
        {
            CreateMap();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMap();
        }

        void CreateMap()
        {
            CreateGrounds();
            CreateWalls();
        }

        private void UpdateMap()
        {
            foreach (Ground __ground in _grounds)
            {
                __ground.UpdateFillAmount(_waterFloodVelocity);
            }
        }

        private void ListenGroundEvents(Ground p_ground)
        {
            p_ground.onChangeState += Ground_OnChangeState;
        }

        private void Ground_OnChangeState(object p_source, Ground.OnChangeStateEventArgs p_eventArgs)
        {
            if (p_eventArgs.State == Ground.State.IMMERSE)
            {
                FloodAdjacent(p_source as Ground);
            }
        }
        //horizontal 1.3
        //vertical 0.3 ground 1.3 vertical 1.3
        //horizontal 1.2
        private void FloodAdjacent(Ground p_ground)
        {
            try
            {
                if (p_ground.X > 0)
                {
                    if (_wallsVertical[p_ground.X - 1, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X - 1, p_ground.Y] as Door).CurrentState == Door.State.OPEN)
                    {
                        if (_grounds[p_ground.X - 1, p_ground.Y].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X - 1, p_ground.Y].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.X <= _wallsVertical.GetLength(0) - 1)
                {
                    if (_wallsVertical[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X, p_ground.Y] as Door).CurrentState == Door.State.OPEN)
                    {
                        if (_grounds[p_ground.X + 1, p_ground.Y].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X + 1, p_ground.Y].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.Y > 0)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y - 1].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y - 1] as Door).CurrentState == Door.State.OPEN)
                    {
                        if (_grounds[p_ground.X, p_ground.Y - 1].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X, p_ground.Y - 1].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.Y <= _wallsHorizontal.GetLength(1) - 1)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y] as Door).CurrentState == Door.State.OPEN)
                    {
                        if (_grounds[p_ground.X, p_ground.Y + 1].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X, p_ground.Y + 1].SetFillAmount(0.01f);
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.Log($"x: {p_ground.X} | y: {p_ground.Y}");
                Debug.Log($"Vertical: {_wallsVertical.GetLength(0)} - {_wallsVertical.GetLength(1)}");
                Debug.Log($"Horizontal: {_wallsHorizontal.GetLength(0)} - {_wallsHorizontal.GetLength(1)}");
                Debug.LogError(e);
            }
        }

        private void CreateGrounds()
        {
            _grounds = new Ground[_mapWidth, _mapHeight];

            int __floodStartX, __floodStartY;

            __floodStartX = Random.Range(0, _mapWidth - 1);
            __floodStartY = Random.Range(0, _mapHeight - 1);

            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    _grounds[i, j] = CreateGround(i, j, (__floodStartX == i && __floodStartY == j));
                }
            }
        }

        private Ground CreateGround(int p_x, int p_y, bool p_startWithFlood)
        {
            Vector3 __spawnPosition = new Vector3(
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_x) + (GameSettings.WALL_SIZE * p_x),
                0f,
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_y) + (GameSettings.WALL_SIZE * p_y));

            Ground __ground = Instantiate(_groundPrefab, transform).GetComponent<Ground>();

            ListenGroundEvents(__ground);

            __ground.name = $"Ground [{p_x}][{p_y}]";
            __ground.Initialize(p_x, p_y);
            __ground.SetFillAmount(p_startWithFlood ? 0.01f : 0f);
            __ground.SetPosition(__spawnPosition);


            return __ground;
        }

        private void CreateWalls()
        {
            _wallsHorizontal = new Wall[_mapWidth, _mapHeight - 1];
            _wallsVertical = new Wall[_mapWidth - 1, _mapHeight];

            int __maxDoors = Mathf.RoundToInt(_wallsHorizontal.GetLength(0) * _wallsHorizontal.GetLength(1) * 0.95f);
            int __doorCounter = 0;

            for (int i = 0; i < _wallsHorizontal.GetLength(0); i++)
            {
                for (int j = 0; j < _wallsHorizontal.GetLength(1); j ++)
                {
                    bool __hasDoor = false;

                    if (__doorCounter < __maxDoors && Random.Range(0, 1) == 0)
                    {
                        __doorCounter++;
                        __hasDoor = true;
                    }

                    _wallsHorizontal[i, j] = CreateWall(i, j, true, __hasDoor);
                }
            }

            __maxDoors = Mathf.RoundToInt(_wallsVertical.GetLength(0) * _wallsVertical.GetLength(1) * 0.95f);
             __doorCounter = 0;
            for (int i = 0; i < _wallsVertical.GetLength(0); i++)
            {
                for (int j = 0; j < _wallsVertical.GetLength(1); j++)
                {
                    bool __hasDoor = false;

                    if (__doorCounter < __maxDoors && Random.Range(0, 1) == 0)
                    {
                        __doorCounter++;
                        __hasDoor = true;
                    }

                    _wallsVertical[i, j] = CreateWall(i, j, false, __hasDoor);
                }
            }

        }

        private Wall CreateWall(int p_x, int p_y, bool p_isHorizontal, bool p_isDoor)
        {
            Vector3 __spawnPosition;
            Vector3 __scale;

            if (p_isHorizontal)
            {
                __spawnPosition = new Vector3(
               GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_x) + (GameSettings.WALL_SIZE * p_x),
               2f,
               GameSettings.WALL_SIZE2 + GameSettings.GROUND_SIZE + (GameSettings.GROUND_SIZE * p_y) + (GameSettings.WALL_SIZE * p_y));

                __scale = new Vector3(GameSettings.WALL_WIDTH, 5f, 1f);
            }
            else
            {
                __spawnPosition = new Vector3(
               GameSettings.WALL_SIZE2 + GameSettings.GROUND_SIZE + (GameSettings.GROUND_SIZE * p_x) + (GameSettings.WALL_SIZE * p_x),
               2f,
               GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_y) + (GameSettings.WALL_SIZE * p_y));

                __scale = new Vector3(1f, 5f, GameSettings.WALL_WIDTH);
            }

            Wall __wall = Instantiate(p_isDoor ? _doorPrefab : _wallPrefab, transform).GetComponent<Wall>();
            __wall.name = $"Wall {(p_isHorizontal?"Horizontal":"Vertical")} [{p_x}][{p_y}]";
            __wall.Initialize(p_x, p_y);
            __wall.SetLocalScale(__scale);
            __wall.SetPosition(__spawnPosition);

            return __wall;
        }
    }
}

