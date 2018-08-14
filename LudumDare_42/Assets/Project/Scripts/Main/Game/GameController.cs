using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Game.Itens;
using Internal.Audio;
using System.Diagnostics;

namespace Main.Game
{
    [Serializable]
    public struct PrefabData
    {
        [Range(0f, 1f)]
        public float spawnPercentage;
        public GameObject spawnObject;
    }

    public class GameController : EnvironmentController
    {
        [Header("Game Configuration")]
        [Range(0f, 1f)]
        [SerializeField] private float _startEnergy = 0.10f;

        [Range(0f, 1f)]
        [SerializeField] private float _startOxygen = 1f;
        [SerializeField] private float _oxygenGainSpeed = 0f;

        [SerializeField] private int _startMinutesLeft = 5;
        [SerializeField] private int _startSecondsLeft = 0;

        [SerializeField] private int _mapWidth;
        [SerializeField] private int _mapHeight;

        [SerializeField] private int _floodSourceX;
        [SerializeField] private int _floodSourceY;

        [Header("Prefabs")]
        [SerializeField] private WallDoor _doorPrefab;
        [SerializeField] private Player _playerPrefab;

        [SerializeField] private List<PrefabData> _listGroundPrefabs;

        private Ground[,] _grounds;
        private Player _player;
        private Wall[,] _wallsHorizontal;
        private Wall[,] _wallsVertical;

        private Stopwatch _timeCounter;
        private TimeSpan _timeLimit;

        private bool _isStopped = false;

        // Use this for initialization
        public override void IntializeController()
        {
            base.IntializeController();

            _timeCounter = new Stopwatch();
            _timeLimit = new TimeSpan(0, _startMinutesLeft, _startSecondsLeft);

            GameScore.AddEnergy(_startEnergy);
            GameScore.AddOxygen(_startOxygen);

            CreateMap();
            CreatePlayer();
        }

        public override void EnableController()
        {
            base.EnableController();
            _isStopped = false;
            _timeCounter.Start();
            AudioController.Instance.Play(Tags.Background_Game);
            AudioController.Instance.Play(Tags.Ambiente_WaterFlooding);
        }

