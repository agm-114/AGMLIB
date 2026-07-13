using Random = System.Random;

namespace AGMLIB.Editor
{
    public class MaterialRandomizer : MonoBehaviour
    {
        public List<Material> Materials = new(2);
        public MeshRenderer Target;

        public void Awake()
        {
            //Debug.LogError("Randomizing: " + Materials.Count());
            Target.material = Materials[new Random().Next(Materials.Count())];
        }
    }
}
