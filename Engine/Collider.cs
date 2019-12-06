using IniParser;
using IniParser.Model;
using System.Diagnostics;

namespace Fenrir
{
    public class Collider
    {
        public int CollisionIndex = 0;
        public int CollisionField = 0;
        public int Id = 0;
        public string Name = "";
        public int[] CollisionBounds;

        // check collision with other entities
        public bool CheckCollision(Collider collider, int location = 0)
        {
            bool isColliding = (CollisionField | (1 << collider.CollisionIndex)) == CollisionField;

            if (CollisionBounds == null || CollisionBounds.Length == 0)
            {
                return isColliding;
            }

            return CollisionBounds[location] == 0 ? false : isColliding;
        }

        // check collision with tiles
        public bool CheckCollision(int collisionIndex, int location = 0)
        {
            bool isColliding = (CollisionField | (1 << collisionIndex)) == CollisionField;

            if (CollisionBounds == null || CollisionBounds.Length == 0)
            {
                return isColliding;
            }

            return CollisionBounds[location] == 0 ? false : isColliding;
        }

        public Collider(int id = 0, string name = "")
        {
            Id = id;
            Name = name;
        }

        public Collider(int[] bounds, int id = 0, string name = "")
        {
            Id = id;
            Name = name;

            CollisionBounds = bounds;
        }
    }

    public class ColliderGenerator
    {
        public void LoadCollisionData()
        {
            string collisionIniFilePath = "./" + Constants.AssetsRoot + "/collision.ini";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(collisionIniFilePath);

            int collidersCount = int.Parse(data["Collision"]["Count"]);

            Collider none = new Collider(-1, "None");
            none.CollisionField = 0;
            Constants.LoadedColliders.Add(none.Name, none);

            for (int idx = 0; idx < collidersCount; idx++)
            {
                string cSection = idx.ToString();

                Collider collider = spawnCollider(data[cSection]);
                Constants.LoadedColliders.Add(collider.Name, collider);
            }
        }

        public static Collider GetCollider(string id)
        {
            Constants.LoadedColliders.TryGetValue(id, out Collider collider);
            Debug.Assert(collider != null, "Collider is null. Have you checked your Collider count?");

            return collider;
        }

        private Collider spawnCollider(KeyDataCollection data)
        {
                string colliderName = data["Name"].Trim('"');
                _ = int.TryParse(data["Id"], out int id);
                string collidesWithRaw = data["CollidesWith"];

                var tokens = collidesWithRaw.Split(',');

                Collider collider = new Collider(id, colliderName);
                collider.CollisionIndex = id;

                foreach (string rawVal in tokens)
                {
                    bool isOk = int.TryParse(rawVal, out int value);
                    Debug.Assert(isOk, "Failed to parse collision value for " + colliderName);

                    collider.CollisionField |= (1 << value);
                }

                return collider;
        }
    }
}