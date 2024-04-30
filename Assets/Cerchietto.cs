using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image crosshairImage; // L'immagine del mirino nel tuo canvas
    public float maxDistance = 100f; // La distanza massima a cui il mirino può essere posizionato

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistance))
        {
            // Se il raggio colpisce qualcosa, posiziona il mirino lì
            crosshairImage.transform.position = Camera.main.WorldToScreenPoint(hit.point);
        }
        else
        {
            // Se il raggio non colpisce nulla, posiziona il mirino alla distanza massima
            crosshairImage.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * maxDistance);
        }
    }
}
