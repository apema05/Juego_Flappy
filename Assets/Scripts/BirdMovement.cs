
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BirdMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadSubida = 5f;
    public float velocidadBajada = 9.8f;
    public float cooldown = 0.5f;

    [Header("Referencias")]
    public CharacterController Controller;
    public Animator animator;
    public TMP_Text contadorTexto;
    public TMP_Text contadorVidas;
    public GameObject canvasGameOver;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip golpeSonido;
    public AudioClip morirSonido;
    public AudioClip saltoSonido;

    [Header("Música")]
    public AudioSource musicaFondo;

    [Header("Muerte")]
    public float tiempoReinicio = 2f;

    [Header("Efectos Visuales")]
    public Image flashRojo;
    public Camera camaraPrincipal;
    public float duracionFlash = 0.3f;
    public float duracionTemblor = 0.2f;
    public float intensidadTemblor = 0.15f;

    [Header("Invulnerabilidad")]
    public float tiempoInvulnerabilidad = 1f;

    private float velocidadVertical = 0f;
    private float tiempoCooldown = 0f;
    private int puntuacion = 0;
    private int vidas = 3;
    private bool muerto = false;
    private bool invulnerable = false;

    private Coroutine coroutineFlash;
    private Coroutine coroutineTemblor;

    void Start()
    {
        if (Controller == null)
            Controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        // Ocultar Game Over
        if (canvasGameOver != null)
            canvasGameOver.SetActive(false);

        // Ocultar flash rojo
        if (flashRojo != null)
        {
            Color color = flashRojo.color;
            color.a = 0f;
            flashRojo.color = color;
            flashRojo.raycastTarget = false;
        }

        ActualizarContador();
        ActualizarVidas();
    }

    void Update()
    {
        if (muerto)
            return;

        // Reducir cooldown
        if (tiempoCooldown > 0)
            tiempoCooldown -= Time.deltaTime;

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) &&
            tiempoCooldown <= 0)
        {
            velocidadVertical = velocidadSubida;

            if (animator != null)
                animator.SetBool("Fly", true);

            if (audioSource != null && saltoSonido != null)
                audioSource.PlayOneShot(saltoSonido);

            tiempoCooldown = cooldown;
        }
        else
        {
            // Aplicar gravedad
            velocidadVertical -= velocidadBajada * Time.deltaTime;

            if (animator != null)
                animator.SetBool("Fly", false);
        }

        // Mover el pájaro
        if (Controller != null)
        {
            Controller.Move(
                Vector3.up * velocidadVertical * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (muerto)
            return;

        // Sumar puntuación
        if (other.CompareTag("Score"))
        {
            puntuacion++;
            ActualizarContador();
        }

        // Colisión con obstáculo
        if (other.CompareTag("Obstacle") && !invulnerable)
        {
            vidas--;
            ActualizarVidas();

            // Flash rojo y temblor de cámara
            EfectoGolpe();

            if (vidas > 0)
            {
                // Sonido de golpe
                if (audioSource != null && golpeSonido != null)
                    audioSource.PlayOneShot(golpeSonido);

                // Activar invulnerabilidad
                StartCoroutine(Invulnerabilidad());
            }
            else
            {
                GameOver();
            }
        }
    }

    private void EfectoGolpe()
    {
        // Flash rojo
        if (flashRojo != null)
        {
            if (coroutineFlash != null)
                StopCoroutine(coroutineFlash);

            coroutineFlash = StartCoroutine(FlashRojo());
        }

        // Temblor de cámara
        if (camaraPrincipal != null)
        {
            if (coroutineTemblor == null)
                coroutineTemblor = StartCoroutine(TemblarCamara());
        }
    }

    private IEnumerator FlashRojo()
    {
        float tiempo = 0f;
        Color color = flashRojo.color;

        color.a = 0.45f;
        flashRojo.color = color;

        while (tiempo < duracionFlash)
        {
            tiempo += Time.deltaTime;

            color.a = Mathf.Lerp(
                0.45f,
                0f,
                Mathf.Clamp01(tiempo / Mathf.Max(duracionFlash, 0.001f))
            );

            flashRojo.color = color;

            yield return null;
        }

        color.a = 0f;
        flashRojo.color = color;
        coroutineFlash = null;
    }

    private IEnumerator TemblarCamara()
    {
        Transform camara = camaraPrincipal.transform;
        Vector3 posicionInicial = camara.localPosition;

        float tiempo = 0f;

        while (tiempo < duracionTemblor)
        {
            float x = Random.Range(
                -intensidadTemblor,
                intensidadTemblor
            );

            float y = Random.Range(
                -intensidadTemblor,
                intensidadTemblor
            );

            camara.localPosition = posicionInicial +
                new Vector3(x, y, 0f);

            tiempo += Time.deltaTime;

            yield return null;
        }

        camara.localPosition = posicionInicial;
        coroutineTemblor = null;
    }

    private IEnumerator Invulnerabilidad()
    {
        invulnerable = true;

        yield return new WaitForSeconds(tiempoInvulnerabilidad);

        invulnerable = false;
    }

    private void GameOver()
    {
        if (muerto)
            return;

        muerto = true;

        // Detener movimiento
        if (Controller != null)
            Controller.enabled = false;

        if (animator != null)
            animator.SetBool("Fly", false);

        // Mostrar Game Over
        if (canvasGameOver != null)
            canvasGameOver.SetActive(true);

        // Detener música
        if (musicaFondo != null)
            musicaFondo.Stop();

        // Sonido de muerte
        if (audioSource != null && morirSonido != null)
            audioSource.PlayOneShot(morirSonido);

        StartCoroutine(ReiniciarPartida());
    }

    private IEnumerator ReiniciarPartida()
    {
        float duracionSonido = morirSonido != null
            ? morirSonido.length
            : 0f;

        float espera = Mathf.Max(
            tiempoReinicio,
            duracionSonido
        );

        yield return new WaitForSeconds(espera);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void ActualizarContador()
    {
        if (contadorTexto != null)
            contadorTexto.text = "Puntuación: " + puntuacion;
    }

    private void ActualizarVidas()
    {
        if (contadorVidas != null)
            contadorVidas.text = "Vidas: " + vidas;
    }
}
