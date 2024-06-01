using UnityEngine;
using System.Collections.Generic;

public class FreezeCamera : MonoBehaviour
{
    public List<float> Positions = new List<float>();
    private CameraController camScript;
    private Camera mainCamera;
    private List<GameObject> enemies = new List<GameObject>();
    private bool _isFreezed = false;

    void Start()
    {
        mainCamera = Camera.main;
        camScript = mainCamera.GetComponent<CameraController>();

        GameObject[] rangeEnemies = GameObject.FindGameObjectsWithTag("RangeEnemy");
        GameObject[] meleeEnemies = GameObject.FindGameObjectsWithTag("MeleeEnemy");

        foreach (GameObject enemy in rangeEnemies)
        {
            enemies.Add(enemy);
        }

        foreach (GameObject enemy in meleeEnemies)
        {
            enemies.Add(enemy);
        }
    }

    void Update()
    {
        bool allEnemiesDefeated = true;

        foreach (GameObject enemy in enemies.ToArray()) 
        {
            if (enemy != null)
            {
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(enemy.transform.position);
                if (enemy.activeSelf && viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1)
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }
            else
            {
                enemies.Remove(enemy); 
            }
        }

        foreach (float targetPosition in Positions.ToArray())
        {
            float distance = Mathf.Abs(mainCamera.transform.position.x - targetPosition);
            if (distance < 0.05f && !_isFreezed) 
            {
                camScript.isFreezed = true;
                Positions.Remove(targetPosition); 
                _isFreezed = true;
                return;
            }
        }

        if (allEnemiesDefeated)
        {
            camScript.isFreezed = false;
            _isFreezed = false;
        }
    }
}
