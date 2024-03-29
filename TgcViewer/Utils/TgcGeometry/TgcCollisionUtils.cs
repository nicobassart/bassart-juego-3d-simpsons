using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Utilidades para hacer detecci�n de colisiones
    /// </summary>
    public class TgcCollisionUtils
    {

        #region BoundingBox

        /// <summary>
        /// Clasifica un BoundingBox respecto de otro. Las opciones de clasificacion son:
        /// <para># Adentro: box1 se encuentra completamente dentro de la box2</para>
        /// <para># Afuera: box2 se encuentra completamente afuera de box1</para>
        /// <para># Atravesando: box2 posee una parte dentro de box1 y otra parte fuera de la box1</para>
        /// <para># Encerrando: box1 esta completamente adentro a la box1, es decir, la box1 se encuentra dentro
        ///     de la box2. Es un caso especial de que box2 est� afuera de box1</para>
        /// </summary>
        public static BoxBoxResult classifyBoxBox(TgcBoundingBox box1, TgcBoundingBox box2)
        {
            if (((box1.PMin.X <= box2.PMin.X && box1.PMax.X >= box2.PMax.X) ||
                (box1.PMin.X >= box2.PMin.X && box1.PMin.X <= box2.PMax.X) ||
                (box1.PMax.X >= box2.PMin.X && box1.PMax.X <= box2.PMax.X)) &&
               ((box1.PMin.Y <= box2.PMin.Y && box1.PMax.Y >= box2.PMax.Y) ||
                (box1.PMin.Y >= box2.PMin.Y && box1.PMin.Y <= box2.PMax.Y) ||
                (box1.PMax.Y >= box2.PMin.Y && box1.PMax.Y <= box2.PMax.Y)) &&
               ((box1.PMin.Z <= box2.PMin.Z && box1.PMax.Z >= box2.PMax.Z) ||
                (box1.PMin.Z >= box2.PMin.Z && box1.PMin.Z <= box2.PMax.Z) ||
                (box1.PMax.Z >= box2.PMin.Z && box1.PMax.Z <= box2.PMax.Z)))
            {
                if ((box1.PMin.X <= box2.PMin.X) &&
                   (box1.PMin.Y <= box2.PMin.Y) &&
                   (box1.PMin.Z <= box2.PMin.Z) &&
                   (box1.PMax.X >= box2.PMax.X) &&
                   (box1.PMax.Y >= box2.PMax.Y) &&
                   (box1.PMax.Z >= box2.PMax.Z))
                {
                    return BoxBoxResult.Adentro;
                }
                else if ((box1.PMin.X > box2.PMin.X) &&
                         (box1.PMin.Y > box2.PMin.Y) &&
                         (box1.PMin.Z > box2.PMin.Z) &&
                         (box1.PMax.X < box2.PMax.X) &&
                         (box1.PMax.Y < box2.PMax.Y) &&
                         (box1.PMax.Z < box2.PMax.Z))
                {
                    return BoxBoxResult.Encerrando;
                }
                else
                {
                    return BoxBoxResult.Atravesando;
                }
            }
            else
            {
                return BoxBoxResult.Afuera;
            }
        }

        /// <summary>
        /// Resultado de una clasificaci�n BoundingBox-BoundingBox
        /// </summary>
        public enum BoxBoxResult
        {
            /// <summary>
            /// El BoundingBox 1 se encuentra completamente adentro del BoundingBox 2
            /// </summary>
            Adentro,
            /// <summary>
            /// El BoundingBox 1 se encuentra completamente afuera del BoundingBox 2
            /// </summary>
            Afuera,
            /// <summary>
            /// El BoundingBox 1 posee parte adentro y parte afuera del BoundingBox 2
            /// </summary>
            Atravesando,
            /// <summary>
            /// El BoundingBox 1 contiene completamente adentro al BoundingBox 2.
            /// Caso particular de Afuera.
            /// </summary>
            Encerrando
        }
    
        /// <summary>
        /// Indica si un BoundingBox colisiona con otro.
        /// Solo indica si hay colisi�n o no. No va mas en detalle.
        /// </summary>
        /// <param name="a">BoundingBox 1</param>
        /// <param name="b">BoundingBox 2</param>
        /// <returns>True si hay colisi�n</returns>
        public static bool testAABBAABB(TgcBoundingBox a, TgcBoundingBox b)
        {
            // Exit with no intersection if separated along an axis
            if (a.PMax.X < b.PMin.X || a.PMin.X > b.PMax.X) return false;
            if (a.PMax.Y < b.PMin.Y || a.PMin.Y > b.PMax.Y) return false;
            if (a.PMax.Z < b.PMin.Z || a.PMin.Z > b.PMax.Z) return false;
            // Overlapping on all axes means AABBs are intersecting
            return true;
        }
        

        /// <summary>
        /// Indica si un Ray colisiona con un AABB.
        /// Si hay intersecci�n devuelve True, q contiene
        /// el punto de intesecci�n.
        /// Basado en el c�digo de: http://www.codercorner.com/RayAABB.cpp
        /// La direcci�n del Ray puede estar sin normalizar.
        /// </summary>
        /// <param name="ray">Ray</param>
        /// <param name="a">AABB</param>
        /// <param name="q">Punto de intersecci�n</param>
        /// <returns>True si hay colisi�n</returns>
        public static bool intersectRayAABB(TgcRay ray, TgcBoundingBox aabb, out Vector3 q)
        {
            q = Vector3.Empty;
            bool inside = true;
            float[] aabbMin = toArray(aabb.PMin);
            float[]aabbMax = toArray(aabb.PMax);
            float[]rayOrigin = toArray(ray.Origin);
            float[]rayDir = toArray(ray.Direction);

            float[] max_t = new float[3]{-1.0f, -1.0f, -1.0f};
            float[] coord = new float[3];

            for (uint i = 0; i < 3; ++i)
            {
                if (rayOrigin[i] < aabbMin[i])
                {
                    inside = false;
                    coord[i] = aabbMin[i];

                    if (rayDir[i] != 0.0f)
                    {
                        max_t[i] = (aabbMin[i] - rayOrigin[i]) / rayDir[i];
                    }
                }
                else if (rayOrigin[i] > aabbMax[i])
                {
                    inside = false;
                    coord[i] = aabbMax[i];

                    if (rayDir[i] != 0.0f)
                    {
                        max_t[i] = (aabbMax[i] - rayOrigin[i]) / rayDir[i];
                    }
                }
            }

            // If the Ray's start position is inside the Box, we can return true straight away.
            if (inside)
            {
                q = toVector3(rayOrigin);
                return true;
            }

            uint plane = 0;
            if (max_t[1] > max_t[plane])
            {
                plane = 1;
            }
            if (max_t[2] > max_t[plane])
            {
                plane = 2;
            }
            
            if (max_t[plane] < 0.0f)
            {
                return false;
            }

            for (uint i = 0; i < 3; ++i)
            {
                if (plane != i)
                {
                    coord[i] = rayOrigin[i] + max_t[plane] * rayDir[i];

                    if (coord[i] < aabbMin[i] - float.Epsilon || coord[i] > aabbMax[i] + float.Epsilon)
                    {
                        return false;
                    }
                }
            }

            q = toVector3(coord);
            return true;
        }

        /// <summary>
        /// Indica si el segmento de recta compuesto por p0-p1 colisiona con el BoundingBox.
        /// </summary>
        /// <param name="p0">Punto inicial del segmento</param>
        /// <param name="p1">Punto final del segmento</param>
        /// <param name="aabb">BoundingBox</param>
        /// <param name="q">Punto de intersecci�n</param>
        /// <returns>True si hay colisi�n</returns>
        public static bool intersectSegmentAABB(Vector3 p0, Vector3 p1, TgcBoundingBox aabb, out Vector3 q)
        {
            Vector3 segmentDir = p1 - p0;
            TgcRay ray = new TgcRay(p0, segmentDir);
            if (TgcCollisionUtils.intersectRayAABB(ray, aabb, out q))
            {
                float segmentLengthSq = segmentDir.LengthSq();
                Vector3 collisionDiff = q - p0;
                float collisionLengthSq = collisionDiff.LengthSq();
                if (collisionLengthSq <= segmentLengthSq)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Dado el punto p, devuelve el punto del contorno del BoundingBox mas pr�ximo a p.
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="aabb">BoundingBox a testear</param>
        /// <returns>Punto mas cercano a p del BoundingBox</returns>
        public static Vector3 closestPointAABB(Vector3 p, TgcBoundingBox aabb)
        {
            float[] aabbMin = toArray(aabb.PMin);
            float[] aabbMax = toArray(aabb.PMax);
            float[] pArray = toArray(p);
            float[] q = new float[3];

            // For each coordinate axis, if the point coordinate value is
            // outside box, clamp it to the box, else keep it as is
            for (int i = 0; i < 3; i++)
            {
                float v = pArray[i];
                if (v < aabbMin[i]) v = aabbMin[i]; // v = max(v, b.min[i])
                if (v > aabbMax[i]) v = aabbMax[i]; // v = min(v, b.max[i])
                q[i] = v;
            }
            return TgcCollisionUtils.toVector3(q);
        }

        /// <summary>
        /// Calcula la m�nima distancia al cuadrado entre el punto p y el BoundingBox.
        /// Si no se necesita saber el punto exacto de colisi�n es m�s �gil que utilizar closestPointAABB(). 
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>M�nima distacia al cuadrado</returns>
        public static float sqDistPointAABB(Vector3 p, TgcBoundingBox aabb)
        {
            float[] aabbMin = toArray(aabb.PMin);
            float[] aabbMax = toArray(aabb.PMax);
            float[] pArray = toArray(p);
            float sqDist = 0.0f;

            for (int i = 0; i < 3; i++)
            {
                // For each axis count any excess distance outside box extents
                float v = pArray[i];
                if (v < aabbMin[i]) sqDist += (aabbMin[i] - v) * (aabbMin[i] - v);
                if (v > aabbMax[i]) sqDist += (v - aabbMax[i]) * (v - aabbMax[i]);
            }
            return sqDist;
        }

        /// <summary>
        /// Indica si un BoundingSphere colisiona con un BoundingBox.
        /// </summary>
        /// <param name="sphere">BoundingSphere</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>True si hay colisi�n</returns>
        public static bool testSphereAABB(TgcBoundingSphere sphere, TgcBoundingBox aabb)
        {
            //Compute squared distance between sphere center and AABB
            float sqDist = TgcCollisionUtils.sqDistPointAABB(sphere.Center, aabb);
            //Sphere and AABB intersect if the (squared) distance
            //between them is less than the (squared) sphere radius
            return sqDist <= sphere.Radius * sphere.Radius;
        }

        /// <summary>
        /// Clasifica un BoundingBox respecto de un Plano.
        /// </summary>
        /// <param name="plane">Plano</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>
        /// Resultado de la clasificaci�n.
        /// </returns>
        /// 
        public static PlaneBoxResult classifyPlaneAABB(Plane plane, TgcBoundingBox aabb)
        {
            Vector3 vmin = Vector3.Empty;
            Vector3 vmax = Vector3.Empty;

            //Obtener puntos minimos y maximos en base a la direcci�n de la normal del plano
            if (plane.A >= 0f)
            {
                vmin.X = aabb.PMin.X;
                vmax.X = aabb.PMax.X;
            }
            else
            {
                vmin.X = aabb.PMax.X;
                vmax.X = aabb.PMin.X;
            }

            if (plane.B >= 0f)
            {
                vmin.Y = aabb.PMin.Y;
                vmax.Y = aabb.PMax.Y;
            }
            else
            {
                vmin.Y = aabb.PMax.Y;
                vmax.Y = aabb.PMin.Y;
            }

            if (plane.C >= 0f)
            {
                vmin.Z = aabb.PMin.Z;
                vmax.Z = aabb.PMax.Z;
            }
            else
            {
                vmin.Z = aabb.PMax.Z;
                vmax.Z = aabb.PMin.Z;
            }

            //Analizar punto minimo y maximo contra el plano
            PlaneBoxResult result;
            if (plane.Dot(vmin) > 0f)
            {
                result = PlaneBoxResult.IN_FRONT_OF;
            } 
            else if(plane.Dot(vmax) > 0f)
            {
                result = PlaneBoxResult.INTERSECT;
            }
            else 
            {
                result = PlaneBoxResult.BEHIND;
            }

            return result;
        }

        /// <summary>
        /// Resultado de una clasificaci�n Plano-BoundingBox
        /// </summary>
        public enum PlaneBoxResult
        {
            /// <summary>
            /// El BoundingBox est� completamente en el lado negativo el plano
            /// </summary>
            BEHIND,

            /// <summary>
            /// El BoundingBox est� completamente en el lado positivo del plano
            /// </summary>
            IN_FRONT_OF,

            /// <summary>
            /// El Plano atraviesa al BoundingBox
            /// </summary>
            INTERSECT,
        }

        /// <summary>
        /// Indica si un Plano colisiona con un BoundingBox
        /// </summary>
        /// <param name="plane">Plano</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>True si hay colisi�n.</returns>
        public static bool testPlaneAABB(Plane plane, TgcBoundingBox aabb)
        {
            Vector3 c = (aabb.PMax + aabb.PMin) * 0.5f; // Compute AABB center
            Vector3 e = aabb.PMax - c; // Compute positive extents

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            float r = e.X * FastMath.Abs(plane.A) + e.Y * FastMath.Abs(plane.B) + e.Z * FastMath.Abs(plane.C);
            // Compute distance of box center from plane
            float s = plane.Dot(c);
            // Intersection occurs when distance s falls within [-r,+r] interval
            return FastMath.Abs(s) <= r;
        }

        #endregion


        #region BoundingSphere

        /// <summary>
        /// Indica si un BoundingSphere colisiona con otro.
        /// </summary>
        /// <returns>True si hay colisi�n</returns>
        public static bool testSphereSphere(TgcBoundingSphere a, TgcBoundingSphere b)
        {
            // Calculate squared distance between centers
            Vector3 d = a.Center - b.Center;
            float dist2 = Vector3.Dot(d, d);
            // Spheres intersect if squared distance is less than squared sum of radii
            float radiusSum = a.Radius + b.Radius;
            return dist2 <= radiusSum * radiusSum;
        }

        /// <summary>
        /// Idica si un BoundingSphere colisiona con un plano
        /// </summary>
        /// <returns>True si hay colisi�n</returns>
        public static bool testSpherePlane(TgcBoundingSphere s, Plane plane)
        {
            Vector3 p = toVector3(plane);

            // For a normalized plane (|p.n| = 1), evaluating the plane equation
            // for a point gives the signed distance of the point to the plane
            float dist = Vector3.Dot(s.Center, p) - plane.D;
            // If sphere center within +/-radius from plane, plane intersects sphere
            return FastMath.Abs(dist) <= s.Radius;
        }

        // 
        /// <summary>
        /// Indica si un BoundingSphere se encuentra completamente en el lado negativo del plano
        /// </summary>
        /// <returns>True si se encuentra completamente en el lado negativo del plano</returns>
        public static bool insideSpherePlane(TgcBoundingSphere s, Plane plane)
        {
            Vector3 p = toVector3(plane);

            float dist = Vector3.Dot(s.Center, p) - plane.D;
            return dist < -s.Radius;
        }


        /// <summary>
        /// Indica si un Ray colisiona con un BoundingSphere.
        /// Si el resultado es True se carga el punto de colision (q) y la distancia de colision en el Ray (t).
        /// La direcci�n del Ray debe estar normalizada.
        /// </summary>
        /// <param name="ray">Ray</param>
        /// <param name="sphere">BoundingSphere</param>
        /// <param name="t">Distancia de colision del Ray</param>
        /// <param name="q">Punto de colision</param>
        /// <returns>True si hay colision</returns>
        public static bool intersectRaySphere(TgcRay ray, TgcBoundingSphere sphere, out float t, out Vector3 q)
        {
            t = -1;
            q = Vector3.Empty;

            Vector3 m = ray.Origin - sphere.Center;
            float b = Vector3.Dot(m, ray.Direction);
            float c = Vector3.Dot(m, m) - sphere.Radius * sphere.Radius;
            // Exit if r�s origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f) return false;
            float discr = b*b - c;
            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0.0f) return false;
            // Ray now found to intersect sphere, compute smallest t value of intersection
            t = -b - FastMath.Sqrt(discr);
            // If t is negative, ray started inside sphere so clamp t to zero
            if (t < 0.0f) t = 0.0f;
            q = ray.Origin + t * ray.Direction;
            return true;
        }

        /// <summary>
        /// Indica si un segmento de recta colisiona con un BoundingSphere.
        /// Si el resultado es True se carga el punto de colision (q) y la distancia de colision en el t.
        /// La direcci�n del Ray debe estar normalizada.
        /// </summary>
        /// <param name="p0">Punto inicial del segmento</param>
        /// <param name="p1">Punto final del segmento</param>
        /// <param name="s">BoundingSphere</param>
        /// <param name="t">Distancia de colision del segmento</param>
        /// <param name="q">Punto de colision</param>
        /// <returns>True si hay colision</returns>
        public static bool intersectSegmentSphere(Vector3 p0, Vector3 p1, TgcBoundingSphere sphere, out float t, out Vector3 q)
        {
            Vector3 segmentDir = p1 - p0;
            TgcRay ray = new TgcRay(p0, segmentDir);
            if (TgcCollisionUtils.intersectRaySphere(ray, sphere, out t, out q))
            {
                float segmentLengthSq = segmentDir.LengthSq();
                Vector3 collisionDiff = q - p0;
                float collisionLengthSq = collisionDiff.LengthSq();
                if (collisionLengthSq <= segmentLengthSq)
                {
                    return true;
                }
            }

            return false;
        }


        // 
        /// <summary>
        /// Indica si un BoundingSphere colisiona con un Ray (sin indicar su punto de colision)
        /// La direcci�n del Ray debe estar normalizada.
        /// </summary>
        /// <param name="ray">Ray</param>
        /// <param name="sphere">BoundingSphere</param>
        /// <returns>True si hay colision</returns>
        public static bool testRaySphere(TgcRay ray, TgcBoundingSphere sphere)
        {
            Vector3 m = ray.Origin - sphere.Center;
            float c = Vector3.Dot(m, m) - sphere.Radius * sphere.Radius;
            // If there is definitely at least one real root, there must be an intersection
            if (c <= 0.0f) return true;
            float b = Vector3.Dot(m, ray.Direction);
            // Early exit if ray origin outside sphere and ray pointing away from sphere
            if (b > 0.0f) return false;
            float disc = b * b - c;
            // A negative discriminant corresponds to ray missing sphere
            if (disc < 0.0f) return false;
            // Now ray must hit sphere
            return true;
        }

        /// <summary>
        /// Indica si el punto p se encuentra dentro de la esfera
        /// </summary>
        /// <param name="sphere">BoundingSphere</param>
        /// <param name="p">Punto a testear</param>
        /// <returns>True si p est� dentro de la esfera</returns>
        public static bool testPointSphere(TgcBoundingSphere sphere, Vector3 p)
        {
            Vector3 cp = p - sphere.Center;
            float d = cp.LengthSq();

            return d <= (sphere.Radius * sphere.Radius);
        }



        #endregion


        #region Planos, Segmentos y otros

        /// <summary>
        /// Determina el punto del plano p mas cercano al punto q.
        /// La normal del plano puede estar sin normalizar.
        /// </summary>
        /// <param name="q">Punto a testear</param>
        /// <param name="p">Plano</param>
        /// <returns>Punto del plano que mas cerca esta de q</returns>
        public static Vector3 closestPointPlane(Vector3 q, Plane p)
        {
            Vector3 p_n = toVector3(p);

            float t = (Vector3.Dot(p_n, q) + p.D) / Vector3.Dot(p_n, p_n);
            return q - t * p_n;
        }

        /// <summary>
        /// Determina el punto del plano p mas cercano al punto q.
        /// M�s �gil que closestPointPlane() pero la normal del plano debe estar normalizada.
        /// </summary>
        /// <param name="q">Punto a testear</param>
        /// <param name="p">Plano</param>
        /// <returns>Punto del plano que mas cerca esta de q</returns>
        public static Vector3 closestPointPlaneNorm(Vector3 q, Plane p)
        {
            Vector3 p_n = toVector3(p);

            float t = (Vector3.Dot(p_n, q) + p.D);
            return q - t * p_n;
        }

        /// <summary>
        /// Indica la distancia de un punto al plano
        /// </summary>
        /// <param name="q">Punto a testear</param>
        /// <param name="p">Plano</param>
        /// <returns>Distancia del punto al plano</returns>
        public static float distPointPlane(Vector3 q, Plane p)
        {
            /*
            Vector3 p_n = toVector3(p);
            return (Vector3.Dot(p_n, q) + p.D) / Vector3.Dot(p_n, p_n);
            */
            return p.Dot(q);
        }

        /// <summary>
        /// Clasifica un Punto respecto de un Plano
        /// </summary>
        /// <param name="q">Punto a clasificar</param>
        /// <param name="p">Plano</param>
        /// <returns>Resultado de la colisi�n</returns>
        public static PointPlaneResult classifyPointPlane(Vector3 q, Plane p)
        {
            float distance = distPointPlane(q, p);

            if (distance < -float.Epsilon)
            {
                return PointPlaneResult.BEHIND;
            }
            else if (distance > float.Epsilon)
            {
                return PointPlaneResult.IN_FRONT_OF;
            }
            else
            {
                return PointPlaneResult.COINCIDENT;
            } 
        }

        /// <summary>
        /// Resultado de una clasificaci�n Punto-Plano
        /// </summary>
        public enum PointPlaneResult
        {
            /// <summary>
            /// El punto est� sobre el lado negativo el plano
            /// </summary>
            BEHIND,

            /// <summary>
            /// El punto est� sobre el lado positivo del plano
            /// </summary>
            IN_FRONT_OF,

            /// <summary>
            /// El punto pertenece al plano
            /// </summary>
            COINCIDENT,
        }

        /// <summary>
        /// Dado el segmento ab y el punto p, determina el punto mas cercano sobre el segmento ab.
        /// </summary>
        /// <param name="c">Punto a testear</param>
        /// <param name="a">Inicio del segmento ab</param>
        /// <param name="b">Fin del segmento ab</param>
        /// <param name="t">Valor que cumple la ecuacion d(t) = a + t*(b - a)</param>
        /// <returns>Punto sobre ab que esta mas cerca de p</returns>
        public static Vector3 closestPointSegment(Vector3 p, Vector3 a, Vector3 b, out float t)
        {
            Vector3 ab = b - a;
            // Project c onto ab, computing parameterized position d(t) = a + t*(b � a)
            t = Vector3.Dot(p - a, ab) / Vector3.Dot(ab, ab);
            // If outside segment, clamp t (and therefore d) to the closest endpoint
            if (t < 0.0f) t = 0.0f;
            if (t > 1.0f) t = 1.0f;
            // Compute projected position from the clamped t
            return a + t * ab;
        }

        /// <summary>
        /// Devuelve la distancia al cuadrado entre el punto c y el segmento ab
        /// </summary>
        /// <param name="a">Inicio del segmento ab</param>
        /// <param name="b">Fin del segmento ab</param>
        /// <param name="c">Punto a testear</param>
        /// <returns>Distancia al cuadrado entre c y ab</returns>
        public static float sqDistPointSegment(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a, ac = c - a, bc = c - b;
            float e = Vector3.Dot(ac, ab);
            // Handle cases where c projects outside ab
            if (e <= 0.0f) return Vector3.Dot(ac, ac);
            float f = Vector3.Dot(ab, ab);
            if (e >= f) return Vector3.Dot(bc, bc);
            // Handle cases where c projects onto ab
            return Vector3.Dot(ac, ac) - e * e / f;
        }

        /// <summary>
        /// Indica si un Ray colisiona con un Plano.
        /// Tanto la normal del plano como la direcci�n del Ray se asumen normalizados.
        /// </summary>
        /// <param name="ray">Ray a testear</param>
        /// <param name="plane">Plano a testear</param>
        /// <param name="t">Instante de colisi�n</param>
        /// <param name="q">Punto de colisi�n con el plano</param>
        /// <returns>True si hubo colisi�n</returns>
        public static bool intersectRayPlane(TgcRay ray, Plane plane, out float t, out Vector3 q)
        {
            Vector3 planeNormal = TgcCollisionUtils.getPlaneNormal(plane);
            float numer = plane.Dot(ray.Origin);
            float denom = Vector3.Dot(planeNormal, ray.Direction);
            t = -numer / denom;

            if(t > 0.0f)
            {
                q = ray.Origin + ray.Direction * t;
                return true;
            }

            q = Vector3.Empty;
            return false;
        }
        
        /// <summary>
        /// Indica si el segmento de recta compuesto por a-b colisiona con el Plano.
        /// La normal del plano se considera normalizada.
        /// </summary>
        /// <param name="a">Punto inicial del segmento</param>
        /// <param name="b">Punto final del segmento</param>
        /// <param name="plane">Plano a testear</param>
        /// <param name="t">Instante de colisi�n</param>
        /// <param name="q">Punto de colisi�n</param>
        /// <returns>True si hay colisi�n</returns>
        public static bool intersectSegmentPlane(Vector3 a, Vector3 b, Plane plane, out float t, out Vector3 q)
        {
            Vector3 planeNormal = getPlaneNormal(plane);

            //t = -(n.A + d / n.(B - A))
            Vector3 ab = b - a;
            t = -(plane.Dot(a)) / (Vector3.Dot(planeNormal, ab));

            // If t in [0..1] compute and return intersection point
            if (t >= 0.0f && t <= 1.0f)
            {
                q = a  + t * ab;
                return true;
            }

            q = Vector3.Empty;
            return false;
        }

        /// <summary>
        /// Determina el punto mas cercano entre el tri�ngulo (abc) y el punto p.
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="a">V�rtice A del tri�ngulo</param>
        /// <param name="b">V�rtice B del tri�ngulo</param>
        /// <param name="c">V�rtice C del tri�ngulo</param>
        /// <returns>Punto mas cercano al tri�ngulo</returns>
        public static Vector3 closestPointTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            // Check if P in vertex region outside A
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;
            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f) return a; // barycentric coordinates (1,0,0)
            // Check if P in vertex region outside B
            Vector3 bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0.0f && d4 <= d3) return b; // barycentric coordinates (0,1,0)
            // Check if P in edge region of AB, if so return projection of P onto AB
            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                float v = d1 / (d1 - d3);
                return a + v * ab; // barycentric coordinates (1-v,v,0)
            }
            // Check if P in vertex region outside C
            Vector3 cp = p - c;
            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);
            if (d6 >= 0.0f && d5 <= d6) return c; // barycentric coordinates (0,0,1)

            // Check if P in edge region of AC, if so return projection of P onto AC
            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                float w = d2 / (d2 - d6);
                return a + w * ac; // barycentric coordinates (1-w,0,w)
            }
            // Check if P in edge region of BC, if so return projection of P onto BC
            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + w * (c - b); // barycentric coordinates (0,1-w,w)
            }
            // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
            float denom = 1.0f / (va + vb + vc);
            float vFinal = vb * denom;
            float wFinal = vc * denom;
            return a + ab * vFinal + ac * wFinal; // = u*a + v*b + w*c, u = va * denom = 1.0f - v - w
        }

        /*
        /// <summary>
        /// Determina el punto mas cercano entre el rect�ngulo 3D (abcd) y el punto p.
        /// Los cuatro puntos abcd del rect�ngulo deben estar contenidos sobre el mismo plano.
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="a">V�rtice A del rect�ngulo</param>
        /// <param name="b">V�rtice B del rect�ngulo</param>
        /// <param name="c">V�rtice C del rect�ngulo</param>
        /// <param name="c">V�rtice D del rect�ngulo</param>
        /// <returns>Punto mas cercano al rect�ngulo</returns>
        public static Vector3 closestPointRectangle3d(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            //Buscar el punto mas cercano a cada uno de los 4 segmentos de recta que forman el rect�ngulo
            float t;
            Vector3[] points = new Vector3[4];
            points[0] = closestPointSegment(p, a, b, out t);
            points[1] = closestPointSegment(p, b, c, out t);
            points[2] = closestPointSegment(p, c, d, out t);
            points[3] = closestPointSegment(p, d, a, out t);

            //Buscar el menor punto de los 4
            return TgcCollisionUtils.closestPoint(p, points, out t);
        }
        */

        /// <summary>
        /// Determina el punto mas cercano entre un rect�nglo 3D (especificado por a, b y c) y el punto p.
        /// Los puntos a, b y c deben formar un rect�ngulo 3D tal que los vectores AB y AC expandan el rect�ngulo.
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="a">V�rtice A del rect�ngulo</param>
        /// <param name="b">V�rtice B del rect�ngulo</param>
        /// <param name="c">V�rtice C del rect�ngulo</param>
        /// <returns></returns>
        public static Vector3 closestPointRectangle3d(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a; // vector across rect
            Vector3 ac = c - a; // vector down rect
            Vector3 d = p - a;
            // Start result at top-left corner of rect; make steps from there
            Vector3 q = a;
            // Clamp p� (projection of p to plane of r) to rectangle in the across direction
            float dist = Vector3.Dot(d, ab);
            float maxdist = Vector3.Dot(ab, ab);
            if (dist >= maxdist)
                q += ab;
            else if (dist > 0.0f)
                q += (dist / maxdist) * ab;
            // Clamp p� (projection of p to plane of r) to rectangle in the down direction
            dist = Vector3.Dot(d, ac);
            maxdist = Vector3.Dot(ac, ac);
            if (dist >= maxdist)
                q += ac;
            else if (dist > 0.0f)
                q += (dist / maxdist) * ac;

            return q;
        }

        /// <summary>
        /// Indica el punto mas cercano a p
        /// </summary>
        /// <param name="p">Punto a testear</param>
        /// <param name="points">Array de puntos del cual se quiere buscar el mas cercano</param>
        /// <param name="minDistSq">Distancia al cuadrado del punto mas cercano</param>
        /// <returns>Punto m�s cercano a p del array</returns>
        public static Vector3 closestPoint(Vector3 p, Vector3[] points, out float minDistSq)
        {
            Vector3 min = points[0];
            Vector3 diffVec = points[0] - p;
            minDistSq = diffVec.LengthSq();
            float distSq;
            for (int i = 1; i < points.Length; i++)
            {
                diffVec = points[i] - p;
                distSq = diffVec.LengthSq();
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    min = points[i];
                }
            }

            return min;
        }

        

        
        #endregion


        #region Frustum

        
        /// <summary>
        /// Clasifica un BoundingBox respecto del Frustum
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>Resultado de la clasificaci�n</returns>
        public static FrustumResult classifyFrustumAABB(TgcFrustum frustum, TgcBoundingBox aabb)
        {
	        int totalIn = 0;
            Plane[] frustumPlanes = frustum.FrustumPlanes;

	        // get the corners of the box into the vCorner array
	        Vector3[] aabbCorners = aabb.computeCorners();

	        // test all 8 corners against the 6 sides 
	        // if all points are behind 1 specific plane, we are out
	        // if we are in with all points, then we are fully in
	        for(int p = 0; p < 6; ++p) 
            {
		        int inCount = 8;
		        int ptIn = 1;

		        for(int i = 0; i < 8; ++i) 
                {
			        // test this point against the planes
                    if (classifyPointPlane(aabbCorners[i], frustumPlanes[p]) == PointPlaneResult.BEHIND)
                    {
			            ptIn = 0;
				        --inCount;
			        }
		        }

		        // were all the points outside of plane p?
                if (inCount == 0)
                {
                    return FrustumResult.OUTSIDE;
                }
			        
		        // check if they were all on the right side of the plane
		        totalIn += ptIn;
	        }

	        // so if iTotalIn is 6, then all are inside the view
            if (totalIn == 6)
            {
                return FrustumResult.INSIDE;
            }

	        // we must be partly in then otherwise
            return FrustumResult.INTERSECT;
        }


        /*
        classifyFrustumAABB SUPUESTAMENTE ES MAS RAPIDO PERO NO FUNCIONA BIEN
        
        /// <summary>
        /// Clasifica un BoundingBox respecto del Frustum
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>Resultado de la colisi�n</returns>
        public static FrustumResult classifyFrustumAABB(TgcFrustum frustum, TgcBoundingBox aabb)
        {
            bool intersect = false;
            FrustumResult result = FrustumResult.OUTSIDE;
            Vector3 minExtreme;
            Vector3 maxExtreme;
            Plane[] m_frustumPlanes = frustum.FrustumPlanes;


            for (int i = 0; i < 6 ; i++ )
            {
                if (m_frustumPlanes[i].A <= 0)
                {
                   minExtreme.X = aabb.PMin.X;
                   maxExtreme.X = aabb.PMax.X;
                }
                else
                {
                    minExtreme.X = aabb.PMax.X;
                   maxExtreme.X = aabb.PMin.X;
                }

                if (m_frustumPlanes[i].B <= 0)
                {
                    minExtreme.Y = aabb.PMin.Y;
                    maxExtreme.Y = aabb.PMax.Y;
                }
                else
                {
                    minExtreme.Y = aabb.PMax.Y;
                   maxExtreme.Y = aabb.PMin.Y;
                }

                if (m_frustumPlanes[i].C <= 0)
                {
                    minExtreme.Z = aabb.PMin.Z;
                    maxExtreme.Z = aabb.PMax.Z;
                }
                else
                {
                    minExtreme.Z = aabb.PMax.Z;
                    maxExtreme.Z = aabb.PMin.Z; 
                }

                if (classifyPointPlane(minExtreme, m_frustumPlanes[i]) == PointPlaneResult.IN_FRONT_OF)
                {
                    result = FrustumResult.OUTSIDE;
                    return result;
                }

                if (classifyPointPlane(maxExtreme, m_frustumPlanes[i]) != PointPlaneResult.BEHIND)
                {
                    intersect = true;
                }    
            }

            if (intersect)
            {
                result = FrustumResult.INTERSECT;
            }
            else
            {
                result = FrustumResult.INSIDE;
            }
                
            return result;
        }
        */


        /// <summary>
        /// Resultado de una colisi�n entre un objeto y el Frustum
        /// </summary>
        public enum FrustumResult
        {
            /// <summary>
            /// El objeto se encuentra completamente fuera del Frustum
            /// </summary>
            OUTSIDE,

            /// <summary>
            /// El objeto se encuentra completamente dentro del Frustum
            /// </summary>
            INSIDE,

            /// <summary>
            /// El objeto posee parte fuera y parte dentro del Frustum
            /// </summary>
            INTERSECT,
        }
        
        /// <summary>
        /// Indica si un Punto colisiona con el Frustum
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <param name="p">Punto</param>
        /// <returns>True si el Punto est� adentro del Frustum</returns>
        public static bool testPointFrustum(TgcFrustum frustum, Vector3 p) {

            bool result = true;
            Plane[] frustumPlanes = frustum.FrustumPlanes;

		    for(int i=0; i < 6; i++) 
            {
                if (distPointPlane(p, frustumPlanes[i]) < 0)
                {
                    return false;
                }
		    }
            return result;

	    }

        /// <summary>
        /// Indica si un BoundingSphere colisiona con el Frustum
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <param name="sphere">BoundingSphere</param>
        /// <returns>Resultado de la colisi�n</returns>
        public static FrustumResult classifyFrustumSphere(TgcFrustum frustum, TgcBoundingSphere sphere)
        {
            float distance;
            FrustumResult result = FrustumResult.INSIDE;
            Plane[] frustumPlanes = frustum.FrustumPlanes;

            for (int i = 0; i < 6; i++)
            {
                distance = distPointPlane(sphere.Center, frustumPlanes[i]);

                if (distance < - sphere.Radius)
                {
                    return FrustumResult.OUTSIDE;
                }
                else if (distance < sphere.Radius)
                {
                    result = FrustumResult.INTERSECT;
                }
                    
            }
            return result;
        }



        #endregion


        #region ConvexPolyhedron

        /// <summary>
        /// Clasifica un BoundingBox respecto de un Cuerpo Convexo.
        /// Los planos del Cuerpo Convexo deben apuntar hacia adentro.
        /// </summary>
        /// <param name="polyhedron">Cuerpo convexo</param>
        /// <param name="aabb">BoundingBox</param>
        /// <returns>Resultado de la clasificaci�n</returns>
        public static ConvexPolyhedronResult classifyConvexPolyhedronAABB(TgcConvexPolyhedron polyhedron, TgcBoundingBox aabb)
        {
            int totalIn = 0;
            Plane[] polyhedronPlanes = polyhedron.Planes;

            // get the corners of the box into the vCorner array
            Vector3[] aabbCorners = aabb.computeCorners();

            // test all 8 corners against the polyhedron sides 
            // if all points are behind 1 specific plane, we are out
            // if we are in with all points, then we are fully in
            for (int p = 0; p < polyhedronPlanes.Length; ++p)
            {
                int inCount = 8;
                int ptIn = 1;

                for (int i = 0; i < 8; ++i)
                {
                    // test this point against the planes
                    if (classifyPointPlane(aabbCorners[i], polyhedronPlanes[p]) == PointPlaneResult.BEHIND)
                    {
                        ptIn = 0;
                        --inCount;
                    }
                }

                // were all the points outside of plane p?
                if (inCount == 0)
                {
                    return ConvexPolyhedronResult.OUTSIDE;
                }

                // check if they were all on the right side of the plane
                totalIn += ptIn;
            }

            // so if iTotalIn is 6, then all are inside the view
            if (totalIn == 6)
            {
                return ConvexPolyhedronResult.INSIDE;
            }

            // we must be partly in then otherwise
            return ConvexPolyhedronResult.INTERSECT;
        }

        /// <summary>
        /// Resultado de una colisi�n entre un objeto y un Cuerpo Convexo
        /// </summary>
        public enum ConvexPolyhedronResult
        {
            /// <summary>
            /// El objeto se encuentra completamente fuera del Cuerpo Convexo
            /// </summary>
            OUTSIDE,

            /// <summary>
            /// El objeto se encuentra completamente dentro del Cuerpo Convexo
            /// </summary>
            INSIDE,

            /// <summary>
            /// El objeto posee parte fuera y parte dentro del Cuerpo Convexo
            /// </summary>
            INTERSECT,
        }

        /// <summary>
        /// Clasifica un punto respecto de un Cuerpo Convexo de N caras.
        /// Puede devolver OUTSIDE o INSIDE (si es coincidente se considera como INSIDE).
        /// Los planos del Cuerpo Convexo deben apuntar hacia adentro.
        /// </summary>
        /// <param name="q">Punto a clasificar</param>
        /// <param name="polyhedron">Cuerpo Convexo</param>
        /// <returns>Resultado de la clasificaci�n</returns>
        public static ConvexPolyhedronResult classifyPointConvexPolyhedron(Vector3 q, TgcConvexPolyhedron polyhedron)
        {
            bool fistTime = true;
            PointPlaneResult lastC = PointPlaneResult.BEHIND;
            PointPlaneResult c;

            for (int i = 0; i < polyhedron.Planes.Length; i++)
            {
                c = TgcCollisionUtils.classifyPointPlane(q, polyhedron.Planes[i]);

                if (c == PointPlaneResult.COINCIDENT)
                    continue;

                //guardar clasif para primera vez
                if (fistTime)
                {
                    fistTime = false;
                    lastC = c;
                }


                //comparar con ultima clasif
                if (c != lastC)
                {
                    //basta con que haya una distinta para que este Afuera
                    return ConvexPolyhedronResult.OUTSIDE;
                }
            }

            //Si todos dieron el mismo resultado, entonces esta adentro
            return ConvexPolyhedronResult.INSIDE;
        }

        /// <summary>
        /// Indica si un punto se encuentra dentro de un Cuerpo Convexo.
        /// Los planos del Cuerpo Convexo deben apuntar hacia adentro.
        /// Es m�s �gil que llamar a classifyPointConvexPolyhedron()
        /// </summary>
        /// <param name="q">Punto a clasificar</param>
        /// <param name="polyhedron">Cuerpo Convexo</param>
        /// <returns>True si se encuentra adentro.</returns>
        public static bool testPointConvexPolyhedron(Vector3 q, TgcConvexPolyhedron polyhedron)
        {
            for (int i = 0; i < polyhedron.Planes.Length; i++)
            {
                //Si el punto est� detr�s de alg�n plano, entonces est� afuera
                if (TgcCollisionUtils.classifyPointPlane(q, polyhedron.Planes[i]) == PointPlaneResult.BEHIND)
                {
                    return false;
                }
            }
            //Si est� delante de todos los planos, entonces est� adentro.
            return true;
        }

        #endregion


        #region Convex Polygon

        /// <summary>
        /// Recorta un pol�gono convexo en 3D por un plano.
        /// Devuelve el nuevo pol�gono recortado.
        /// Algoritmo de Sutherland-Hodgman
        /// </summary>
        /// <param name="poly">V�rtices del pol�gono a recortar</param>
        /// <param name="p">Plano con el cual se recorta</param>
        /// <param name="clippedPoly">V�rtices del pol�gono recortado></param>
        /// <returns>True si el pol�gono recortado es v�lido. False si est� degenerado</returns>
        public static bool clipConvexPolygon(Vector3[] polyVertices, Plane p, out Vector3[] clippedPolyVertices)
        {
            int thisInd = polyVertices.Length - 1;
            PointPlaneResult thisRes = classifyPointPlane(polyVertices[thisInd], p);
            List<Vector3> outVert = new List<Vector3>(polyVertices.Length);
            float t;

            for (int nextInd = 0; nextInd < polyVertices.Length; nextInd++)
			{
                PointPlaneResult nextRes = classifyPointPlane(polyVertices[nextInd], p);
                if(thisRes == PointPlaneResult.IN_FRONT_OF || thisRes == PointPlaneResult.COINCIDENT)
                {
                    // Add the point
                    outVert.Add(polyVertices[thisInd]);
                }

                if((thisRes == PointPlaneResult.BEHIND && nextRes == PointPlaneResult.IN_FRONT_OF) ||
                    thisRes == PointPlaneResult.IN_FRONT_OF && nextRes == PointPlaneResult.BEHIND)
                {
                    // Add the split point
                    Vector3 q;
                    intersectSegmentPlane(polyVertices[thisInd], polyVertices[nextInd], p, out t, out q);
                    outVert.Add(q);
                }

                thisInd = nextInd;
                thisRes = nextRes;
			}

            //Pol�gono v�lido
            if (outVert.Count >= 3)
            {
                clippedPolyVertices = outVert.ToArray();
                return true;
            }

            //Pol�gono degenerado
            clippedPolyVertices = null;
            return false;
        }

        #endregion


        #region Herramientas generales

        /// <summary>
        /// Crea un vector en base a los valores A, B y C de un plano
        /// </summary>
        public static Vector3 toVector3(Plane p)
        {
            return new Vector3(p.A, p.B, p.C);
        }

        /// <summary>
        /// Crea un array de floats con X,Y,Z
        /// </summary>
        public static float[] toArray(Vector3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        /// <summary>
        /// Crea un vector en base a un array de floats con X,Y,Z
        /// </summary>
        public static Vector3 toVector3(float[] a)
        {
            return new Vector3(a[0], a[1], a[2]);
        }

        /// <summary>
        /// Invierte el valor de dos floats
        /// </summary>
        public static void swap(ref float t1, ref float t2)
        {
            float aux = t1;
            t1 = t2;
            t2 = aux;
        }

        /// <summary>
        /// Devuelve un Vector3 con la normal del plano (sin normalizar)
        /// </summary>
        public static Vector3 getPlaneNormal(Plane p)
        {
            return new Vector3(p.A, p.B, p.C);
        }

        #endregion







        
    }


    



    

}
