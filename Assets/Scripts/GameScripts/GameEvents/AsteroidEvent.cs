using UnityEngine;
using System.Collections.Generic;

public sealed class AsteroidEvent : GameEvent {
    private List<Transform> Asteroids;
    private List<Vector3> AsteroidOrigin;
    private bool EndTurnActive;
    private float deltaPos;
    public SpriteRenderer Border;
    public GameObject Asteroid;

    public override void StartTurn() {
        EndTurnActive = false;
        Asteroids = new List<Transform>();
        AsteroidOrigin = new List<Vector3>();

        for (int i = 0; i < 4; i++) {
            Transform newAsteroid = Instantiate(Asteroid).transform;
            Vector3 newAsteroidOrigin = new Vector3(
                Random.Range(0.0f, GameController.Instance.GridSizeX * GameController.Instance.TileSize),
                GameController.Instance.GridSizeY + 1);
            newAsteroid.position = newAsteroidOrigin;
            newAsteroid.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            Asteroids.Add(newAsteroid);
            AsteroidOrigin.Add(newAsteroidOrigin);
        }
    }
    
    public override void EndTurn() {
        EndTurnActive = true;
        deltaPos = 0;
    }

    private void Update() {
        float alpha = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * 2 * Mathf.PI * 0.3f));
        Border.color = new Color(1, 0, 0, alpha);
        if (EndTurnActive) {
            deltaPos += Time.deltaTime / 1.75f;

            int i = 0;
            foreach (Transform asteroid in Asteroids) {
                Module module = GameController.Instance.GetRandomModule();
                asteroid.position = Vector3.Lerp(AsteroidOrigin[i], new Vector3(17.777f, 0.0f, 0.0f), deltaPos);
                i++;
            }
        }
    }
}
