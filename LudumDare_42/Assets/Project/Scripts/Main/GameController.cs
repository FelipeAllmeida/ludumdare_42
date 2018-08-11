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

        private void FloodAdjacent(Ground p_ground)
        {
            if (p_ground.X > 0)
            {
                if (_grounds[p_ground.X - 1, p_ground.Y].CurrentState == Ground.State.DRY)
                    _grounds[p_ground.X - 1, p_ground.Y].SetFillAmount(0.01f);
            }

            if (p_ground.X < _mapWidth - 1)
            {
                if (_grounds[p_ground.X + 1, p_ground.Y].CurrentState == Ground.State.DRY)
                    _grounds[p_ground.X + 1, p_ground.Y].SetFillAmount(0.01f);
            }

            if (p_ground.Y > 0)
            {
                if (_grounds[p_ground.X, p_ground.Y - 1].CurrentState == Ground.State.DRY)
                    _grounds[p_ground.X, p_ground.Y - 1].SetFillAmount(0.01f);
            }

            if (p_ground.Y < _mapHeight - 1)
            {
                if (_grounds[p_ground.X, p_ground.Y + 1].CurrentState == Ground.State.DRY)
                    _grounds[p_ground.X, p_ground.Y + 1].SetFillAmount(0.01f);
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

            Ground __ground = Instantiate(_groundPrefab, __spawnPosition, new Quaternion(0f, 0f, 0f, 1f), transform).GetComponent<Ground>();

            ListenGroundEvents(__ground);

            __ground.name = $"Ground [{p_x}][{p_y}]";
            __ground.Initialize(p_x, p_y);
            __ground.SetFillAmount(p_startWithFlood ? 0.01f : 0f);


            return __ground;
        }

        private void CreateWalls()
        {
            _wallsHorizontal = new Wall[_mapWidth, _mapHeight - 1];
            _wallsVertical = new Wall[_mapWidth - 1, _mapHeight];


            for (int i = 0; i < _wallsHorizontal.GetLength(0); i++)
            {
                for (int j = 0; j < _wallsHorizontal.GetLength(1); j ++)
                {
                    _wallsHorizontal[i, j] = CreateWall(i, j, true);
                }
            }

            for (int i = 0; i < _wallsVertical.GetLength(0); i++)
            {
                for (int j = 0; j < _wallsVertical.GetLength(1); j++)
                {
                    _wallsVertical[i, j] = CreateWall(i, j, false);
                }
            }

        }

        private Wall CreateWall(int p_x, int p_y, bool p_isHorizontal)
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

            Wall __wall = Instantiate(_wallPrefab, __spawnPosition, new Quaternion(0f, 0f, 0f, 1f), transform).GetComponent<Wall>();
            __wall.name = $"Wall {(p_isHorizontal?"Horizontal":"Vertical")} [{p_x}][{p_y}]";
            __wall.Initialize(p_x, p_y);
            __wall.transform.localScale = __scale;

            return __wall;
        }
    }
}

