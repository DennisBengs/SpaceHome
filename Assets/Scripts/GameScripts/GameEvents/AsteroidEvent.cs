using UnityEngine;
using System.Collections.Generic;

public sealed class AsteroidEvent : GameEvent {
    private List<Transform> Asteroids;
    private List<Vector3> AsteroidOrigin;
    private List<Module> TargetModules;
    private bool EndTurnActive;
    private float deltaPos;
    public SpriteRenderer Border;
    public GameObject Asteroid;

    public void Start() {
        Vector3 cameraPos = GameObject.Find("MainCamera").GetComponent<Transform>().position;
        GetComponent<Transform>().position = new Vector3(cameraPos.x, cameraPos.y, 1);
    }

    public override void StartTurn() {
        EndTurnActive = false;
        Asteroids = new List<Transform>();
        AsteroidOrigin = new List<Vector3>();
        TargetModules = new List<Module>();
    }
    
    private void SpawnAsteroids(int numberOfAsteroids) {
        for(int i = 0; i < numberOfAsteroids; i++) {
            Vector3 thisPos = GetComponent<Transform>().position;
            Transform newAsteroid = Instantiate(Asteroid).transform;
            Vector3 newAsteroidOrigin = new Vector3(
                Random.Range(0.0f, GameController.Instance.GridSizeX * GameController.Instance.TileSize) + thisPos.x,
                GameController.Instance.GridSizeY + thisPos.y + 16);
            newAsteroid.position = newAsteroidOrigin;
            newAsteroid.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            Asteroids.Add(newAsteroid);
            AsteroidOrigin.Add(newAsteroidOrigin);
        }
    }

    private void DestroyAsteroids() {
        foreach(Transform item in Asteroids) {
            Destroy(item.gameObject);
        }
        Asteroids.Clear();
    }

    public override void EndTurn() {
        int numberOfAsteroids = 0;

        numberOfAsteroids = Random.Range(1, GameController.Instance.GetModuleCount() + 1) + (int)(GameController.Instance.TurnIndex / 3);

        if (numberOfAsteroids > GameController.Instance.GetModuleCount()) {
            numberOfAsteroids = GameController.Instance.GetModuleCount();
        }

        SpawnAsteroids(numberOfAsteroids);
        for (int i = 0; i < Asteroids.Count; i++) {
            TargetModules.Add(GameController.Instance.GetRandomModule(TargetModules));
        }
        EndTurnActive = true;
        deltaPos = 0;
    }

    private void Update() {
        float alpha = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 2 * Mathf.PI * 0.3f));
        Border.color = new Color(1, 0, 0, alpha);
        if (EndTurnActive) {
            deltaPos += Time.deltaTime * 1.0f;

            int i = 0;
            foreach (Transform asteroid in Asteroids) {
                Vector3 newPos = Vector3.Lerp(AsteroidOrigin[i], TargetModules[i].GetCenter(), deltaPos);
                asteroid.position = newPos;
                i++;
            }

            if (deltaPos >= 1) {
                DestroyAsteroids();
                IsDestroyed = true;
                EndTurnActive = false;
                foreach (Module targetModule in TargetModules) {
                    targetModule.Damage();
                }
            }
        }
    }
}
