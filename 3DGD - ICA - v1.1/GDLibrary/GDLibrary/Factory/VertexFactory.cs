using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{

    public static class VertexFactory
    {

        //defined vertices for a new shape in our game
        public static VertexPositionColor[] GetColoredTriangle(out PrimitiveType primitiveType,
                                out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 1;

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.White); //T
            vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.White); //R
            vertices[2] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.White); //L
            return vertices;
        }

        //TriangleStrip
        public static VertexPositionColorTexture[] GetTextureQuadVertices(out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            float halfLength = 0.5f;

            Vector3 topLeft = new Vector3(-halfLength, halfLength, 0);
            Vector3 topRight = new Vector3(halfLength, halfLength, 0);
            Vector3 bottomLeft = new Vector3(-halfLength, -halfLength, 0);
            Vector3 bottomRight = new Vector3(halfLength, -halfLength, 0);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(topLeft, Color.White, Vector2.Zero);
            vertices[1] = new VertexPositionColorTexture(topRight, Color.White, Vector2.UnitX);
            vertices[2] = new VertexPositionColorTexture(bottomLeft, Color.White, Vector2.UnitY);
            vertices[3] = new VertexPositionColorTexture(bottomRight, Color.White, Vector2.One);

            return vertices;
        }

        public static VertexPositionColor[] GetSpiralVertices(int radius, int angleInDegrees,
                            float verticalIncrement, out PrimitiveType primitiveType, out int primitiveCount)
        {
            VertexPositionColor[] vertices = GetCircleVertices(radius, angleInDegrees,
                 out primitiveType,
                out primitiveCount, 
                OrientationType.XZAxis);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position.Y = verticalIncrement * i;
            }

            return vertices;
        }


        public static VertexPositionColor[] GetSphereVertices(int radius, int angleInDegrees,
             out PrimitiveType primitiveType,
                            out int primitiveCount)
        {
            List<VertexPositionColor> vertList = new List<VertexPositionColor>();

            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.XYAxis));
            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.YZAxis));
            vertList.AddRange(GetCircleVertices(radius, angleInDegrees, out primitiveType, out primitiveCount, OrientationType.XZAxis));
            primitiveCount = vertList.Count - 1;      
            return vertList.ToArray();
        }

        public static VertexPositionColor[] GetCircleVertices(int radius, int angleInDegrees, 
            out PrimitiveType primitiveType,
            out int primitiveCount,  OrientationType orientationType)
        {
            primitiveType = PrimitiveType.LineStrip;
            primitiveCount = 360 / angleInDegrees;
            VertexPositionColor[] vertices = new VertexPositionColor[primitiveCount + 1];

            Vector3 position = Vector3.Zero;
            float angleInRadians = MathHelper.ToRadians(angleInDegrees);

            for (int i = 0; i <= primitiveCount; i++)
            {
                if (orientationType == OrientationType.XYAxis)
                {
                    position.X = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Y = (float)(radius * Math.Sin(i * angleInRadians));
                }
                else if (orientationType == OrientationType.XZAxis)
                {
                    position.X = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Z = (float)(radius * Math.Sin(i * angleInRadians));
                }
                else
                {
                    position.Y = (float)(radius * Math.Cos(i * angleInRadians));
                    position.Z = (float)(radius * Math.Sin(i * angleInRadians));
                }

                vertices[i] = new VertexPositionColor(position, Color.White);
            }
            return vertices;
        }


        /******************************************** Textured - Quad, Cube & Pyramid ********************************************/

        public static VertexPositionColorTexture[] GetVerticesPositionColorTextureQuad(int sidelength,
                    out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeft = new Vector3(-halfSideLength, halfSideLength, 0);
            Vector3 topRight = new Vector3(halfSideLength, halfSideLength, 0);
            Vector3 bottomLeft = new Vector3(-halfSideLength, -halfSideLength, 0);
            Vector3 bottomRight = new Vector3(halfSideLength, -halfSideLength, 0);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            vertices[0] = new VertexPositionColorTexture(topLeft, Color.White, Vector2.Zero);
            vertices[1] = new VertexPositionColorTexture(topRight, Color.White, Vector2.UnitX);
            vertices[2] = new VertexPositionColorTexture(bottomLeft, Color.White, Vector2.UnitY);
            vertices[3] = new VertexPositionColorTexture(bottomRight, Color.White, Vector2.One);

            return vertices;
        }

        public static VertexPositionColorTexture[] GetVerticesPositionTexturedCube(int sidelength,
                    out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[36];

            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeftFront = new Vector3(-halfSideLength, halfSideLength, halfSideLength);
            Vector3 topLeftBack = new Vector3(-halfSideLength, halfSideLength, -halfSideLength);
            Vector3 topRightFront = new Vector3(halfSideLength, halfSideLength, halfSideLength);
            Vector3 topRightBack = new Vector3(halfSideLength, halfSideLength, -halfSideLength);

            Vector3 bottomLeftFront = new Vector3(-halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomLeftBack = new Vector3(-halfSideLength, -halfSideLength, -halfSideLength);
            Vector3 bottomRightFront = new Vector3(halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomRightBack = new Vector3(halfSideLength, -halfSideLength, -halfSideLength);

            //uv coordinates
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);


            //top - 1 polygon for the top
            vertices[0] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[1] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[2] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);

            vertices[3] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[4] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[5] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);

            //front
            vertices[6] = new VertexPositionColorTexture(topLeftFront, Color.White, uvBottomLeft);
            vertices[7] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);
            vertices[8] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);

            vertices[9] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[10] = new VertexPositionColorTexture(topRightFront, Color.White, uvBottomRight);
            vertices[11] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvTopRight);

            //back
            vertices[12] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[13] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[14] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);

            vertices[15] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[16] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[17] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);

            //left 
            vertices[18] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[19] = new VertexPositionColorTexture(topLeftFront, Color.White, uvTopRight);
            vertices[20] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvBottomRight);

            vertices[21] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);
            vertices[22] = new VertexPositionColorTexture(topLeftBack, Color.White, uvTopLeft);
            vertices[23] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvBottomRight);

            //right
            vertices[24] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvBottomLeft);
            vertices[25] = new VertexPositionColorTexture(topRightFront, Color.White, uvTopLeft);
            vertices[26] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            vertices[27] = new VertexPositionColorTexture(topRightFront, Color.White, uvTopLeft);
            vertices[28] = new VertexPositionColorTexture(topRightBack, Color.White, uvTopRight);
            vertices[29] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            //bottom
            vertices[30] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[31] = new VertexPositionColorTexture(bottomRightFront, Color.White, uvTopRight);
            vertices[32] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);

            vertices[33] = new VertexPositionColorTexture(bottomLeftFront, Color.White, uvTopLeft);
            vertices[34] = new VertexPositionColorTexture(bottomRightBack, Color.White, uvBottomRight);
            vertices[35] = new VertexPositionColorTexture(bottomLeftBack, Color.White, uvBottomLeft);

            return vertices;
        }

        public static VertexPositionColorTexture[] GetVerticesPositionTexturedPyramidSquare(int sidelength,
                    out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 6;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[18];
            float halfSideLength = sidelength / 2.0f;

            Vector3 topCentre = new Vector3(0, 0.71f * sidelength, 0); //multiplier gives a pyramid where the length of the rising edges == length of the base edges
            Vector3 frontLeft = new Vector3(-halfSideLength, 0, halfSideLength);
            Vector3 frontRight = new Vector3(halfSideLength, 0, halfSideLength);
            Vector3 backLeft = new Vector3(-halfSideLength, 0, -halfSideLength);
            Vector3 backRight = new Vector3(halfSideLength, 0, -halfSideLength);

            Vector2 uvTopCentre = new Vector2(0.5f, 0);
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //front 
            vertices[0] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[1] = new VertexPositionColorTexture(frontRight, Color.White, uvBottomRight);
            vertices[2] = new VertexPositionColorTexture(frontLeft, Color.White, uvBottomLeft);

            //left 
            vertices[3] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[4] = new VertexPositionColorTexture(frontLeft, Color.White, uvBottomRight);
            vertices[5] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            //right 
            vertices[6] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[7] = new VertexPositionColorTexture(backRight, Color.White, uvBottomRight);
            vertices[8] = new VertexPositionColorTexture(frontRight, Color.White, uvBottomLeft);

            //back 
            vertices[9] = new VertexPositionColorTexture(topCentre, Color.White, uvTopCentre);
            vertices[10] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomRight);
            vertices[11] = new VertexPositionColorTexture(backRight, Color.White, uvBottomLeft);

            //bottom 
            vertices[12] = new VertexPositionColorTexture(frontLeft, Color.White, uvTopLeft);
            vertices[13] = new VertexPositionColorTexture(frontRight, Color.White, uvTopRight);
            vertices[14] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            vertices[15] = new VertexPositionColorTexture(frontRight, Color.White, uvTopRight);
            vertices[16] = new VertexPositionColorTexture(backRight, Color.White, uvBottomRight);
            vertices[17] = new VertexPositionColorTexture(backLeft, Color.White, uvBottomLeft);

            return vertices;
        }

        /******************************************** Textured & Normal - Cube ********************************************/

        //adding normals - step 1 - add the vertices for the object shape
        public static VertexPositionNormalTexture[] GetVerticesPositionNormalTexturedCube(int sidelength,
                    out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleList;
            primitiveCount = 12;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[36];

            float halfSideLength = sidelength / 2.0f;

            Vector3 topLeftFront = new Vector3(-halfSideLength, halfSideLength, halfSideLength);
            Vector3 topLeftBack = new Vector3(-halfSideLength, halfSideLength, -halfSideLength);
            Vector3 topRightFront = new Vector3(halfSideLength, halfSideLength, halfSideLength);
            Vector3 topRightBack = new Vector3(halfSideLength, halfSideLength, -halfSideLength);

            Vector3 bottomLeftFront = new Vector3(-halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomLeftBack = new Vector3(-halfSideLength, -halfSideLength, -halfSideLength);
            Vector3 bottomRightFront = new Vector3(halfSideLength, -halfSideLength, halfSideLength);
            Vector3 bottomRightBack = new Vector3(halfSideLength, -halfSideLength, -halfSideLength);

            //uv coordinates
            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);


            //top - 1 polygon for the top
            vertices[0] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitY, uvBottomLeft);
            vertices[1] = new VertexPositionNormalTexture(topLeftBack, Vector3.UnitY, uvTopLeft);
            vertices[2] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitY, uvTopRight);

            vertices[3] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitY, uvBottomLeft);
            vertices[4] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitY, uvTopRight);
            vertices[5] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitY, uvBottomRight);

            //front
            vertices[6] = new VertexPositionNormalTexture(topLeftFront, Vector3.UnitZ, uvBottomLeft);
            vertices[7] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitZ, uvBottomRight);
            vertices[8] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitZ, uvTopLeft);

            vertices[9] = new VertexPositionNormalTexture(bottomLeftFront, Vector3.UnitZ, uvTopLeft);
            vertices[10] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitZ, uvBottomRight);
            vertices[11] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitZ, uvTopRight);

            //back
            vertices[12] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitZ, uvBottomRight);
            vertices[13] = new VertexPositionNormalTexture(topRightBack, -Vector3.UnitZ, uvTopRight);
            vertices[14] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitZ, uvTopLeft);

            vertices[15] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitZ, uvBottomRight);
            vertices[16] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitZ, uvTopLeft);
            vertices[17] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitZ, uvBottomLeft);

            //left 
            vertices[18] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitX, uvTopLeft);
            vertices[19] = new VertexPositionNormalTexture(topLeftFront, -Vector3.UnitX, uvTopRight);
            vertices[20] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitX, uvBottomRight);

            vertices[21] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitX, uvBottomLeft);
            vertices[22] = new VertexPositionNormalTexture(topLeftBack, -Vector3.UnitX, uvTopLeft);
            vertices[23] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitX, uvBottomRight);

            //right
            vertices[24] = new VertexPositionNormalTexture(bottomRightFront, Vector3.UnitX, uvBottomLeft);
            vertices[25] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, uvTopLeft);
            vertices[26] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, uvBottomRight);

            vertices[27] = new VertexPositionNormalTexture(topRightFront, Vector3.UnitX, uvTopLeft);
            vertices[28] = new VertexPositionNormalTexture(topRightBack, Vector3.UnitX, uvTopRight);
            vertices[29] = new VertexPositionNormalTexture(bottomRightBack, Vector3.UnitX, uvBottomRight);

            //bottom
            vertices[30] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitY, uvTopLeft);
            vertices[31] = new VertexPositionNormalTexture(bottomRightFront, -Vector3.UnitY, uvTopRight);
            vertices[32] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitY, uvBottomRight);

            vertices[33] = new VertexPositionNormalTexture(bottomLeftFront, -Vector3.UnitY, uvTopLeft);
            vertices[34] = new VertexPositionNormalTexture(bottomRightBack, -Vector3.UnitY, uvBottomRight);
            vertices[35] = new VertexPositionNormalTexture(bottomLeftBack, -Vector3.UnitY, uvBottomLeft);

            return vertices;
        }


        //returns the vertices for a billboard which has a custom vertex declaration
        public static VertexBillboard[] GetVertexBillboard(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            VertexBillboard[] vertices = new VertexBillboard[4];
            float halfSideLength = sidelength / 2.0f;

            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            vertices[0] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopLeft, -halfSideLength, halfSideLength));
            vertices[1] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopRight, halfSideLength, halfSideLength));
            vertices[2] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomLeft, -halfSideLength, -halfSideLength));
            vertices[3] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomRight, halfSideLength, -halfSideLength));

            return vertices;
        }





    }
}
