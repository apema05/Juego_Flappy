
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabEscenario;
    public GameObject puntoSpawn;
    public float velEscenario;

    public Transform jugador;
    public float distanciaDestruccion = 40f;

    private float DistanceTravelled = 0;
    private List<GameObject> escenarios = new List<GameObject>();

    void Start()
    {
        // Crear el primer escenario
        GameObject escenario = Instantiate(
            prefabEscenario,
            puntoSpawn.transform.position,
            Quaternion.identity,
            transform
        );

        escenarios.Add(escenario);
    }

    public void Update()
    {
        // Desplazar el escenario hacia Z negativo
        float distancia = velEscenario * Time.deltaTime;

        transform.Translate( - Vector3.back * distancia, Space.World);

        // Acumular distancia recorrida
        DistanceTravelled += distancia;

        // Generar un nuevo escenario cada 20 unidades
        if (DistanceTravelled >= 20f)
        {
            DistanceTravelled -= 20f;

            GameObject escenario = Instantiate(
                prefabEscenario,
                puntoSpawn.transform.position,
                Quaternion.identity,
                transform
            );

            escenarios.Add(escenario);
        }

        // Comprobar la distancia de los escenarios al jugador
        for (int i = escenarios.Count - 1; i >= 0; i--)
        {
            if (escenarios[i] == null)
            {
                escenarios.RemoveAt(i);
                continue;
            }

            float distanciaJugador = Mathf.Abs(
                escenarios[i].transform.position.z - jugador.position.z
            );

            // Destruir si está demasiado lejos
            if (distanciaJugador > distanciaDestruccion)
            {
                Destroy(escenarios[i]);
                escenarios.RemoveAt(i);
            }
        }
    }
}
