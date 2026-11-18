using UnityEngine;

public static class FisicaColisiones
{
    private static readonly ContactPoint2D[] contactos = new ContactPoint2D[8];

    /// <summary>
    /// Frena el avance hacia otro objeto para que la colisión no produzca empuje
    /// </summary>
    public static Vector2 SinEmpujar(Rigidbody2D rb, Vector2 velocidad, string tagObjetivo)
    {
        int cantidad = rb.GetContacts(contactos);

        for (int i = 0; i < cantidad; i++)
        {
            Collider2D otro = contactos[i].collider;
            if (otro != null && otro.attachedRigidbody == rb)
                otro = contactos[i].otherCollider;

            if (otro == null || !otro.CompareTag(tagObjetivo))
                continue;

            Vector2 haciaOtro = ((Vector2)otro.bounds.center - rb.position).normalized;
            float avance = Vector2.Dot(velocidad, haciaOtro);

            if (avance > 0f)
                velocidad -= haciaOtro * avance;
        }

        return velocidad;
    }
}