        public override void DisableController()
        {
            AudioController.Instance.Stop(Tags.Background_Game);
            AudioController.Instance.Stop(Tags.Ambiente_WaterFlooding);
            
            if (_grounds != null)
            {
                foreach(MapObject mapObject in _grounds)
                {
                    mapObject.Clear();
                }
            }

            if (_wallsHorizontal != null)
            {
                foreach(MapObject mapObject in _wallsHorizontal)
                {
                    mapObject.Clear();
                }
            }

            if (_wallsVertical != null)
            {
                foreach (MapObject mapObject in _wallsVertical)
                {
                    mapObject.Clear();
                }
            }   

            for (int i = 0; i < transform.childCount; i ++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            base.DisableController();
        }

        // Update is called once per frame
        public override void UpdateController()
        {
            base.UpdateController();

            if (!_isStopped)
            {
                UpdateMap();
                UpdatePlayer();
            }
        }

        private void UpdateMap()
        {
            for (int i = 0; i < _grounds.GetLength(0); i++)
            {
                for (int j = 0; j < _grounds.GetLength(1); j++)
                {
                    Ground __ground = _grounds[i, j];

                    switch(__ground.CurrentState)
                    {
                        case Ground.State.IMMERSE:
                            break;
                        case Ground.State.FLOOD_SOURCE:
                            __ground.UpdateFillAmount();
                            break;
                        default:
                            if (HaveWaterComingFromAdjacentRooms(__ground))
                                __ground.UpdateFillAmount();
                            break;
                    }

                    if (__ground.CurrentState == Ground.State.FLOOD_SOURCE || HaveWaterComingFromAdjacentRooms(__ground))
                        __ground.UpdateFillAmount();
                }
            }
        }

        private void Resume()
        {
            if (_grounds != null)
            {
                foreach (var ground in _grounds)
                {
                    ground.Resume();
                }
            }

            _timeCounter.Start();
            _isStopped = false;
            _player.EnableInputs(false);
            _player.ResumeCurrentCommand();
        }

        private void Pause()
        {
            if (_grounds != null)
            {
                foreach(var ground in _grounds)
                {
                    ground.Pause();
                }
            }

            _timeCounter.Stop();
            _isStopped = true;
            _player.EnableInputs(false);
            _player.PauseCurrentCommand();
        }

        private void UpdateOxygen()
        {
            if (_player.IsImmerse)
            {
                if (GameScore.CurrentOxygen > 0)
                {
                    GameScore.AddOxygen(- _oxygenGainSpeed * Time.deltaTime);

                    if (GameScore.CurrentOxygen <= 0)
                    {
                        Pause();
                        _player.UI.EnableGameOverPanel(UI.GameOverType.Lost);
                    }
                }
            }
            else
            {
                if (GameScore.CurrentOxygen < 1)
                {
                    GameScore.AddOxygen((_oxygenGainSpeed / 4f) * Time.deltaTime);
                }
            }
        }

        private void UpdateTimeCounter()
        {
            if (_timeLimit.Ticks <= _timeCounter.ElapsedTicks)
            {
                Pause();
                _player.UI.EnableGameOverPanel(UI.GameOverType.Victory);
            }
        }

        private void UpdatePlayer()
        {
            UpdateOxygen();
            UpdateTimeCounter();

            _player?.UpdatePlayer(GameScore.CurrentOxygen, GameScore.CurrentEnergy, _timeLimit.Subtract(_timeCounter.Elapsed));
        }

        void CreateMap()
        {
            CreateGrounds();
            CreateWalls();
        }

        private void ListenGroundEvents(Ground p_ground)
        {
            p_ground.onChangeState += Ground_OnChangeState;
            p_ground.onMaxPressure += Ground_OnMaxPressure;
        }

        private void Ground_OnChangeState(object p_source, Ground.OnChangeStateEventArgs p_eventArgs)
        {
            if (p_eventArgs.State == Ground.State.IMMERSE)
            {
                FloodAdjacent(p_source as Ground);
            }
        }

        private void Ground_OnMaxPressure(object p_source, EventArgs p_args)
        {
            ForceFloodAdjacent(p_source as Ground);
        }

        private void ListenDoorEvents(WallDoor p_door)
        {
            p_door.onChangeState += Door_OnChangeState;
        }

        private void Door_OnChangeState(object p_source, WallDoor.OnChangeStateEventArgs p_eventArgs)
        {
            if (p_eventArgs.State == ItemState.Enabled)
            {
                FloodAdjacent(p_source as Wall);
            }
        }

        public bool HaveWaterComingFromAdjacentRooms(int x, int y)
        {
            return HaveWaterComingFromAdjacentRooms(_grounds[x, y]);
        }

        private bool HaveWaterComingFromAdjacentRooms(Ground p_ground)
        {
            if (p_ground.X > 0)
            {
                if (_wallsVertical[p_ground.X - 1, p_ground.Y].Type == Wall.WallType.DOOR
                    && (_wallsVertical[p_ground.X - 1, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                {
                    if (_grounds[p_ground.X - 1, p_ground.Y].CurrentState != Ground.State.DRY)
                        return true;
                }
            }

            if (p_ground.X <= _wallsVertical.GetLength(0) - 1)
            {
                if (_wallsVertical[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                    && (_wallsVertical[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                {
                    if (_grounds[p_ground.X + 1, p_ground.Y].CurrentState != Ground.State.DRY)
                        return true;
                }
            }

            if (p_ground.Y > 0)
            {
                if (_wallsHorizontal[p_ground.X, p_ground.Y - 1].Type == Wall.WallType.DOOR
                    && (_wallsHorizontal[p_ground.X, p_ground.Y - 1] as WallDoor).CurrentState == ItemState.Enabled)
                {
                    if (_grounds[p_ground.X, p_ground.Y - 1].CurrentState != Ground.State.DRY)
                        return true;
                }
            }

            if (p_ground.Y <= _wallsHorizontal.GetLength(1) - 1)
            {
                if (_wallsHorizontal[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                    && (_wallsHorizontal[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                {
                    if (_grounds[p_ground.X, p_ground.Y + 1].CurrentState != Ground.State.DRY)
                        return true;
                }
            }

            return false;
        }

        private void ForceFloodAdjacent(Ground p_ground)
        {
            try
            {
                if (p_ground.X > 0)
                {
                    if (_wallsVertical[p_ground.X - 1, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X - 1, p_ground.Y] as WallDoor).CurrentState == ItemState.Disabled)
                    {
                        _wallsVertical[p_ground.X - 1, p_ground.Y].ForceInteract();
                    }
                }

                if (p_ground.X <= _wallsVertical.GetLength(0) - 1)
                {
                    if (_wallsVertical[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Disabled)
                    {
                        _wallsVertical[p_ground.X, p_ground.Y].ForceInteract();
                    }
                }

                if (p_ground.Y > 0)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y - 1].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y - 1] as WallDoor).CurrentState == ItemState.Disabled)
                    {
                        _wallsHorizontal[p_ground.X, p_ground.Y - 1].ForceInteract();
                    }
                }

                if (p_ground.Y <= _wallsHorizontal.GetLength(1) - 1)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Disabled)
                    {
                        _wallsHorizontal[p_ground.X, p_ground.Y].ForceInteract();
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log($"x: {p_ground.X} | y: {p_ground.Y}");
                UnityEngine.Debug.Log($"Vertical: {_wallsVertical.GetLength(0)} - {_wallsVertical.GetLength(1)}");
                UnityEngine.Debug.Log($"Horizontal: {_wallsHorizontal.GetLength(0)} - {_wallsHorizontal.GetLength(1)}");
                UnityEngine.Debug.LogError(e);
            }
        }

        private void FloodAdjacent(Ground p_ground)
        {
            try
            {
                if (p_ground.X > 0)
                {
                    if (_wallsVertical[p_ground.X - 1, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X - 1, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                    {
                        if (_grounds[p_ground.X - 1, p_ground.Y].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X - 1, p_ground.Y].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.X <= _wallsVertical.GetLength(0) - 1)
                {
                    if (_wallsVertical[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsVertical[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                    {
                        if (_grounds[p_ground.X + 1, p_ground.Y].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X + 1, p_ground.Y].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.Y > 0)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y - 1].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y - 1] as WallDoor).CurrentState == ItemState.Enabled)
                    {
                        if (_grounds[p_ground.X, p_ground.Y - 1].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X, p_ground.Y - 1].SetFillAmount(0.01f);
                    }
                }

                if (p_ground.Y <= _wallsHorizontal.GetLength(1) - 1)
                {
                    if (_wallsHorizontal[p_ground.X, p_ground.Y].Type == Wall.WallType.DOOR
                        && (_wallsHorizontal[p_ground.X, p_ground.Y] as WallDoor).CurrentState == ItemState.Enabled)
                    {
                        if (_grounds[p_ground.X, p_ground.Y + 1].CurrentState == Ground.State.DRY)
                            _grounds[p_ground.X, p_ground.Y + 1].SetFillAmount(0.01f);
                    }
                }
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.Log($"x: {p_ground.X} | y: {p_ground.Y}");
                UnityEngine.Debug.Log($"Vertical: {_wallsVertical.GetLength(0)} - {_wallsVertical.GetLength(1)}");
                UnityEngine.Debug.Log($"Horizontal: {_wallsHorizontal.GetLength(0)} - {_wallsHorizontal.GetLength(1)}");
                UnityEngine.Debug.LogError(e);
            }
        }

        private void FloodAdjacent(Wall p_wall)
        {
            if (p_wall.IsHorizontal)
            {
                if (p_wall.Y >= 0 && _grounds[p_wall.X, p_wall.Y].CurrentState == Ground.State.IMMERSE)
                {
                    if (p_wall.Y < _grounds.GetLength(1) - 1 && _grounds[p_wall.X, p_wall.Y + 1].CurrentState == Ground.State.DRY)
                        _grounds[p_wall.X, p_wall.Y + 1].SetFillAmount(0.01f);
                }

                if (p_wall.Y < _grounds.GetLength(1) - 1 && _grounds[p_wall.X, p_wall.Y + 1].CurrentState == Ground.State.IMMERSE)
                {
                    if (p_wall.Y >= 0 && _grounds[p_wall.X, p_wall.Y].CurrentState == Ground.State.DRY)
                        _grounds[p_wall.X, p_wall.Y].SetFillAmount(0.01f);
                }
            }
            else
            {
                if (p_wall.X >= 0 && _grounds[p_wall.X, p_wall.Y].CurrentState == Ground.State.IMMERSE)
                {
                    if (p_wall.X < _grounds.GetLength(0) - 1 && _grounds[p_wall.X + 1, p_wall.Y].CurrentState == Ground.State.DRY)
                        _grounds[p_wall.X + 1, p_wall.Y].SetFillAmount(0.01f);
                }

                if (p_wall.X < _grounds.GetLength(0) - 1 && _grounds[p_wall.X + 1, p_wall.Y ].CurrentState == Ground.State.IMMERSE)
                {
                    if (p_wall.X >= 0 && _grounds[p_wall.X, p_wall.Y].CurrentState == Ground.State.DRY)
                        _grounds[p_wall.X, p_wall.Y].SetFillAmount(0.01f);
                }
            }
        }

        private void CreatePlayer()
        {
            int __randomX = UnityEngine.Random.Range(0, _mapWidth - 1);
            int __randomY = UnityEngine.Random.Range(0, _mapHeight - 1);

            Vector3 __spawnPosition = new Vector3(
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * __randomX) + (GameSettings.WALL_SIZE * __randomX), 
                1f,
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * __randomY) + (GameSettings.WALL_SIZE * __randomY));

            _player = Instantiate(_playerPrefab, __spawnPosition, Quaternion.identity, transform).GetComponent<Player>();

            _player.Initialize();

            _player.onPause -= Player_OnPause;
            _player.onPause += Player_OnPause;

            _player.UI.onGameOverFinish -= UI_OnGameOverFinish; 
            _player.UI.onGameOverFinish += UI_OnGameOverFinish;

            _player.UI.onPauseFinish -= UI_PauseFinish;
            _player.UI.onPauseFinish += UI_PauseFinish;
        }

        private void Player_OnPause()
        {
            Pause();
            _player.UI.EnablePausePanel(true);
        }

        private void UI_OnGameOverFinish(UI.GameOverOptions p_options)
        {
            switch(p_options)
            {
                case UI.GameOverOptions.Menu:
                    ChangeController(EnvironmentControllers.Menu);
                    break;
                case UI.GameOverOptions.Exit:
                    Application.Quit();
                    break;
            }
        }

        private void UI_PauseFinish(UI.PauseOptions p_options)
        {
            switch(p_options)
            {
                case UI.PauseOptions.Menu:
                    ChangeController(EnvironmentControllers.Menu);
                    break;
                case UI.PauseOptions.Resume:
                    _player.UI.EnablePausePanel(false);
                    Resume();
                    break;
                case UI.PauseOptions.Exit:
                    Application.Quit();
                    break;
            }
        }

        private void CreateGrounds()
        {
            _grounds = new Ground[_mapWidth, _mapHeight];

            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    _grounds[i, j] = CreateGround(i, j, (_floodSourceX == i && _floodSourceY == j));
                }
            }
        }

        private Ground CreateGround(int p_x, int p_y, bool p_startWithFlood)
        {
            Vector3 __spawnPosition = new Vector3(
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_x) + (GameSettings.WALL_SIZE * p_x),
                0f,
                GameSettings.GROUND_SIZE2 + (GameSettings.GROUND_SIZE * p_y) + (GameSettings.WALL_SIZE * p_y));

            int __bestIndex = 0;
            float __bestResult = 0;

            for (int i = 0; i < _listGroundPrefabs.Count; i++)
            {
                float __result = _listGroundPrefabs[i].spawnPercentage * UnityEngine.Random.Range(1f, 10f);

                if (__result > __bestResult)
                {
                    __bestResult = __result;
                    __bestIndex = i;
                }
            }   

            Ground __ground = Instantiate(_listGroundPrefabs[__bestIndex].spawnObject, transform).GetComponent<Ground>();

            ListenGroundEvents(__ground);

            __ground.name = $"Ground [{p_x}][{p_y}]";

            __ground.isFloodSource = p_startWithFlood;

            __ground.Initialize(p_x, p_y);
            __ground.SetFillAmount(0f);
            __ground.SetPosition(__spawnPosition);


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

            Wall __wall = Instantiate(_doorPrefab, transform).GetComponent<Wall>();

            switch(__wall.Type)
            {
                case Wall.WallType.DOOR:
                    (__wall as WallDoor).SetState((UnityEngine.Random.Range(0, 2) == 1) ? ItemState.Enabled : ItemState.Disabled);
                    ListenDoorEvents(__wall as WallDoor);
                    break;
            }
            __wall.name = $"Wall {(p_isHorizontal?"Horizontal":"Vertical")} [{p_x}][{p_y}]";
            __wall.Initialize(p_x, p_y);
            __wall.SetLocalScale(__scale);
            __wall.SetPosition(__spawnPosition);

            return __wall;
        }
    }
}

