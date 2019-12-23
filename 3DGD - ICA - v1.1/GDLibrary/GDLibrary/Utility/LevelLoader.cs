using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class LevelLoader
    {
        private static readonly Color ColorLevelLoaderIgnore = Color.White;

        private Dictionary<string, DrawnActor3D> objectArchetypeDictionary;
        private ContentDictionary<Texture2D> textureDictionary;

        public LevelLoader(Dictionary<string, DrawnActor3D> objectArchetypeDictionary,
            ContentDictionary<Texture2D> textureDictionary)
        {
            this.objectArchetypeDictionary = objectArchetypeDictionary;
            this.textureDictionary = textureDictionary;
        }

        public List<DrawnActor3D> Load(Texture2D texture,
            float scaleX, float scaleZ, float height, Vector3 offset)
        {
            List<DrawnActor3D> list = new List<DrawnActor3D>();
            Color[] colorData = new Color[texture.Height * texture.Width];
            texture.GetData<Color>(colorData);

            Color color; 
            Vector3 position;
            DrawnActor3D actor;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    color = colorData[x + y * texture.Width];

                    if (!color.Equals(ColorLevelLoaderIgnore))
                    {
                        //scale allows us to increase the separation between objects in the XZ plane
                        position = new Vector3(x * scaleX, height, y * scaleZ);

                        //the offset allows us to shift the whole set of objects in X, Y, and Z      
                        position += offset;

                        actor = getObjectFromColor(color, position);

                        if (actor != null)
                            list.Add(actor);
                    }
                } //end for x
            } //end for y
            return list;
        }

        private Random rand = new Random();
        private DrawnActor3D getObjectFromColor(Color color, Vector3 position)
        {

            //if the pixel is red then draw a tall (stretched collidable unlit cube)
            if (color.Equals(new Color(255, 0, 0)))  
            {
                //noticed I had to use the literal string for the name in the dictionary i.e. AppData.UnlitTexturedCubeArchetypeID
                //this was the name I used when adding the archetype in Main::LoadArchetypePrimitivesToDictionary()
                CollidablePrimitiveObject collidablePrimitiveObject = objectArchetypeDictionary["unlit tex cube archetype"] as CollidablePrimitiveObject;
                CollidablePrimitiveObject actor = collidablePrimitiveObject.Clone() as CollidablePrimitiveObject;
                //set texture, color, alpha etc
                actor.EffectParameters.Texture = this.textureDictionary["crate1"];
                actor.Transform.Scale = new Vector3(4, 12, 4);
                actor.Transform.Translation = position + new Vector3(0, 6, 0); //a little vertical offset to move up based on scale.Y = 12
                return actor;
            }
            //add an else if for each type of object that you want to load...

            return null;
        }


    }
}
