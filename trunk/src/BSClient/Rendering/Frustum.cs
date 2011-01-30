using System;
using System.Collections.Generic;
using OpenTK;

// This is broken - ignore it.
namespace JollyBit.BS.Client.Rendering
{
    public static class Frustum
    {
        // Is the sphere entirely outside (returns false) or at least partially inside (returns true) the given plane?
        public static bool InsidePlane(Vector4 planeEquation, Vector3 center, float radius) {
            float result = Vector3.Dot(planeEquation.Xyz, center) + planeEquation.W;
            //Console.WriteLine("{0} (r: {1}) -> {2} (plane: {3})",center,radius,result,planeEquation);
            return (result >= 0 || Math.Abs(result) < radius);
        }

        // Is the point inside (returns true) or outside (returns false) the given plane?
        public static bool InsidePlane(Vector4 planeEquation, Vector3 point) {
            return ( (Vector3.Dot(planeEquation.Xyz, point) + planeEquation.W) >= 0 );
        }

        public static bool InsideFrustum(Vector4[] planes, Vector3 point) {
            for(uint i = 0; i < 6; i++)
                if( !InsidePlane(planes[i], point) )
                    return false;
            return true;
        }

        public static bool InsideFrustum(Vector4[] planes, Vector3 center, float radius) {
            for(uint i = 0; i < 6; i++)
                if(!InsidePlane(planes[i], center, radius))
                    return false;
            return true;
        }

        public static Vector4[] CalculatePlanes(Matrix4 mModelView, Matrix4 mProjection) {
            return CalculatePlanes(mModelView,mProjection,true);
        }

        public static Vector4[] CalculatePlanes(Matrix4 mModelView, Matrix4 mProjection, bool normalize) {
            Vector4[] planes = new Vector4[6];
            Matrix4 mat = mProjection * mModelView;

            planes[0].X = mat.M41 + mat.M11;
            planes[0].Y = mat.M42 + mat.M12;
            planes[0].Z = mat.M43 + mat.M13;
            planes[0].W = mat.M44 + mat.M14;

            planes[1].X = mat.M41 - mat.M11;
            planes[1].Y = mat.M42 - mat.M12;
            planes[1].Z = mat.M43 - mat.M13;
            planes[1].W = mat.M44 - mat.M14;

            planes[2].X = mat.M41 - mat.M21;
            planes[2].Y = mat.M42 - mat.M22;
            planes[2].Z = mat.M43 - mat.M23;
            planes[2].W = mat.M44 - mat.M24;

            planes[3].X = mat.M41 + mat.M21;
            planes[3].Y = mat.M42 + mat.M22;
            planes[3].Z = mat.M43 + mat.M23;
            planes[3].W = mat.M44 + mat.M24;

            planes[4].X = mat.M41 + mat.M31;
            planes[4].Y = mat.M42 + mat.M32;
            planes[4].Z = mat.M43 + mat.M33;
            planes[4].W = mat.M44 + mat.M34;

            planes[5].X = mat.M41 - mat.M31;
            planes[5].Y = mat.M42 - mat.M32;
            planes[5].Z = mat.M43 - mat.M33;
            planes[5].W = mat.M44 - mat.M34;

            if(normalize) {
                for(uint i = 0; i < 6; i++)
                    planes[i].Normalize();
            }

            return planes;
        }
    }
}

