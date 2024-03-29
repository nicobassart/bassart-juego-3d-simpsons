using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Herramientas de manipulación de vectores
    /// </summary>
    public class TgcVectorUtils
    {

        /// <summary>
        /// Longitud al cuadrado del segmento ab
        /// </summary>
        /// <param name="a">Punto inicial del segmento</param>
        /// <param name="b">Punto final del segmento</param>
        /// <returns>Longitud al cuadrado</returns>
        public static float lengthSq(Vector3 a, Vector3 b)
        {
            return Vector3.Subtract(a, b).LengthSq();
        }

        /// <summary>
        /// Multiplicar dos vectores.
        /// Se multiplica cada componente
        /// </summary>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <returns>Vector resultante</returns>
        public static Vector3 mul(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        /// <summary>
        /// Dividir dos vectores.
        /// Se divide cada componente
        /// </summary>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <returns>Vector resultante</returns>
        public static Vector3 div(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

    }
}
